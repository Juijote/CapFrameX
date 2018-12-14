﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace CapFrameX.OcatInterface
{
    public static class RecordManager
    {
        public static Session LoadData(string csvFile)
        {
            if (csvFile == null || csvFile == "")
            {
                return null;
            }

            var session = new Session
            {
                Path = csvFile,
                IsVR = false
            };

			int index = csvFile.LastIndexOf('\\');
			session.Filename = csvFile.Substring(index + 1);

			session.FrameStart = new List<double>();
			session.FrameEnd = new List<double>();
			session.FrameTimes = new List<double>();
			session.ReprojectionStart = new List<double>();
			session.ReprojectionEnd = new List<double>();
			session.ReprojectionTimes = new List<double>();
			session.VSync = new List<double>();
			session.AppMissed = new List<bool>();
			session.WarpMissed = new List<bool>();

			session.AppMissesCount = 0;
			session.WarpMissesCount = 0;
			session.ValidAppFrames = 0;
			session.LastFrameTime = 0;
			session.ValidReproFrames = 0;
			session.LastReprojectionTime = 0;

			try
			{
				using (var reader = new StreamReader(csvFile))
				{
					// header -> csv layout may differ, identify correct columns based on column title
					var line = reader.ReadLine();
					int indexFrameStart = 0;
					int indexFrameTimes = 0;
					int indexFrameEnd = 0;
					int indexReprojectionStart = 0;
					int indexReprojectionTimes = 0;
					int indexReprojectionEnd = 0;
					int indexVSync = 0;
					int indexAppMissed = 0;
					int indexWarpMissed = 0;

					var metrics = line.Split(',');
					for (int i = 0; i < metrics.Count(); i++)
					{
						if (String.Compare(metrics[i], "AppRenderStart") == 0 || String.Compare(metrics[i], "TimeInSeconds") == 0)
						{
							indexFrameStart = i;
						}
						// MsUntilRenderComplete needs to be added to AppRenderStart to get the timestamp
						if (String.Compare(metrics[i], "AppRenderEnd") == 0 || String.Compare(metrics[i], "MsUntilRenderComplete") == 0)
						{
							indexFrameEnd = i;
						}
						if (String.Compare(metrics[i], "MsBetweenAppPresents") == 0 || String.Compare(metrics[i], "MsBetweenPresents") == 0)
						{
							indexFrameTimes = i;
						}
						if (String.Compare(metrics[i], "ReprojectionStart") == 0)
						{
							indexReprojectionStart = i;
						}
						//MsUntilDisplayed needs to be added to AppRenderStart, we don't have a reprojection start timestamp in this case
						if (String.Compare(metrics[i], "ReprojectionEnd") == 0 || String.Compare(metrics[i], "MsUntilDisplayed") == 0)
						{
							indexReprojectionEnd = i;
						}
						if (String.Compare(metrics[i], "MsBetweenReprojections") == 0 || String.Compare(metrics[i], "MsBetweenLsrs") == 0)
						{
							indexReprojectionTimes = i;
						}
						if (String.Compare(metrics[i], "VSync") == 0)
						{
							indexVSync = i;
							session.IsVR = true;
						}
						if (String.Compare(metrics[i], "AppMissed") == 0 || String.Compare(metrics[i], "Dropped") == 0)
						{
							indexAppMissed = i;
						}
						if (String.Compare(metrics[i], "WarpMissed") == 0 || String.Compare(metrics[i], "LsrMissed") == 0)
						{
							indexWarpMissed = i;
						}
					}

					while (!reader.EndOfStream)
					{
						line = reader.ReadLine();
						var values = line.Split(',');

						// non VR titles only have app render start and frame times metrics
						// app render end and reprojection end get calculated based on ms until render complete and ms until displayed metric
						if (double.TryParse(values[indexFrameStart], NumberStyles.Any, CultureInfo.InvariantCulture, out var frameStart)
							&& double.TryParse(values[indexFrameTimes], NumberStyles.Any, CultureInfo.InvariantCulture, out var frameTimes)
							&& int.TryParse(values[indexAppMissed], NumberStyles.Any, CultureInfo.InvariantCulture, out var appMissed))
						{
							if (frameStart > 0)
							{
								session.ValidAppFrames++;
								session.LastFrameTime = frameStart;
							}
							session.FrameStart.Add(frameStart);
							session.FrameTimes.Add(frameTimes);

							session.AppMissed.Add(Convert.ToBoolean(appMissed));
							session.AppMissesCount += appMissed;
						}

						if (double.TryParse(values[indexFrameEnd], NumberStyles.Any, CultureInfo.InvariantCulture, out var frameEnd)
							&& double.TryParse(values[indexReprojectionEnd], NumberStyles.Any, CultureInfo.InvariantCulture, out var reprojectionEnd))
						{
							if (session.IsVR)
							{
								session.FrameEnd.Add(frameEnd);
								session.ReprojectionEnd.Add(reprojectionEnd);
							}
							else
							{
								session.FrameEnd.Add(frameStart + frameEnd / 1000.0);
								session.ReprojectionEnd.Add(frameStart + reprojectionEnd / 1000.0);
							}
						}

						if (double.TryParse(values[indexReprojectionStart], NumberStyles.Any, CultureInfo.InvariantCulture, out var reprojectionStart)
						 && double.TryParse(values[indexReprojectionTimes], NumberStyles.Any, CultureInfo.InvariantCulture, out var reprojectionTimes)
						 && double.TryParse(values[indexVSync], NumberStyles.Any, CultureInfo.InvariantCulture, out var vSync)
						 && int.TryParse(values[indexWarpMissed], NumberStyles.Any, CultureInfo.InvariantCulture, out var warpMissed))
						{
							if (reprojectionStart > 0)
							{
								session.ValidReproFrames++;
								session.LastReprojectionTime = reprojectionStart;
							}
							session.ReprojectionStart.Add(reprojectionStart);
							session.ReprojectionTimes.Add(reprojectionTimes);
							session.VSync.Add(vSync);
							session.WarpMissed.Add(Convert.ToBoolean(warpMissed));
							session.WarpMissesCount += warpMissed;
						}
					}
				}
			}
			catch (IOException)
			{
				return null;
			}

			return session;
        }
    }
}

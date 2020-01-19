﻿using CapFrameX.Configuration;
using CapFrameX.Data;
using CapFrameX.Statistics;
using CapFrameX.ViewModel;
using Prism.Events;
using System;
using System.ComponentModel;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CapFrameX.View
{
	/// <summary>
	/// Interaction logic for  AggregationView.xaml
	/// </summary>
	public partial class AggregationView : UserControl
	{
		public AggregationView()
		{
			InitializeComponent();

			// Design time!
			if (DesignerProperties.GetIsInDesignMode(this))
			{
				var appConfiguration = new CapFrameXConfiguration();
				var recordDataProvider = new RecordDataProvider(new RecordDirectoryObserver(appConfiguration), appConfiguration);
				DataContext = new AggregationViewModel(new FrametimeStatisticProvider(appConfiguration), recordDataProvider, new EventAggregator(), appConfiguration);
			}

			(DataContext as AggregationViewModel)
				.OutlierFlagStream
				.Throttle(TimeSpan.FromMilliseconds(100))
				.Subscribe(OnOutlierFlagsChanged);
		}

		private void AggregationItemDataGrid_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
		{
			(DataContext as AggregationViewModel).SelectedAggregationEntryIndex = -1;
			Keyboard.ClearFocus();
		}

		private void OnOutlierFlagsChanged(bool[] flags)
		{
			Application.Current.Dispatcher.BeginInvoke(new Action(() =>
			{
				int count = 0;
				foreach (var item in AggregationItemDataGrid.ItemsSource)
				{
					DataGridRow row = (DataGridRow)AggregationItemDataGrid.ItemContainerGenerator.ContainerFromIndex(count);

					if (row != null)
					{
						if (flags[count])
						{
							row.Background = new SolidColorBrush(Color.FromArgb(80, 200, 0, 0));
						}
						else
						{
							row.Background = Brushes.Transparent;
						}
					}

					count++;
				}
			}));
		}
	}
}

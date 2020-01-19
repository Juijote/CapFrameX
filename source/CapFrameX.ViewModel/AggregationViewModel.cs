﻿using CapFrameX.Contracts.Aggregation;
using CapFrameX.Contracts.Configuration;
using CapFrameX.Contracts.Data;
using CapFrameX.Data;
using CapFrameX.EventAggregation.Messages;
using CapFrameX.Extensions;
using CapFrameX.Statistics;
using GongSolutions.Wpf.DragDrop;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CapFrameX.ViewModel
{
	public class AggregationViewModel : BindableBase, INavigationAware, IDropTarget
	{
		private readonly IStatisticProvider _statisticProvider;
		private readonly IRecordDataProvider _recordDataProvider;
		private readonly IEventAggregator _eventAggregator;
		private readonly IAppConfiguration _appConfiguration;

		private bool _useUpdateSession;
		private int _selectedAggregationEntryIndex = -1;
		private bool _showHelpText = true;
		private bool _enableButtons = false;
		private List<IFileRecordInfo> _fileRecordInfoList = new List<IFileRecordInfo>();
		private bool _showResultString;
		private string _aggregationResultString = string.Empty;
		private bool _supressCollectionChanged;

		public int SelectedAggregationEntryIndex
		{
			get
			{
				return _selectedAggregationEntryIndex;
			}
			set
			{
				_selectedAggregationEntryIndex = value;
				RaisePropertyChanged();
			}
		}

		public EMetric SelectedSecondMetric
		{
			get
			{
				return _appConfiguration
				  .SecondMetricAggregation
				  .ConvertToEnum<EMetric>();
			}
			set
			{
				_appConfiguration.SecondMetricAggregation =
					value.ConvertToString();
				UpdateAggregationEntries();
				RaisePropertyChanged();
			}
		}

		public EMetric SelectedThirdMetric
		{
			get
			{
				return _appConfiguration
				  .ThirdMetricAggregation
				  .ConvertToEnum<EMetric>();
			}
			set
			{
				_appConfiguration.ThirdMetricAggregation =
					value.ConvertToString();
				UpdateAggregationEntries();
				RaisePropertyChanged();
			}
		}

		public string SelectedRelatedMetric
		{
			get
			{
				return _appConfiguration.RelatedMetricAggregation;
			}
			set
			{
				_appConfiguration.RelatedMetricAggregation = value;
				UpdateAggregationEntries();
				RaisePropertyChanged();
			}
		}

		public int SelectedOutlierPercentage
		{
			get
			{
				return _appConfiguration.OutlierPercentageAggregation;
			}
			set
			{
				_appConfiguration.OutlierPercentageAggregation = value;
				UpdateAggregationEntries();
				RaisePropertyChanged();
			}
		}

		public bool ShowHelpText
		{
			get
			{
				return _showHelpText;
			}
			set
			{
				_showHelpText = value;
				RaisePropertyChanged();
			}
		}

		public string AggregationResultString
		{
			get
			{
				return _aggregationResultString;
			}
			set
			{
				_aggregationResultString = value;
				RaisePropertyChanged();
			}
		}

		public bool ShowResultString
		{
			get
			{
				return _showResultString;
			}
			set
			{
				_showResultString = value;
				RaisePropertyChanged();
			}
		}

		public bool EnableButtons
		{
			get
			{
				return _enableButtons;
			}
			set
			{
				_enableButtons = value;
				RaisePropertyChanged();
			}
		}

		public Array RelatedMetricItemsSource => new[] { "Average", "Second", "Third" };

		public Array OutlierPercentageItemsSource => Enumerable.Range(2, 9).ToArray();

		public Array SecondMetricItems => Enum.GetValues(typeof(EMetric))
									  .Cast<EMetric>()
									  .Where(metric => metric != EMetric.Average)
									  .ToArray();

		public Array ThirdMetricItems => Enum.GetValues(typeof(EMetric))
											 .Cast<EMetric>()
											 .Where(metric => metric != EMetric.Average)
											 .ToArray();

		public ISubject<bool[]> OutlierFlagStream = new Subject<bool[]>();

		public ICommand ClearTableCommand { get; }
		public ICommand AggregateIncludeCommand { get; }
		public ICommand AggregateExcludeCommand { get; }

		public ObservableCollection<IAggregationEntry> AggregationEntries { get; private set; }
			= new ObservableCollection<IAggregationEntry>();

		public AggregationViewModel(IStatisticProvider statisticProvider, IRecordDataProvider recordDataProvider,
			IEventAggregator eventAggregator, IAppConfiguration appConfiguration)
		{
			_statisticProvider = statisticProvider;
			_recordDataProvider = recordDataProvider;
			_eventAggregator = eventAggregator;
			_appConfiguration = appConfiguration;

			ClearTableCommand = new DelegateCommand(OnClearTable);
			AggregateIncludeCommand = new DelegateCommand(OnAggregateInclude);
			AggregateExcludeCommand = new DelegateCommand(OnAggregateExclude);

			SubscribeToUpdateSession();

			AggregationEntries.CollectionChanged += new NotifyCollectionChangedEventHandler
				((sender, eventArg) => OnAggregationEntriesChanged());
		}

		private void OnAggregationEntriesChanged()
		{
			ShowHelpText = !AggregationEntries.Any();
			EnableButtons = AggregationEntries.Any();

			// Further analysis
			if (AggregationEntries.Count >= 2 && !_supressCollectionChanged)
			{
				var outlierFlags = _statisticProvider
					.GetOutlierAnalysis(AggregationEntries.Select(analysis => analysis.MetricAnalysis).ToList(),
					_appConfiguration.RelatedMetricAggregation, _appConfiguration.OutlierPercentageAggregation);

				OutlierFlagStream.OnNext(outlierFlags);
			}
		}

		private void SubscribeToUpdateSession()
		{
			_eventAggregator.GetEvent<PubSubEvent<ViewMessages.SelectSession>>()
							.Subscribe(msg =>
							{
								if (_useUpdateSession)
								{
									AddAggregationEntry(msg.RecordInfo, msg.CurrentSession);
								}
							});
		}

		private void AddAggregationEntry(IFileRecordInfo recordInfo, Session session)
		{
			if (recordInfo != null)
				_fileRecordInfoList.Add(recordInfo);
			else
				return;

			List<double> frametimes = session?.FrameTimes;

			if (session == null)
			{
				var localSession = RecordManager.LoadData(recordInfo.FullPath);
				frametimes = localSession?.FrameTimes;
			}

			var metricAnalysis = _statisticProvider
				.GetMetricAnalysis(frametimes, SelectedSecondMetric.ConvertToString(),
					SelectedThirdMetric.ConvertToString());

			AggregationEntries.Add(new AggregationEntry()
			{
				GameName = recordInfo.GameName,
				CreationDate = recordInfo.CreationDate,
				CreationTime = recordInfo.CreationTime,
				AverageValue = metricAnalysis.Average,
				SecondMetricValue = metricAnalysis.Second,
				ThirdMetricValue = metricAnalysis.Third,
				MetricAnalysis = metricAnalysis
			});
		}

		private void UpdateAggregationEntries()
		{
			if (!_fileRecordInfoList.Any())
				return;

			AggregationEntries.Clear();

			_supressCollectionChanged = true;
			foreach (var recordInfo in _fileRecordInfoList)
			{
				var localSession = RecordManager.LoadData(recordInfo.FullPath);
				var frametimes = localSession?.FrameTimes;

				var metricAnalysis = _statisticProvider
					.GetMetricAnalysis(frametimes, SelectedSecondMetric.ConvertToString(),
						SelectedThirdMetric.ConvertToString());

				AggregationEntries.Add(new AggregationEntry()
				{
					GameName = recordInfo.GameName,
					CreationDate = recordInfo.CreationDate,
					CreationTime = recordInfo.CreationTime,
					AverageValue = metricAnalysis.Average,
					SecondMetricValue = metricAnalysis.Second,
					ThirdMetricValue = metricAnalysis.Third,
					MetricAnalysis = metricAnalysis
				});
			}
			_supressCollectionChanged = false;
			OnAggregationEntriesChanged();
		}

		private void OnClearTable()
		{
			AggregationEntries.Clear();
			_fileRecordInfoList.Clear();
			AggregationResultString = string.Empty;
			ShowResultString = false;
		}

		private void OnAggregateInclude()
		{
			var concatedFrametimes = new List<double>();

			foreach (var recordInfo in _fileRecordInfoList)
			{
				var localSession = RecordManager.LoadData(recordInfo.FullPath);
				var frametimes = localSession?.FrameTimes;
				concatedFrametimes.AddRange(frametimes);
			}

			var resultString = _statisticProvider
				.GetMetricAnalysis(concatedFrametimes,
				_appConfiguration.SecondMetricAggregation,
				_appConfiguration.ThirdMetricAggregation).ResultString; 

			AggregationResultString = $"Result: {resultString}";
			ShowResultString = true;

			// write aggregated file
			Task.Run(async () =>
			{
				// await _recordDataProvider.SaveAggregatedPresentData(null);
			});
		}

		private void OnAggregateExclude()
		{
			var outlierFlags = _statisticProvider
					.GetOutlierAnalysis(AggregationEntries.Select(analysis => analysis.MetricAnalysis).ToList(),
					_appConfiguration.RelatedMetricAggregation, _appConfiguration.OutlierPercentageAggregation);

			var concatedFrametimes = new List<double>();

			foreach (var recordInfo in _fileRecordInfoList.Where((x, i) => !outlierFlags[i]))
			{
				var localSession = RecordManager.LoadData(recordInfo.FullPath);
				var frametimes = localSession?.FrameTimes;
				concatedFrametimes.AddRange(frametimes);
			}

			var resultString = _statisticProvider
				.GetMetricAnalysis(concatedFrametimes,
				_appConfiguration.SecondMetricAggregation,
				_appConfiguration.ThirdMetricAggregation).ResultString;

			AggregationResultString = $"Result: {resultString}";
			ShowResultString = true;

			// write aggregated file
			Task.Run(async () =>
			{
				// await _recordDataProvider.SaveAggregatedPresentData(null);
			});
		}

		public void OnNavigatedTo(NavigationContext navigationContext)
		{
			_useUpdateSession = true;
		}

		public bool IsNavigationTarget(NavigationContext navigationContext)
		{
			return true;
		}

		public void OnNavigatedFrom(NavigationContext navigationContext)
		{
			_useUpdateSession = false;
		}

		void IDropTarget.Drop(IDropInfo dropInfo)
		{
			if (dropInfo != null)
			{
				if (dropInfo.VisualTarget is FrameworkElement frameworkElement)
				{
					if (frameworkElement.Name == "AggregationItemDataGrid" 
						|| frameworkElement.Name == "DragAndDropInfoTextTextBlock")
					{
						if (dropInfo.Data is IFileRecordInfo recordInfo)
						{
							AddAggregationEntry(recordInfo, null);
						}
					}
				}
			}
		}

		void IDropTarget.DragOver(IDropInfo dropInfo)
		{
			if (dropInfo != null)
			{
				dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
				dropInfo.Effects = DragDropEffects.Move;
			}
		}
	}
}
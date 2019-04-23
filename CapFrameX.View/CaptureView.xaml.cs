﻿using CapFrameX.PresentMonInterface;
using CapFrameX.ViewModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace CapFrameX.View
{
	/// <summary>
	/// Interaction logic for CaptureView.xaml
	/// </summary>
	public partial class CaptureView : UserControl
	{
		public CaptureView()
		{
			InitializeComponent();

			// Design time!
			if (DesignerProperties.GetIsInDesignMode(this))
			{
				DataContext = new CaptureViewModel(new PresentMonCaptureService());
			}
		}

		private void Flipper_IsFlippedChanged(object sender, RoutedPropertyChangedEventArgs<bool> e)
		{
			(DataContext as CaptureViewModel).OnCaptureModeChanged(e.NewValue);
		}
	}
}

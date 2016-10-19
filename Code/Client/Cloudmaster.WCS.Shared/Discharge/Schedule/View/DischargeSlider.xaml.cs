using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using WCS.Shared.Schedule;

namespace WCS.Shared.Discharge.Schedule
{
	public partial class DischargeSlider : UserControl
	{
		//private DispatcherTimer _timer;

		public DischargeSlider()
		{
			InitializeComponent();

			Unloaded += Slider_Unloaded;

			ManipulationStarted += Slider_ManipulationStarted;
			ManipulationDelta += Slider_ManipulationDelta;
			ManipulationCompleted += Slider_ManipulationCompleted;

			MouseMove += Slider_MouseMove;
			MouseLeftButtonDown += Slider_MouseLeftButtonDown;
			MouseLeftButtonUp += Slider_MouseLeftButtonUp;
			MouseLeave += Slider_MouseLeave;

			//_timer = new DispatcherTimer();

			//_timer.Tick += UpdateTimeline_Tick;
			//_timer.Interval = TimeSpan.FromSeconds(60);
			//_timer.Start();
		}

		public bool IsDragEnabled
		{
			get { return (bool)GetValue(IsDragEnabledProperty); }
			set { SetValue(IsDragEnabledProperty, value); }
		}

		public static readonly DependencyProperty IsDragEnabledProperty =
			DependencyProperty.Register("IsDragEnabled", typeof(bool), typeof(DischargeSlider), new UIPropertyMetadata(true));

		void Slider_Unloaded(object sender, RoutedEventArgs e)
		{
			ManipulationStarted -= Slider_ManipulationStarted;
			ManipulationDelta -= Slider_ManipulationDelta;
			ManipulationCompleted -= Slider_ManipulationCompleted;

			MouseMove -= Slider_MouseMove;
			MouseLeftButtonDown -= Slider_MouseLeftButtonDown;
			MouseLeftButtonUp -= Slider_MouseLeftButtonUp;
			MouseLeave += Slider_MouseLeave;

		//	_timer.Tick -= UpdateTimeline_Tick;

			Unloaded -= Slider_Unloaded;
		}

		#region Manipulation

		#region Touch

		private void Slider_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
		{
			var dc = DataContext as DischargeBedViewModel;
			if (dc == null) throw new InvalidOperationException("DataContext is invalid");

			if (IsDragEnabled)
			{

				var pt = e.ManipulationOrigin;

				var oneHourWidth = ActualWidth/16;
			 
				var depth = pt.X;

				dc.StartTouchManipulation(depth, pt.X, oneHourWidth);
			}
		}

		private void Slider_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
		{
			var dc = DataContext as DischargeBedViewModel;
			if (dc == null) throw new InvalidOperationException("DataContext is invalid");

			if (IsDragEnabled)
			{
				var oneHourWidth = ActualWidth/16;
				dc.UpdateTouchManipulation(e.CumulativeManipulation.Translation.X, oneHourWidth);
			}
		}

		private void Slider_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
		{
			if (IsDragEnabled)
			{
				var dc = DataContext as DischargeBedViewModel;
				if (dc == null) throw new InvalidOperationException("DataContext is invalid");

				dc.ConfirmTouchManipulation();

				CommandManager.InvalidateRequerySuggested();
			}
		}
		#endregion

		#region Mouse

		private void Slider_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			//Debug.WriteLine("Click");
			if (IsDragEnabled)
			{
				var pt = e.GetPosition((UIElement)sender);
				//var ov = WpfHelper.TryFindChild<WcsOrderBorder>(this);

				//if (ov.IsMouseOver)
				//{
				var dc = DataContext as DischargeBedViewModel;
				if (dc == null) throw new InvalidOperationException("DataContext is invalid");


				var oneHourWidth = ActualWidth/16;

				var depth = pt.X;
				dc.StartMouseManipulation(depth, pt.X, oneHourWidth);
				//}
			}
		}

		private void Slider_MouseMove(object sender, MouseEventArgs e)
		{
		//	Debug.WriteLine("Move");
			if (IsDragEnabled)
			{
				if (e.LeftButton == MouseButtonState.Pressed)
				{
					var dc = DataContext as DischargeBedViewModel;
					if (dc == null) throw new InvalidOperationException("DataContext is invalid");

					if (dc.IsMouseManipulation)
					{
						var oneHourWidth = ActualWidth / 16;

						Point pt = e.GetPosition(this);
						dc.UpdateMouseManipulation(pt.X, oneHourWidth);
					}

				}
			}
		}

		private void Slider_MouseLeave(object sender, MouseEventArgs e)
		{
		//	Debug.WriteLine("Leave");
			var dc = DataContext as DischargeBedViewModel;
			if (dc == null) return;

			if (dc.HasMouseManipulationMoved)
			{
				dc.AbortMouseManipulation();
			}
		}

		private void Slider_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (IsDragEnabled)
			{
				var dc = DataContext as DischargeBedViewModel;
				if (dc == null) throw new InvalidOperationException("DataContext is invalid");

				if (dc.HasMouseManipulationMoved)
				{
					dc.ConfirmMouseManipulation();
				}
			}
		}
		#endregion

		#endregion


		//void UpdateTimeline_Tick(object sender, EventArgs e)
		//{
		//    InvalidateVisual();
		//}

	}
}

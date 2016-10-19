using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using WCS.Shared.Schedule;

namespace WCS.Shared.Orders
{
	public partial class OrderItemsView : UserControl
	{
		private DispatcherTimer _timer;

		public OrderItemsView()
		{
			InitializeComponent();

			Unloaded += DepartmentAppointment_Unloaded;

			ManipulationStarted += ScheduleDragElementControl_ManipulationStarted;
			ManipulationDelta += ScheduleDragElementControl_ManipulationDelta;
			ManipulationCompleted += DepartmentAppointment_ManipulationCompleted;

			MouseMove += ScheduleDragElementControl_MouseMove;
			MouseLeftButtonDown += ScheduleDragElementControl_MouseLeftButtonDown;
			MouseLeftButtonUp += DepartmentAppointment_MouseLeftButtonUp;
			MouseLeave += DepartmentAppointment_MouseLeave;

			_timer = new DispatcherTimer();

			_timer.Tick += UpdateTimeline_Tick;
			_timer.Interval = TimeSpan.FromSeconds(60);
			_timer.Start();
		}

		public bool IsDragEnabled
		{
			get { return (bool)GetValue(IsDragEnabledProperty); }
			set { SetValue(IsDragEnabledProperty, value); }
		}

		public static readonly DependencyProperty IsDragEnabledProperty =
			DependencyProperty.Register("IsDragEnabled", typeof(bool), typeof(OrderItemsView), new UIPropertyMetadata(true));
		 
		void DepartmentAppointment_Unloaded(object sender, RoutedEventArgs e)
		{
			ManipulationStarted -= ScheduleDragElementControl_ManipulationStarted;
			ManipulationDelta -= ScheduleDragElementControl_ManipulationDelta;
			ManipulationCompleted -= DepartmentAppointment_ManipulationCompleted;

			MouseMove -= ScheduleDragElementControl_MouseMove;
			MouseLeftButtonDown -= ScheduleDragElementControl_MouseLeftButtonDown;
			MouseLeftButtonUp -= DepartmentAppointment_MouseLeftButtonUp;
			MouseLeave += DepartmentAppointment_MouseLeave;

			_timer.Tick -= UpdateTimeline_Tick;

			Unloaded -= DepartmentAppointment_Unloaded;
		}

		#region Manipulation

		#region Touch
		private void ScheduleDragElementControl_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
		{
			var dc = DataContext as OrderViewModel;
			if (dc == null) throw new InvalidOperationException("DataContext is invalid");

			if (IsDragEnabled)
			{

				var pt = e.ManipulationOrigin;

				var oneHourWidth = ScheduleBackgroundBase.GetOneHourWidth(ActualWidth, true);
				var startOffset = dc.OrderCoordinator.Order.StartTime.HasValue ? (oneHourWidth * dc.OrderCoordinator.Order.StartTime.Value.TotalHours - 4) : 0;

				var depth = pt.X - startOffset;

				dc.StartTouchManipulation(depth, pt.X, oneHourWidth);
			}
		}

		private void ScheduleDragElementControl_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
		{
			var dc = DataContext as OrderViewModel;
			if (dc == null) throw new InvalidOperationException("DataContext is invalid");

			if (IsDragEnabled)
			{
				var oneHourWidth = ScheduleBackgroundBase.GetOneHourWidth(ActualWidth, true);
				dc.UpdateTouchManipulation(e.CumulativeManipulation.Translation.X, oneHourWidth);
			}
		}

		private void DepartmentAppointment_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
		{
			if (IsDragEnabled)
			{
				var dc = DataContext as OrderViewModel;
				if (dc == null) throw new InvalidOperationException("DataContext is invalid");

				dc.ConfirmTouchManipulation();
				
				CommandManager.InvalidateRequerySuggested();
			}
		} 
		#endregion

		#region Mouse

		private void ScheduleDragElementControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (IsDragEnabled)
			{
				var pt = e.GetPosition((UIElement) sender);
				var ov = WpfHelper.TryFindChild<WcsOrderBorder>(this);

				//if (ov.IsMouseOver)
				//{
					var dc = DataContext as OrderViewModel;
					if (dc == null) throw new InvalidOperationException("DataContext is invalid");


					var oneHourWidth = ScheduleBackgroundBase.GetOneHourWidth(ActualWidth, true);
					var startOffset = dc.OrderCoordinator.Order.StartTime.HasValue?(oneHourWidth * dc.OrderCoordinator.Order.StartTime.Value.TotalHours-4):0;

					var depth = pt.X - startOffset;
					dc.StartMouseManipulation(depth, pt.X, oneHourWidth);
				//}
			}
		}

		private void ScheduleDragElementControl_MouseMove(object sender, MouseEventArgs e)
		{
			if (IsDragEnabled)
			{
				if (e.LeftButton == MouseButtonState.Pressed)
				{
					var dc = DataContext as OrderViewModel;
					if (dc == null) throw new InvalidOperationException("DataContext is invalid");
					 
					if (dc.IsMouseManipulation)
					{
						var oneHourWidth = ScheduleBackgroundBase.GetOneHourWidth(ActualWidth, true);
						
						Point pt = e.GetPosition(this);
						dc.UpdateMouseManipulation(pt.X, oneHourWidth);
					}

				}
			}
		}

		private void DepartmentAppointment_MouseLeave(object sender, MouseEventArgs e)
		{
			var dc = DataContext as OrderViewModel;
			if (dc == null) return;

			if (dc.HasMouseManipulationMoved)
			{
				dc.AbortMouseManipulation();
			}
		}

		private void DepartmentAppointment_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (IsDragEnabled)
			{
				var dc = DataContext as OrderViewModel;
				if (dc == null) throw new InvalidOperationException("DataContext is invalid");

				if (dc.HasMouseManipulationMoved)
				{
					dc.ConfirmMouseManipulation();
				}
			} 
		}  
		#endregion

		#endregion


		void UpdateTimeline_Tick(object sender, EventArgs e)
		{
			InvalidateVisual();
		}
		 
	}
}

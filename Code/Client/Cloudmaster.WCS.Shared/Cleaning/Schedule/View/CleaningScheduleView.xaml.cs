using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WCS.Shared.Beds;

namespace WCS.Shared.Cleaning.Schedule
{
    public partial class CleaningScheduleView : UserControl
    {
		public CleaningScheduleView()
        {
            InitializeComponent();
		}

		private void UserControl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			var dp = e.MouseDevice.DirectlyOver as DependencyObject;
			var order = dp.TryFindParent<BedView>();
			if (order == null)
			{
				ClearSelection();
			}
		}


		private void UserControl_PreviewTouchDown(object sender, TouchEventArgs e)
		{
			var dp = e.TouchDevice.DirectlyOver as DependencyObject;
			var order = dp.TryFindParent<BedView>();
			if (order == null)
			{
				ClearSelection();
			}
		}


		private void ClearSelection()
		{
			var order = DataContext as CleaningScheduleViewModel;
			if (order != null)
				order.CancelSelectionCommand.Execute(null);
		}

		private void lstAppointments_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
		{
			CommandManager.InvalidateRequerySuggested();
		}

		private void HandleGetFocus(object sender, RoutedEventArgs e)
		{
			var dc = DataContext as CleaningScheduleViewModel;
			if (dc == null)
				return;

			//	dc.HideTimelineNotes();
		}
    }
}

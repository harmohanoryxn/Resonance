using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WCS.Shared.Controls;

namespace WCS.Shared.Physio.Schedule
{
    public partial class PhysioScheduleView : UserControl
    {
		public PhysioScheduleView()
        {
            InitializeComponent();
        }

		private void UserControl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			var dp = e.MouseDevice.DirectlyOver as DependencyObject;
			var order = dp.TryFindParent<PhysioOrderView>();
			if (order==null)
			{
				ClearSelection();
			}
		}


    	private void UserControl_PreviewTouchDown(object sender, TouchEventArgs e)
		{
			var dp = e.TouchDevice.DirectlyOver as DependencyObject;
			var order = dp.TryFindParent<PhysioOrderView>();
			if (order == null)
			{
				ClearSelection();
			}
		}


		private void ClearSelection()
		{
			var order = DataContext as PhysioScheduleViewModel;
			if (order != null)
				order.CancelSelectionCommand.Execute(null);
		} 

		//private void HandleGetFocus(object sender, RoutedEventArgs e)
		//{
		//    var dc = DataContext as PhysioScheduleViewModel;
		//    if (dc == null)
		//        return;

		////		dc.Tombstone();
		//}
		//private void HandleTimelinePopupClosed(object sender, System.EventArgs e)
		//{
		//    var dc = DataContext as PhysioScheduleViewModel;
		//    if (dc == null) return;

		//    dc.ScheduleItems.ForEach(si => si.HideAllNotes());
		//}
    }
}

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WCS.Shared.Controls;

namespace WCS.Shared.Ward.Schedule
{
    public partial class WardScheduleView : UserControl
    {
		public WardScheduleView()
        {
            InitializeComponent();
		}

		private void UserControl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			var dp = e.MouseDevice.DirectlyOver as DependencyObject;
			var order = dp.TryFindParent<WardOrderView>();
			if (order == null)
			{
				ClearSelection();
			}
		}


		private void UserControl_PreviewTouchDown(object sender, TouchEventArgs e)
		{
			var dp = e.TouchDevice.DirectlyOver as DependencyObject;
			var order = dp.TryFindParent<WardOrderView>();
			if (order == null)
			{
				ClearSelection();
			}
		}


		private void ClearSelection()
		{
			var order = DataContext as WardScheduleViewModel;
			if (order != null)
				order.CancelSelectionCommand.Execute(null);
		} 
    }
}

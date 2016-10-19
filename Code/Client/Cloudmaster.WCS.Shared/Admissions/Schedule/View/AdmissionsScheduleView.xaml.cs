using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WCS.Shared.Beds;

namespace WCS.Shared.Admissions.Schedule
{
	public partial class AdmissionsScheduleView : UserControl
    {
		public AdmissionsScheduleView()
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
			var order = DataContext as AdmissionsScheduleViewModel;
			if (order != null)
				order.CancelSelectionCommand.Execute(null);
		} 
		 
    }
}

using System.Windows.Controls;

namespace WCS.Shared.Alerts
{
    public partial class AlertsView : UserControl
    {
        public AlertsView()
        {
            InitializeComponent();
        }

		private void UserControl_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			Dismiss();
		}

		private void UserControl_PreviewTouchDown(object sender, System.Windows.Input.TouchEventArgs e)
		{
			Dismiss();
		}

		private void Dismiss()
		{
			var dc = DataContext as AlertViewModel;
			if (dc == null) return;
			dc.DismissCommand.Execute(null);

		}
    }
}

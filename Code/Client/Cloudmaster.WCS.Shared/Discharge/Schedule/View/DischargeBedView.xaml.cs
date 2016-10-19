using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using WCS.Shared.Discharge.Schedule;

namespace WCS.Shared.Beds
{
	public partial class DischargeBedView : UserControl
	{
		public DischargeBedView()
		{
			InitializeComponent();
		}
		
		private void uc_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			HighlightBed();
		}

		private void uc_PreviewTouchDown(object sender, System.Windows.Input.TouchEventArgs e)
		{
			HighlightBed();
		}

		private void HighlightBed()
		{
			var bed = DataContext as DischargeBedViewModel;
			if (bed != null)
				bed.TrySelectCommand.Execute(null);
		}
	}
}

using System.Windows.Controls;
using System.Windows.Input;

namespace WCS.Shared.Schedule
{
	public partial class GenericWcsScreenView : UserControl
	{
		public GenericWcsScreenView()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Handles the MouseMove event that is responsible for extending the auto security lock
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
		private void HandleMouseMove(object sender, MouseEventArgs e)
		{
			InvalidateActivity();
		}

		private void HandlePreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			InvalidateActivity();
		}

		private void HandlePreviewTouchDown(object sender, TouchEventArgs e)
		{
			InvalidateActivity();
		}

		private void InvalidateActivity()
		{
			var dc = DataContext as WcsViewModel;
			if (dc != null)
			{
				dc.InvalidateInactivity();
				dc.AlertViewModel.DismissCommand.Execute(null);
			}
		}

	 
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WCS.Shared.Controls;
using WCS.Shared.Location;
using WCS.Shared.Notes;
using WCS.Shared.Schedule;

namespace WCS.Shared.Discharge.Schedule.View
{
	public partial class DischargeCombinedScheduleView : UserControl
	{
		public DischargeCombinedScheduleView()
		{
			InitializeComponent();
		} 

		/// <summary>
		/// THis function is used to swap between list box scrolling and moving orders on the screen, depending where the user touches the screen
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Input.TouchEventArgs"/> instance containing the event data.</param>
		private void roomsContainer_PreviewTouchDown(object sender, TouchEventArgs e)
		{
			var position = e.GetTouchPoint(this);
			if (position.Position.X < 240)
				ScrollViewer.SetPanningMode(listRooms, PanningMode.VerticalOnly);
			else
				ScrollViewer.SetPanningMode(listRooms, PanningMode.None);

		}
	}
}

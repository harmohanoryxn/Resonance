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

namespace WCS.Shared.Department.Schedule
{
	public partial class DepartmentScheduleView : UserControl
	{
		public DepartmentScheduleView()
		{
			InitializeComponent();
		}



		private void UserControl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			var dp = e.MouseDevice.DirectlyOver as DependencyObject;
			var order = dp.TryFindParent<DepartmentOrderView>();
			if (order == null)
			{
				ClearSelection();
			}
		}


		private void UserControl_PreviewTouchDown(object sender, TouchEventArgs e)
		{
			var dp = e.TouchDevice.DirectlyOver as DependencyObject;
			var order = dp.TryFindParent<DepartmentOrderView>();
			if (order == null)
			{
				ClearSelection();
			}
		}


		private void ClearSelection()
		{
			var order = DataContext as DepartmentScheduleViewModel;
			if (order != null)
				order.CancelSelectionCommand.Execute(null);
		}

		/// <summary>
		/// THis function is used to swap between list box scrolling and moving orders on the screen, depending where the user touches the screen
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Input.TouchEventArgs"/> instance containing the event data.</param>
		private void patientsContainer_PreviewTouchDown(object sender, TouchEventArgs e)
		{
			var position = e.GetTouchPoint(this);
			if (position.Position.X < 240)
				ScrollViewer.SetPanningMode(patientsList, PanningMode.VerticalOnly);
			else
				ScrollViewer.SetPanningMode(patientsList, PanningMode.None);

		}
		 
	}
}

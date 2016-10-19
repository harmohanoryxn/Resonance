using System;
using Cloudmaster.WCS.Controls.Browser;
using Cloudmaster.WCS.Controls.ObjectEditor;
using Cloudmaster.WCS.Controls.Scheduling;
using Cloudmaster.WCS.Department.Processing;
using Cloudmaster.WCS.Model;
using System.Windows;
using Cloudmaster.WCS.Classes;
using GalaSoft.MvvmLight.Command;

namespace Cloudmaster.WCS.Department.Model
{
	/// <summary>
	/// Department DataContext
	/// </summary>
	public class DepartmentViewModel : ModelBase, IBrowser
	{
		private ScheduleViewModel _scheduleViewModel;
		private AlertViewModel _alertViewModel;

		private DepartmentProcessor _processor;

		public DepartmentViewModel()
		{
			_instance = this;

			NavigationViewModel = new NavigationViewModel();
		//	Labels = new Labels();
			SecurityViewModel = new DepartmentSecurityViewModel();
			ScheduleViewModel = new ScheduleViewModel();
			AlertViewModel = new AlertViewModel(ScheduleViewModel);

			Coordinator = new BrowserItemCoordinator();
			Coordinator.BrowserViewModel = this;
			Coordinator.AddNewItem(ScheduleViewModel);
		

			int securityLockInterval = Convert.ToInt32(Application.Current.Properties["SecurityLockInterval"]);
 
			SecurityViewModel.Initialize(securityLockInterval);

			_processor = new DepartmentProcessor();
			_processor.NewDataAvailable += HandleNewDataFromTheServer;
		}


		#region IBrowser Members
 
		public BrowserItemCoordinator Coordinator { get; set; }

		#endregion

		#region Refactor this away
		private static DepartmentViewModel _instance;
		public static DepartmentViewModel Instance
		{
			get { return _instance; }
		} 
		#endregion
		 
		public ScheduleViewModel ScheduleViewModel
		{
			get { return _scheduleViewModel; }
			set
			{
				_scheduleViewModel = value;
				_scheduleViewModel.Department = this;
			}
		}

		// TODO Uncomment this and make it work
		//public new AppointmentViewModel SelectedOrder
		//{
		//    get { return ScheduleViewModel.SelectedAppointment; }
		//    set{ScheduleViewModel.SelectedAppointment = value;}
		//}

		public AlertViewModel AlertViewModel
		{
			get { return _alertViewModel; }
			set
			{
				_alertViewModel = value;
				this.DoRaisePropertyChanged(() => AlertViewModel, RaisePropertyChanged);
			}
		}

		/// <summary>
		/// Handles the event when the processor has new available for UI consumption
		/// </summary>
		/// <param name="results">The results.</param>
		private void HandleNewDataFromTheServer(ServerInformation results)
		{
			// firstly merge the new feed data into the current orders
			ScheduleViewModel.MergeResults(results);

			// now update visual items to reflect those changes
			// requies that the schedule has already been synce-ed
			AlertViewModel.Synchronise();
		}

		/// <summary>
		/// Opens an appointment into it's own slide-y out panel
		/// </summary>
		/// <param name="appointment">The appointment</param>
		public void ExplodeAppointment(AppointmentViewModel appointment)
		{
			if(appointment == null)
				throw new ArgumentException("Missing", "appointment");

			Coordinator.AddNewItem(new ExplodeAppointmentViewModel(appointment));
		}
	}
}

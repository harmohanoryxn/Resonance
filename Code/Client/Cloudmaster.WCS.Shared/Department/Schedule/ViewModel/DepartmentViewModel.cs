using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using Cloudmaster.WCS.DataServicesInvoker;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using WCS.Core;
using WCS.Core.Composition;
using WCS.Shared.Orders;
using WCS.Shared.Schedule;

namespace WCS.Shared.Department.Schedule
{
	/// <summary>
	/// Top level Department DataContext
	/// </summary>
	public sealed class DepartmentViewModel : WcsViewModel
	{
		private ManualResetEventSlim _isManipulationSync;

		private PresenceRecurringPoller _presencePoller;
		private OrdersRecurringPoller _orderPoller;
		private DetectionRecurringPoller _detectionsPoller;
		private IObservable<IList<Presence>> _presenceSource;
		private IObservable<IList<Order>> _orderSource;
		private IObservable<IList<Detection>> _detectionsSource;
 
		public DepartmentViewModel(bool isLocked, string version, string clientName, string serverName)
			: base(isLocked, version, clientName, serverName)
		{
			ExceptionHandler = MefContainer.GetExportedValue<IWcsExceptionHandler>();
			if (ExceptionHandler != null)
			{
				ExceptionHandler.ExceptionArrived += HandleException;
				ExceptionHandler.ServerStatusUpdate += HandleException;
			}

			ScheduleViewModel = new DepartmentScheduleViewModel();
			//ScheduleViewModel = new DepartmentScheduleViewModel();
			ScheduleViewModel.StartingManipulation += HandleStartingManipulation;
			ScheduleViewModel.EndedManipulation += HandleEndedManipulation;
			ScheduleViewModel.SynchronisationComplete += ScheduleViewModel_SynchronisationComplete;
			ScheduleViewModel.RequestOrdersFromDate += ScheduleViewModel_RequestOrdersFromDate;

			AlertViewModel.Schedule = ScheduleViewModel;

			_isManipulationSync = new ManualResetEventSlim(true);

		} 

		private DepartmentScheduleViewModel _scheduleViewModel;

		public   DepartmentScheduleViewModel ScheduleViewModel
		{
			get { return _scheduleViewModel; }
			set
			{
				_scheduleViewModel = value;
				_scheduleViewModel.Main = this;
			}
		}

		/// <summary>
		/// Handles the event when the processor has new available for UI consumption
		/// </summary>
		/// <param name="orders">The results.</param>
		protected override void HandleNewOrders(IList<Order> orders)
		{
			// This has to be done on a background thread or there is a deadlock possibility
			System.Threading.Tasks.Task.Factory.StartNew(() =>
			{
				_isManipulationSync.Wait(4000);

				ScheduleViewModel.Synchronise(orders, _detectionsPoller.SetPatients);

			}).LogExceptionIfThrownAndIgnore();
		}

		/// <summary>
		/// Handles the event when the processor has new available for UI consumption
		/// </summary>
		/// <param name="detections">The results.</param>
		protected override void HandleNewDetections(IList<Detection> detections)
		{
			// This has to be done on a background thread or there is a deadlock possibility
			System.Threading.Tasks.Task.Factory.StartNew(() =>
			{
				ScheduleViewModel.Synchronise(detections);

			}).LogExceptionIfThrownAndIgnore();
		}

		/// <summary>
		/// Handles the an updated version of the different ward status update from the server
		/// </summary>
		/// <param name="presences">The ward statuses.</param>
		protected override void HandleNewPresences(IList<Presence> presences)
		{
			// This has to be done on a background thread or there is a deadlock possibility
			System.Threading.Tasks.Task.Factory.StartNew(() =>
			{
				_isManipulationSync.Wait(4000);

				ScheduleViewModel.Synchronise(presences);

			}).LogExceptionIfThrownAndIgnore();
		}

		protected override void HandleTombstone()
		{
			ScheduleViewModel.Tombstone();
		}

		protected override void HandleNewTimeouts(PollingTimeouts timeouts)
		{

			// This has to be done on a background thread or there is a deadlock possibility
			System.Threading.Tasks.Task.Factory.StartNew(() =>
			{
				_presencePoller.SetTimeout(timeouts.PresenceTimeout);
				_orderPoller.SetTimeout(timeouts.OrderTimeout);
				_detectionsPoller.SetTimeout(timeouts.RfidTimeout);

				ScheduleViewModel.IsRfidEnabled = timeouts.RfidTimeout != 0;
			}).LogExceptionIfThrownAndIgnore();
		}

		/// <summary>
		/// Handles the event if there any been no activity from the user in a minute
		/// </summary>
		protected override void HandleMinuteOfInActivity()
		{
			ScheduleViewModel.HidePanels();
			ScheduleViewModel.ResetDefaultWard();
			ScheduleViewModel.ResetDefaultOrderStatus();
		}

		protected override void ResetInActivity()
		{
		}

		/// <summary>
		/// Shows Action Bar and hides all other Panel
		/// </summary>
		public override void ClearAllSelections()
		{
			ScheduleViewModel.HidePanelsAndDeselectCommand.Execute(null);
		}

		public void InitialiseConfiguration(DeviceConfigurationInstance configuration, PollingTimeouts timeouts)
		{
			var defaultLocationCode = !string.IsNullOrEmpty(configuration.LocationCode) ? configuration.LocationCode : configuration.VisibleLocations.First().Code;
			var defaultLocationLocation = !string.IsNullOrEmpty(configuration.LocationCode) ? configuration.LocationName : configuration.VisibleLocations.First().Name;

			var defaultDepartmentCode = !string.IsNullOrEmpty(configuration.LocationCode) ? configuration.LocationCode : configuration.VisibleLocations.First().Code;

			var locs = new SortedSet<LocationSummaryViewModel>();

			configuration.VisibleLocations.Distinct().ToList().ForEach(department =>
			{
				var deptvm = new LocationSummaryViewModel(department, defaultDepartmentCode == department.Code);
				deptvm.RequiresSelection += ScheduleViewModel.HandleNewWardSelection;
				locs.Add(deptvm);
			});


			var defaultLocation = locs.FirstOrDefault(w => String.CompareOrdinal(w.Code, defaultLocationCode) == 0);
			if (defaultLocation == null)
				defaultLocation = locs.FirstOrDefault();

			if (defaultLocation == null)
				throw new InvalidOperationException("There is no default location for current configuration " + configuration.LocationName);

		    defaultLocation.WaitingRoomCode = configuration.WaitingRoomLocationCode;

			ScheduleViewModel.AlternativeLocations = new ObservableCollection<LocationSummaryViewModel>(locs);
			ScheduleViewModel.DefaultLocation = defaultLocation;
			ScheduleViewModel.SelectedLocation = ScheduleViewModel.DefaultLocation;

			var invoker = MefContainer.GetExportedValue<IWcsAsyncInvoker>();
			if (invoker != null)
			{
				_orderPoller = new OrdersRecurringPoller(invoker, DateTime.Now, configuration.VisibleLocations.Select(l=>l.Code).ToList(), timeouts.OrderTimeout);
				_orderSource = Observable.FromEvent<IList<Order>>(ev => _orderPoller.OrdersUpdateAvailable += ev, ev => _orderPoller.OrdersUpdateAvailable -= ev);
				_orderSource.Subscribe(HandleNewOrders);

				_presencePoller = new PresenceRecurringPoller(invoker, timeouts.PresenceTimeout);
				_presenceSource = Observable.FromEvent<IList<Presence>>(ev => _presencePoller.LocationConnectionUpdateAvailable += ev, ev => _presencePoller.LocationConnectionUpdateAvailable -= ev);
				_presenceSource.Subscribe(HandleNewPresences);

				_detectionsPoller = new DetectionRecurringPoller(invoker, timeouts.RfidTimeout);
				_detectionsSource = Observable.FromEvent<IList<Detection>>(ev => _detectionsPoller.DetectionsUpdateAvailable += ev, ev => _detectionsPoller.DetectionsUpdateAvailable -= ev);
				_detectionsSource.Subscribe(HandleNewDetections);
			}
		}

		/// <summary>
		/// Handles the event fired when the schedule is finished synchronising. Used to display any new Alerts
		/// </summary>
		private void ScheduleViewModel_SynchronisationComplete()
		{
			// now update visual items to reflect those changes
			// requires that the schedule has already been synce-ed
			AlertViewModel.Update();

			_presencePoller.HitServerForData();
		}

		void ScheduleViewModel_RequestOrdersFromDate(DateTime date)
		{
			_orderPoller.SetDate(date);
		}

		private void HandleEndedManipulation(OrderViewModel order)
		{
			_isManipulationSync.Set();
		}

		private void HandleStartingManipulation(OrderViewModel order)
		{
			_isManipulationSync.Reset();
		}
	}
}

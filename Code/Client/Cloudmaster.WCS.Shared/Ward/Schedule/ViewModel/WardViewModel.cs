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
using WCS.Shared.Browser;
using WCS.Shared.Schedule;

namespace WCS.Shared.Ward.Schedule
{
	/// <summary>
	/// Top level Ward DataContext
	/// </summary>
	public sealed class WardViewModel : WcsViewModel
	{
		private PresenceRecurringPoller _presencePoller;
		private OrdersRecurringPoller _orderPoller;
		private DetectionRecurringPoller _detectionsPoller;
		private IObservable<IList<Presence>> _presenceSource;
		private IObservable<IList<Order>> _orderSource;
		private IObservable<IList<Detection>> _detectionsSource;

		private WardScheduleViewModel _scheduleViewModel;

		public WardViewModel(bool isLocked, string version, string clientName, string serverName)
			: base(isLocked, version, clientName, serverName)
		{

			ScheduleViewModel = new WardScheduleViewModel();
			ScheduleViewModel.SynchronisationComplete += ScheduleViewModel_SynchronisationComplete;
			ScheduleViewModel.RequestOrdersFromDate += ScheduleViewModel_RequestOrdersFromDate;

			AlertViewModel.Schedule = ScheduleViewModel;
		}

		public WardScheduleViewModel ScheduleViewModel
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
				// This then is called back unto the main thread for UI update
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
				ScheduleViewModel.Synchronise(presences);

			}).LogExceptionIfThrownAndIgnore();
		}

		protected override void HandleTombstone()
		{
			ScheduleViewModel.Tombstone();
		}

		/// <summary>
		/// Handles the event if there any been no activity from the user in a minute
		/// </summary>
		protected override void HandleMinuteOfInActivity()
		{
			ScheduleViewModel.HidePanels();
			ScheduleViewModel.ResetDefaultWard();
			ScheduleViewModel.ResetDefaultSort();
			ScheduleViewModel.ResetDefaultOrderStatus();
		}

		/// <summary>
		/// Shows Action Bar and hides all other Panel
		/// </summary>
		public override void ClearAllSelections()
		{
			ScheduleViewModel.HidePanelsAndDeselectCommand.Execute(null);
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

		public void InitialiseConfiguration(DeviceConfigurationInstance configuration, PollingTimeouts timeouts)
		{
			var defaultLocationCode = !string.IsNullOrEmpty(configuration.LocationCode) ? configuration.LocationCode : configuration.VisibleLocations.First().Code;
			var defaultLocationLocation = !string.IsNullOrEmpty(configuration.LocationCode) ? configuration.LocationName : configuration.VisibleLocations.First().Name;

			ScheduleViewModel.WardTitle = defaultLocationLocation;

			var invoker = MefContainer.GetExportedValue<IWcsAsyncInvoker>();
			if (invoker != null)
			{
				_orderPoller = new OrdersRecurringPoller(invoker, DateTime.Now, configuration.VisibleLocations.Select(l => l.Code).ToList(), timeouts.OrderTimeout);
				_orderSource = Observable.FromEvent<IList<Order>>(ev => _orderPoller.OrdersUpdateAvailable += ev, ev => _orderPoller.OrdersUpdateAvailable -= ev);
				_orderSource.Subscribe(HandleNewOrders);

				_presencePoller = new PresenceRecurringPoller(invoker, timeouts.PresenceTimeout);
				_presenceSource = Observable.FromEvent<IList<Presence>>(ev => _presencePoller.LocationConnectionUpdateAvailable += ev, ev => _presencePoller.LocationConnectionUpdateAvailable -= ev);
				_presenceSource.Subscribe(HandleNewPresences);

				_detectionsPoller = new DetectionRecurringPoller(invoker, timeouts.RfidTimeout);
				_detectionsSource = Observable.FromEvent<IList<Detection>>(ev => _detectionsPoller.DetectionsUpdateAvailable += ev, ev => _detectionsPoller.DetectionsUpdateAvailable -= ev);
				_detectionsSource.Subscribe(HandleNewDetections);
			}


			var locs = new SortedSet<LocationSummaryViewModel>();

			configuration.VisibleLocations.Distinct().ToList().ForEach(location =>
			{
				var deptvm = new LocationSummaryViewModel(location, defaultLocationCode == location.Code);
				deptvm.RequiresSelection += ScheduleViewModel.HandleNewWardSelection;
				locs.Add(deptvm);
			});

			var defaultLocation = locs.FirstOrDefault(w => String.CompareOrdinal(w.Code, defaultLocationCode) == 0);
			if (defaultLocation == null)
				defaultLocation = locs.FirstOrDefault();

			if (defaultLocation==null)
				throw new InvalidOperationException("There is no default location for current configuration " + configuration.LocationName);

			ScheduleViewModel.AlternativeLocations = new ObservableCollection<LocationSummaryViewModel>(locs);
			ScheduleViewModel.DefaultLocation = defaultLocation;
			ScheduleViewModel.SelectedLocation = ScheduleViewModel.DefaultLocation;
		}

		/// <summary>
		/// Handles the event fired when the schedule is finished synchronising. Used to display any new Alerts
		/// </summary>
		private void ScheduleViewModel_SynchronisationComplete()
		{
			// now update visual items to reflect those changes
			// requires that the schedule has already been synce-ed
			AlertViewModel.Update();
		}

		void ScheduleViewModel_RequestOrdersFromDate(DateTime date)
		{
			_orderPoller.SetDate(date);
		}

		protected override void ResetInActivity()
		{
		}
	}
}

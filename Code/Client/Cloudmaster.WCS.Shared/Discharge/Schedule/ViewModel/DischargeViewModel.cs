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
using WCS.Shared.Schedule;

namespace WCS.Shared.Discharge.Schedule
{
	/// <summary>
	/// Top level Discharge DataContext
	/// </summary>
	public sealed class DischargeViewModel : WcsViewModel
	{
		private ManualResetEventSlim _isManipulationSync;
		
		private PresenceRecurringPoller _presencePoller;
		private DischargeBedsRecurringPoller _dischargePoller;
		private IObservable<IList<Presence>> _presenceSource;
		private IObservable<IList<BedDischargeData>> _dischargeSource;

		public DischargeViewModel(bool isLocked, string version, string clientName, string serverName)
			: base(isLocked, version, clientName, serverName)
		{

			ExceptionHandler = WcsViewModel.MefContainer.GetExportedValue<IWcsExceptionHandler>();
			if (ExceptionHandler != null)
			{
				ExceptionHandler.ExceptionArrived += HandleException;
				ExceptionHandler.ServerStatusUpdate += HandleException;
			}

			ScheduleViewModel = new DischargeScheduleViewModel();
			ScheduleViewModel.SynchronisationComplete += ScheduleViewModel_SynchronisationComplete;
			ScheduleViewModel.StartingManipulation += HandleStartingManipulation;
			ScheduleViewModel.EndedManipulation += HandleEndedManipulation;
			
			AlertViewModel.Schedule = ScheduleViewModel;

			_isManipulationSync = new ManualResetEventSlim(true);
		}

		private DischargeScheduleViewModel _scheduleViewModel;
		public  DischargeScheduleViewModel ScheduleViewModel
		{
			get { return _scheduleViewModel; }
			set
			{
				_scheduleViewModel = value;
				_scheduleViewModel.Main = this;
			}
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
				_dischargePoller.SetTimeout(timeouts.DischargeTimeout);
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

		/// <summary>
		/// Handles the an updated or new discharge data for beds of the server
		/// </summary>
		/// <param name="discharges">The discharge information for beds</param>
		protected override void HandleNewDischargeBedData(IList<BedDischargeData> discharges)
		{
			// This has to be done on a background thread or there is a deadlock possibility
			System.Threading.Tasks.Task.Factory.StartNew(() =>
			{
				_isManipulationSync.Wait(4000);
				
				ScheduleViewModel.Synchronise(discharges);

			}).LogExceptionIfThrownAndIgnore();
		}
		

		public void InitialiseConfiguration(DeviceConfigurationInstance configuration, PollingTimeouts timeouts)
		{
			var defaultLocationCode = !string.IsNullOrEmpty(configuration.LocationCode) ? configuration.LocationCode : configuration.VisibleLocations.First().Code;
			var defaultLocationLocation = !string.IsNullOrEmpty(configuration.LocationCode) ? configuration.LocationName : configuration.VisibleLocations.First().Name;

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

			if (defaultLocation == null)
				throw new InvalidOperationException("There is no default location for current configuration " + configuration.LocationName);

			ScheduleViewModel.AlternativeLocations = new ObservableCollection<LocationSummaryViewModel>(locs);
			ScheduleViewModel.DefaultLocation = defaultLocation;
			ScheduleViewModel.SelectedLocation = ScheduleViewModel.DefaultLocation;
			ScheduleViewModel.WardTitle = defaultLocationLocation;
			ScheduleViewModel.WardCode = defaultLocationCode;

			var invoker = MefContainer.GetExportedValue<IWcsAsyncInvoker>();
			if (invoker != null)
			{
				_presencePoller = new PresenceRecurringPoller(invoker, timeouts.PresenceTimeout);
				_presenceSource = Observable.FromEvent<IList<Presence>>(ev => _presencePoller.LocationConnectionUpdateAvailable += ev, ev => _presencePoller.LocationConnectionUpdateAvailable -= ev);
				_presenceSource.Subscribe(HandleNewPresences);

				_dischargePoller = new DischargeBedsRecurringPoller(invoker, configuration.VisibleLocations.Select(l => l.Code).ToList(), timeouts.CleaningBedDataTimeout);
				_dischargeSource = Observable.FromEvent<IList<BedDischargeData>>(ev => _dischargePoller.DischargeBedDataUpdateAvailable += ev, ev => _dischargePoller.DischargeBedDataUpdateAvailable -= ev);
				_dischargeSource.Subscribe(HandleNewDischargeBedData);
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
		}

		private void HandleEndedManipulation(DischargeBedViewModel bed)
		{
			_isManipulationSync.Set();
		}

		private void HandleStartingManipulation(DischargeBedViewModel bed)
		{
			_isManipulationSync.Reset();
		}
	}
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using Cloudmaster.WCS.DataServicesInvoker;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using WCS.Core;
using WCS.Core.Composition;
using WCS.Shared.Schedule;

namespace WCS.Shared.Admissions.Schedule
{
	/// <summary>
	/// Top level Admissions DataContext
	/// </summary>
	public sealed class AdmissionsViewModel : WcsViewModel
	{
		private PresenceRecurringPoller _presencePoller;
		private AdmissionsDataRecurringPoller _admissionPoller;
		private DetectionRecurringPoller _detectionsPoller;
		private IObservable<IList<Presence>> _presenceSource;
		private IObservable<AdmissionsData> _admissionSource;
		private IObservable<IList<Detection>> _detectionsSource;

		public AdmissionsViewModel(bool isLocked, string version, string clientName, string serverName)
			: base(isLocked, version, clientName, serverName)
		{

			ExceptionHandler = WcsViewModel.MefContainer.GetExportedValue<IWcsExceptionHandler>();
			if (ExceptionHandler != null)
			{
				ExceptionHandler.ExceptionArrived += HandleException;
				ExceptionHandler.ServerStatusUpdate += HandleException;
			}

			ScheduleViewModel = new AdmissionsScheduleViewModel();
			ScheduleViewModel.SynchronisationComplete += ScheduleViewModel_SynchronisationComplete;

			AlertViewModel.Schedule = ScheduleViewModel;
		}

		private AdmissionsScheduleViewModel _scheduleViewModel;
		public new AdmissionsScheduleViewModel ScheduleViewModel
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
		}

		/// <summary>
		/// Shows Action Bar and hides all other Panel
		/// </summary>
		public override void ClearAllSelections()
		{
			ScheduleViewModel.HidePanelsAndDeselectCommand.Execute(null);
		}

		/// <summary>
		/// Handles the event when there are new Bed updates available for UI consumption
		/// </summary>
		/// <param name="admissionData">The admission data.</param>
		protected override void HandleNewAdmissionsData(AdmissionsData admissionData)
		{
			// This has to be done on a background thread or there is a deadlock possibility
			System.Threading.Tasks.Task.Factory.StartNew(() =>
			{
				ScheduleViewModel.Synchronise(admissionData);
			}).LogExceptionIfThrownAndIgnore();
		}

		protected override void HandleNewTimeouts(PollingTimeouts timeouts)
		{
			// This has to be done on a background thread or there is a deadlock possibility
			System.Threading.Tasks.Task.Factory.StartNew(() =>
			{
				_presencePoller.SetTimeout(timeouts.PresenceTimeout);
				_admissionPoller.SetTimeout(timeouts.AdmissionsTimeout);
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


		public void InitialiseConfiguration(DeviceConfigurationInstance configuration, PollingTimeouts timeouts)
		{
			var defaultLocationCode = !string.IsNullOrEmpty(configuration.LocationCode) ? configuration.LocationCode : configuration.VisibleLocations.First().Code;

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
			ScheduleViewModel.WardTitle = defaultLocation.Name;
			ScheduleViewModel.WardCode = defaultLocation.Code;

			var invoker = MefContainer.GetExportedValue<IWcsAsyncInvoker>();
			if (invoker != null)
			{
				_presencePoller = new PresenceRecurringPoller(invoker, timeouts.PresenceTimeout);
				_presenceSource = Observable.FromEvent<IList<Presence>>(ev => _presencePoller.LocationConnectionUpdateAvailable += ev, ev => _presencePoller.LocationConnectionUpdateAvailable -= ev);
				_presenceSource.Subscribe(HandleNewPresences);

				_admissionPoller = new AdmissionsDataRecurringPoller(invoker, configuration.VisibleLocations.Select(l => l.Code).ToList(), timeouts.AdmissionsTimeout);
				_admissionSource = Observable.FromEvent<AdmissionsData>(ev => _admissionPoller.AdmissionsBedDataUpdateAvailable += ev, ev => _admissionPoller.AdmissionsBedDataUpdateAvailable -= ev);
				_admissionSource.Subscribe(HandleNewAdmissionsData);

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
		}
	}
}

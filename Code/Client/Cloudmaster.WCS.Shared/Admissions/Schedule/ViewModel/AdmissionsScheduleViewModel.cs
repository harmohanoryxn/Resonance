using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using WCS.Shared.Controls;
using WCS.Shared.Schedule;

namespace WCS.Shared.Admissions.Schedule
{
	public class AdmissionsScheduleViewModel : WcsScheduleViewModel<WaitingPatientViewModel>
	{
		public event Action SynchronisationComplete;

		private ObservableCollection<LocationSummaryViewModel> _alternativeLocations;
		private LocationSummaryViewModel _selectedAltervativeLocation;

		private AdmissionsWardObservableCollection _scheduleWards;
		private AdmissionsPatientObservableCollection _schedulePatients;
		private CollectionViewSource _wardsSource;
		private CollectionViewSource _patientsSource;
		private WaitingPatientViewModel _selectedPatient;
		private AdmissionsWardViewModel _selectedWard;

		private string _wardTitle;
		private string _wardCode;

		public AdmissionsScheduleViewModel()
		{
			Application.Current.Dispatcher.InvokeIfRequired((() =>
			{
				SchedulePatients = new AdmissionsPatientObservableCollection((bed) => new WaitingPatientViewModel(bed, Main.Popup, Main.DismissPopup, ToggleNotesVisibility, ToggleHistoryVisibility, ToggleCardInfoVisibility));
				SchedulePatients.TrySelect += HandleChangeToItemSelection;
				SchedulePatients.ShouldRecalculateStatistics += HandleRequestToRecalculateStatistics;

				ScheduleWards = new AdmissionsWardObservableCollection((room) => new AdmissionsWardViewModel(room, (bed) => new AdmissionBedViewModel(bed, Main.Popup, Main.DismissPopup, ToggleNotesVisibility, ToggleHistoryVisibility, ToggleCardInfoVisibility)));
				//TODO: Re-Add
				//ScheduleWards.TrySelect += HandleChangeToItemSelection;
				ScheduleWards.ShouldRecalculateStatistics += HandleRequestToRecalculateStatistics;

				WardsSource = new CollectionViewSource();
				WardsSource.SortDescriptions.Add(new SortDescription("WardName", ListSortDirection.Ascending));
				WardsSource.Source = ScheduleWards;

				PatientsSource = new CollectionViewSource();
				WardsSource.SortDescriptions.Add(new SortDescription("FamilyName", ListSortDirection.Ascending));
				PatientsSource.Source = SchedulePatients;
			}));

		}

		public AdmissionsWardObservableCollection ScheduleWards
		{
			get { return _scheduleWards; }
			set
			{
				_scheduleWards = value;
				this.DoRaisePropertyChanged(() => ScheduleWards, RaisePropertyChanged);
			}
		}

		public AdmissionsPatientObservableCollection SchedulePatients
		{
			get { return _schedulePatients; }
			set
			{
				_schedulePatients = value;
				this.DoRaisePropertyChanged(() => SchedulePatients, RaisePropertyChanged);
			}
		}

		public CollectionViewSource WardsSource
		{
			get { return _wardsSource; }
			set
			{
				_wardsSource = value;
				this.DoRaisePropertyChanged(() => WardsSource, RaisePropertyChanged);
			}
		}

		public CollectionViewSource PatientsSource
		{
			get { return _patientsSource; }
			set
			{
				_patientsSource = value;
				this.DoRaisePropertyChanged(() => PatientsSource, RaisePropertyChanged);
			}
		}

		public AdmissionsWardViewModel SelectedWard
		{
			get { return _selectedWard; }
			set
			{
					_selectedWard = value;
				this.DoRaisePropertyChanged(() => SelectedWard, RaisePropertyChanged);
			}
		}

		public WaitingPatientViewModel SelectedPatient
		{
			get { return _selectedPatient; }
			set
			{
				_selectedPatient = value;
				this.DoRaisePropertyChanged(() => SelectedPatient, RaisePropertyChanged);
			}
		}

		public string WardTitle
		{
			get { return _wardTitle; }
			set
			{
				_wardTitle = value;
				this.DoRaisePropertyChanged(() => WardTitle, RaisePropertyChanged);
			}
		}

		public string WardCode
		{
			get { return _wardCode; }
			set
			{
				_wardCode = value;
				this.DoRaisePropertyChanged(() => WardCode, RaisePropertyChanged);
			}
		}

		public LocationSummaryViewModel SelectedLocation
		{
			get { return _selectedAltervativeLocation; }
			set
			{
				if (_selectedAltervativeLocation != value)
				{
					_selectedAltervativeLocation = value;
					this.DoRaisePropertyChanged(() => SelectedLocation, RaisePropertyChanged);

					WardTitle = _selectedAltervativeLocation.Name;

					UpdateStatistics();
				}
			}
		}

		public ObservableCollection<LocationSummaryViewModel> AlternativeLocations
		{
			get { return _alternativeLocations; }
			set
			{
				_alternativeLocations = value;
				this.DoRaisePropertyChanged(() => AlternativeLocations, RaisePropertyChanged);
			}
		}
		 

		#region Filtering

		#endregion

		#region Synchronise

		/// <summary>
		/// Synchronises the admissions with potentially new ones from the server
		/// </summary>
		/// <param name="admissionData">The admission bed.</param>
		public void Synchronise(AdmissionsData admissionData)
		{
			if (admissionData == null) return;
			// sync the beds
			if (admissionData.Beds != null && admissionData.Beds.Any())
			{
				List<AdmissionsTopWard> wards = admissionData.Beds.GroupBy(bed => bed.Location.Name, bed => bed).Select(g =>
				{
					var first = g.First();
					return new AdmissionsTopWard(first.Location, g.ToList());
				}).ToList();

				ScheduleWards.Synchronise(wards);
				if (SelectedWard == null) SelectedWard = ScheduleWards.FirstOrDefault();
			}

			// sync the patients
            if (admissionData.Admissions != null && admissionData.Admissions.Any())
			{
				SchedulePatients.Synchronise(admissionData.Admissions);
			}

			LastSynchronised = ScheduleWards.LastSyncronized;

			UpdateStatistics();


			var sc = SynchronisationComplete;
			if (sc != null)
				sc.Invoke();


		}

		/// <summary>
		/// Synchronises the ward statuses with potentially new ones from the server
		/// </summary>
		/// <param name="locationConnections">A complete set of up-to-date ward statuses</param>
		public void Synchronise(IEnumerable<Presence> locationConnections)
		{
			ScheduleWards.AsParallel().ForEach(patient =>
			{
				patient.Synchronise(locationConnections);
			});

		}

		/// <summary>
		/// Synchronises the RFID detections with potentially new ones from the server
		/// </summary>
		/// <param name="detections">The orders.</param>
		public void Synchronise(IList<Detection> detections)
		{
			if (detections == null) return;

			SchedulePatients.UnfilteredCollection.Cast<WaitingPatientViewModel>().ForEach(p =>
			{
				var localDetections = detections.Where(d => d.PatientId == p.PatientId).ToList();
				p.Synchronise(localDetections);
			});
		}



		#endregion

		#region Overrides

		/// <summary>
		/// Updates the amounts for all the different patient types on the schedule
		/// </summary>
		protected override void UpdateStatistics()
		{
		}

		public override void HandleLockedEvent()
		{
		}

		public override IEnumerable<string> GetAlertMessages()
		{
			return Enumerable.Empty<string>();
		}


		public override void Tombstone()
		{
		}
		protected override void ClearAll()
		{
			ScheduleWards.ClearAll();
			SchedulePatients.ClearAll();
		}


		protected override void HandleMinuteTimerTick()
		{
			HandleCancelSelection();

			lock (ScheduleWards.SyncRoot)
			{
				ScheduleWards.ForEach(o => o.HandleMinuteTimerTick());
			}
			lock (SchedulePatients.SyncRoot)
			{
				SchedulePatients.ForEach(o => o.HandleMinuteTimerTick());
			}
		}

		protected override void HandleCancelSelection()
		{
			ShowCardPanel = false;
			ShowActionBar = false;
			ShowRfidPanel = false;
			ShowNotesPanel = false;
			ShowHistoryPanel = false;

			//TODO READD
			//SelectedWard = null;
			//SelectedPatient = null;

			ScheduleWards.ClearBedsSelectionType();
			SchedulePatients.ClearBedsSelectionType();
		}

		protected override void HandleChangeToItemSelection(WaitingPatientViewModel b, ScreenSelectionType? selectionType)
		{
			if (!selectionType.HasValue && SelectedWard == null)
			{
				SelectedPatient = b;
				SelectedWard = ScheduleWards.FirstOrDefault(room => room.ScheduleItems.Any(bed => bed.Id == b.Id));

				SchedulePatients.ForEach(bed => bed.IsHighlighted = (bed.Id == b.Id));
				ScheduleWards.SelectMany(sr => sr.ScheduleItems).ForEach(bed => bed.IsHighlighted = (bed.Id == b.Id));

				ShowActionBar = true;
				ToggleBedSelection(b);
			}
			else if (selectionType.HasValue && selectionType.Value == ScreenSelectionType.DeSelected)
			{
				SelectedWard = null;
				SelectedPatient = null;

				SchedulePatients.ForEach(bed => bed.IsHighlighted = false);
				ScheduleWards.SelectMany(sr => sr.ScheduleItems).ForEach(bed => bed.IsHighlighted = false);

				ShowActionBar = false;
				ClearBedsSelectionType();
			}
		}

		#endregion
		 

		internal void HandleNewWardSelection(LocationSummaryViewModel location)
		{
			SelectedLocation = location;
		}

		internal void ToggleBedSelection(WaitingPatientViewModel bed)
		{
			SchedulePatients.ToggleBedSelection(bed);
			//ScheduleWards.ToggleBedSelection(bed);
		}

		internal void ClearBedsSelectionType()
		{
			SchedulePatients.ClearBedsSelectionType();
			ScheduleWards.ClearBedsSelectionType();
		}
	}
}

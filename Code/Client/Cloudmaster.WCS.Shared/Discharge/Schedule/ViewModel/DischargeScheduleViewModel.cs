using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using GalaSoft.MvvmLight.Command;
using WCS.Shared.Beds;
using WCS.Shared.Controls;
using WCS.Shared.Schedule;

namespace WCS.Shared.Discharge.Schedule
{
	public class DischargeScheduleViewModel : WcsScheduleViewModel<DischargeBedViewModel>
	{
		public event Action SynchronisationComplete;

		private ObservableCollection<LocationSummaryViewModel> _alternativeLocations;
		private LocationSummaryViewModel _selectedAltervativeLocation;

		private DischargeRoomObservableCollection _scheduleRooms;
		private DischargeBedObservableCollection _scheduleBeds;
		private CollectionViewSource _timelineSource;
		private CollectionViewSource _mapSource;
		private DischargeBedViewModel _selectedBed;
		private DischargeRoomViewModel _selectedRoom;

		private string _wardTitle;
		private string _wardCode;

		private DischargeViewType _view;
		private bool _showTimelineView;
		private bool _showMapView;
		private bool _showCombinedView;

		public DischargeScheduleViewModel()
		{
			View = DischargeViewType.Combined;
			ShowTimelineView = false;
			ShowMapView = false;
			ShowCombinedView = true;

			Application.Current.Dispatcher.InvokeIfRequired((() =>
			{
				ScheduleBeds = new DischargeBedObservableCollection((discharge) => new DischargeBedViewModel(discharge, Main.Popup, Main.DismissPopup, ToggleNotesVisibility, ToggleHistoryVisibility, ToggleCardInfoVisibility));
				ScheduleBeds.TrySelect += HandleChangeToItemSelection;
				ScheduleBeds.ShouldRecalculateStatistics += HandleRequestToRecalculateStatistics;
				ScheduleBeds.StartingManipulation += HandleStartingManipulation;
				ScheduleBeds.EndedManipulation += HandleEndedManipulation;
		
				ScheduleRooms = new DischargeRoomObservableCollection((room) => new DischargeRoomViewModel(room, (bed) => FindDischarge(bed)));
				ScheduleRooms.TrySelect += HandleChangeToItemSelection;
				ScheduleRooms.ShouldRecalculateStatistics += HandleRequestToRecalculateStatistics;

				TimelineSource = new CollectionViewSource();
				TimelineSource.GroupDescriptions.Add(new PropertyGroupDescription("Room"));
				TimelineSource.SortDescriptions.Add(new SortDescription("Room", ListSortDirection.Ascending));
				TimelineSource.SortDescriptions.Add(new SortDescription("Bed", ListSortDirection.Ascending));
				TimelineSource.Source = ScheduleRooms;

				MapSource = new CollectionViewSource();
				MapSource.Source = ScheduleBeds;
			}));

		}

		private DischargeBedViewModel FindDischarge(BedDischargeData discharge)
		{
			var targetBed = ScheduleBeds.UnfilteredCollection.FirstOrDefault(b => b.Id == discharge.BedId);

			return targetBed;
		}

		public DischargeRoomObservableCollection ScheduleRooms
		{
			get { return _scheduleRooms; }
			set
			{
				_scheduleRooms = value;
				this.DoRaisePropertyChanged(() => ScheduleRooms, RaisePropertyChanged);
			}
		}

		public DischargeBedObservableCollection ScheduleBeds
		{
			get { return _scheduleBeds; }
			set
			{
				_scheduleBeds = value;
				this.DoRaisePropertyChanged(() => ScheduleBeds, RaisePropertyChanged);
			}
		}

		public CollectionViewSource TimelineSource
		{
			get { return _timelineSource; }
			set
			{
				_timelineSource = value;
				this.DoRaisePropertyChanged(() => TimelineSource, RaisePropertyChanged);
			}
		}

		public CollectionViewSource MapSource
		{
			get { return _mapSource; }
			set
			{
				_mapSource = value;
				this.DoRaisePropertyChanged(() => MapSource, RaisePropertyChanged);
			}
		}

		public DischargeBedViewModel SelectedBed
		{
			get { return _selectedBed; }
			set
			{
				_selectedBed = value;
				this.DoRaisePropertyChanged(() => SelectedBed, RaisePropertyChanged);
			}
		}

		public DischargeRoomViewModel SelectedRoom
		{
			get { return _selectedRoom; }
			set
			{
				_selectedRoom = value;
				this.DoRaisePropertyChanged(() => SelectedRoom, RaisePropertyChanged);
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

					// This will filter the appointments
					ScheduleRooms.Location = _selectedAltervativeLocation.Code;
					ScheduleBeds.Location = _selectedAltervativeLocation.Code;

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

		public RelayCommand<string> ChangeViewCommand
		{
			get { return new RelayCommand<string>(DoChangeView); }
		}

		#region Filtering

		#region View Filter

		public bool ShowTimelineView
		{
			get { return _showTimelineView; }
			set
			{
				if (_showTimelineView != value)
				{
					_showTimelineView = value;
					this.DoRaisePropertyChanged(() => ShowTimelineView, RaisePropertyChanged);

					if (_showTimelineView)
						View = DischargeViewType.Timeline;
				}
			}
		}

		public bool ShowMapView
		{
			get { return _showMapView; }
			set
			{
				if (_showMapView != value)
				{
					_showMapView = value;
					this.DoRaisePropertyChanged(() => ShowMapView, RaisePropertyChanged);

					if (_showMapView)
						View = DischargeViewType.Map;
				}
			}
		}

		public bool ShowCombinedView
		{
			get { return _showCombinedView; }
			set
			{
				if (_showCombinedView != value)
				{
					_showCombinedView = value;
					this.DoRaisePropertyChanged(() => ShowCombinedView, RaisePropertyChanged);

					if (_showCombinedView)
						View = DischargeViewType.Combined;
				}
			}
		}

		public DischargeViewType View
		{
			get { return _view; }
			set
			{
				_view = value;

				ShowTimelineView = (_view == DischargeViewType.Timeline);
				ShowMapView = (_view == DischargeViewType.Map);
				ShowCombinedView = (_view == DischargeViewType.Combined);

				this.DoRaisePropertyChanged(() => View, RaisePropertyChanged);
			}
		}

		/// <summary>
		/// Resets the order status to InProcess if it is deselected
		/// </summary>
		internal void ResetDefaultView()
		{
			Application.Current.Dispatcher.BeginInvokeIfRequired((() =>
			{
				View = DischargeViewType.Timeline;
			}));
		}

		/// <summary>
		/// Resets the selected location back to the default location
		/// </summary>
		internal void ResetDefaultWard()
		{
			if (DefaultLocation != SelectedLocation)
			{
				Application.Current.Dispatcher.BeginInvokeIfRequired((() =>
				{
					SelectedLocation = DefaultLocation;
				}));
			}
		}

		#endregion

		#endregion

		#region Synchronise

		/// <summary>
		/// Synchronises the discharge info with potentially new ones from the server
		/// </summary>
		/// <param name="discharges">A complete set of up-to-date discharge information</param>
		public void Synchronise(IList<BedDischargeData> discharges)
		{ 
			if (discharges == null) return;

			List<DischargeTopRoom> rooms = discharges.GroupBy(bed => bed.Room.Id, bed => bed).Select(g =>
			{
				var first = g.First();
				return new DischargeTopRoom(first.Room, g.ToList());
			}).ToList();

			//sync
			//its important that beds get synced before rooms do
			ScheduleBeds.Synchronise(discharges);

			ScheduleRooms.Synchronise(rooms);

			LastSynchronised = ScheduleRooms.LastSyncronized;

			UpdateStatistics();


			var sc = SynchronisationComplete;
			if (sc != null)
				sc.Invoke();

		}

		/// <summary>
		/// Synchronises the ward statuses with potentially new ones from the server
		/// </summary>
		/// <param name="presences">A complete set of up-to-date ward statuses</param>
		public void Synchronise(IEnumerable<Presence> presences)
		{
			ScheduleRooms.AsParallel().ForEach(patient =>
			{
				patient.Synchronise(presences);
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
			ScheduleRooms.ForEach(o => o.HidePopups());
		}

		public override IEnumerable<string> GetAlertMessages()
		{
			return Enumerable.Empty<string>();
		}
		 
		public override void Tombstone()
		{
			ScheduleRooms.ForEach(o => o.HideAllNotes());
		}
		protected override void ClearAll()
		{
			ScheduleRooms.ClearAll();
			ScheduleBeds.ClearAll();
		}


		protected override void HandleMinuteTimerTick()
		{
			HandleCancelSelection();

			lock (ScheduleRooms.SyncRoot)
			{
				ScheduleRooms.ForEach(o => o.HandleMinuteTimerTick());
			}
			lock (ScheduleBeds.SyncRoot)
			{
				ScheduleBeds.ForEach(o => o.HandleMinuteTimerTick());
			}
		}

		protected override void HandleCancelSelection()
		{
			ShowActionBar = false;
			ShowRfidPanel = false;
			ShowNotesPanel = false;
			ShowHistoryPanel = false;
			ShowCardPanel = false;

			SelectedBed = null;
			SelectedRoom = null;

			ScheduleRooms.ClearBedsSelectionType();
			ScheduleBeds.ClearBedsSelectionType();
		}

		protected override void HandleChangeToItemSelection(DischargeBedViewModel b, ScreenSelectionType? selectionType)
		{
			if (!selectionType.HasValue && SelectedBed == null)
			{
				SelectedBed = b;
				SelectedRoom = ScheduleRooms.FirstOrDefault(room => room.ScheduleItems.Any(bed => bed.Id == b.Id));

				ScheduleBeds.ForEach(bed => bed.IsHighlighted = (bed.Id == b.Id));
				ScheduleRooms.SelectMany(sr => sr.ScheduleItems).ForEach(bed => bed.IsHighlighted = (bed.Id == b.Id));

				ShowActionBar = true;
				ToggleBedSelection(b);
			}
			else if (selectionType.HasValue && selectionType.Value == ScreenSelectionType.DeSelected)
			{
				SelectedBed = null;
				SelectedRoom = null;

				ScheduleBeds.ForEach(bed => bed.IsHighlighted = false);
				ScheduleRooms.SelectMany(sr => sr.ScheduleItems).ForEach(bed => bed.IsHighlighted = false);

				ShowActionBar = false;
				ClearBedsSelectionType();
			}
		}

		#endregion


		private void DoChangeView(string newView)
		{
			switch (newView)
			{
				case "Default":
					View = DischargeViewType.Timeline;
					break;
				case "Map":
					View = DischargeViewType.Map;
					break;
				case "Combined":
					View = DischargeViewType.Combined;
					break;
			}
		}

		internal void HandleNewWardSelection(LocationSummaryViewModel location)
		{
			SelectedLocation = location;
		}

		internal void ToggleBedSelection(DischargeBedViewModel bed)
		{
			ScheduleBeds.ToggleBedSelection(bed);
			ScheduleRooms.ToggleBedSelection(bed);
		}

		internal void ClearBedsSelectionType()
		{
			ScheduleBeds.ClearBedsSelectionType();
			ScheduleRooms.ClearBedsSelectionType();
		}
	}
}

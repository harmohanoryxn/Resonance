using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using GalaSoft.MvvmLight.Command;
using WCS.Shared.Beds;
using WCS.Shared.Controls;
using WCS.Shared.Orders;
using WCS.Shared.Schedule;
using WCS.Shared.Ward.Schedule;

namespace WCS.Shared.Cleaning.Schedule
{
	public class CleaningScheduleViewModel : WcsScheduleViewModel<BedViewModel>
	{
		public event Action SynchronisationComplete;

		private ObservableCollection<LocationSummaryViewModel> _alternativeLocations;
		private LocationSummaryViewModel _selectedAltervativeLocation;

		private CleaningRoomObservableCollection _scheduleRooms;
		private CleaningBedObservableCollection _scheduleBeds;
		private CollectionViewSource _timelineSource;
		private CollectionViewSource _mapSource;
		private BedViewModel _selectedBed;
		private RoomViewModel _selectedRoom;

		private string _wardTitle;
		private string _wardCode;

		private bool _sortByAvailability;
		private bool _sortByRoom;

		public CleaningScheduleViewModel()
		{
			SortByRoom = true;

			Application.Current.Dispatcher.InvokeIfRequired((() =>
			{
				ScheduleBeds = new CleaningBedObservableCollection((bed) => new BedViewModel(bed, Main.Popup, Main.DismissPopup, ToggleNotesVisibility, ToggleHistoryVisibility, ToggleCardInfoVisibility));
				ScheduleBeds.TrySelect += HandleChangeToItemSelection;
				ScheduleBeds.ShouldRecalculateStatistics += HandleRequestToRecalculateStatistics;

				ScheduleRooms = new CleaningRoomObservableCollection((room) => new RoomViewModel(room, (bed) => FindBed(bed)));
				ScheduleRooms.TrySelect += HandleChangeToItemSelection;
				ScheduleRooms.ShouldRecalculateStatistics += HandleRequestToRecalculateStatistics;

				TimelineSource = new CollectionViewSource();
				TimelineSource.SortDescriptions.Add(new SortDescription("RoomName", ListSortDirection.Ascending));
				TimelineSource.Source = ScheduleRooms;

				MapSource = new CollectionViewSource();
				MapSource.Source = ScheduleBeds;
			}));

		}

		private BedViewModel FindBed(Bed bed)
		{
			var targetBed = ScheduleBeds.UnfilteredCollection.FirstOrDefault(b => b.Id == bed.BedId);

			return targetBed;
		}

		public CleaningRoomObservableCollection ScheduleRooms
		{
			get { return _scheduleRooms; }
			set
			{
				_scheduleRooms = value;
				this.DoRaisePropertyChanged(() => ScheduleRooms, RaisePropertyChanged);
			}
		}

		public CleaningBedObservableCollection ScheduleBeds
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

		public BedViewModel SelectedBed
		{
			get { return _selectedBed; }
			set
			{
				_selectedBed = value;
				this.DoRaisePropertyChanged(() => SelectedBed, RaisePropertyChanged);
			}
		}

		public RoomViewModel SelectedRoom
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

		#region Filtering

		#region Sort Filter

		public RelayCommand<string> SortCommand
		{
			get { return new RelayCommand<string>(DoSortCommand); }
		}

		private void DoSortCommand(string byWhat) 
		{
			switch (byWhat)
			{
				case "By Availability":
					Application.Current.Dispatcher.InvokeIfRequired((() =>
					{
						SortByAvailability = true;
						SortByRoom = false;
						TimelineSource.SortDescriptions.Clear();
						TimelineSource.SortDescriptions.Add(new SortDescription("TimeToAvailability", ListSortDirection.Ascending));
						TimelineSource.View.Refresh();
					}));
					break;
				case "By Room":
					Application.Current.Dispatcher.InvokeIfRequired((() =>
					{
						SortByRoom = true;
						SortByAvailability = false;
						TimelineSource.SortDescriptions.Clear();
						TimelineSource.SortDescriptions.Add(new SortDescription("RoomName", ListSortDirection.Ascending));
						TimelineSource.View.Refresh();
					}));
					break;
			}
		}
		public bool SortByAvailability
		{
			get { return _sortByAvailability; }
			set
			{
				_sortByAvailability = value;
				this.DoRaisePropertyChanged(() => SortByAvailability, RaisePropertyChanged);
			}
		}

		public bool SortByRoom
		{
			get { return _sortByRoom; }
			set
			{
				_sortByRoom = value;
				this.DoRaisePropertyChanged(() => SortByRoom, RaisePropertyChanged);
			}
		}

		#endregion

		/// <summary>
		/// Resets the selected location back to the default location
		/// </summary>
		internal void ResetDefaultSort()
		{
			if (!SortByRoom)
			{
				Application.Current.Dispatcher.BeginInvokeIfRequired((() =>
				{
					DoSortCommand("By Room");
				}));

			}
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


		#region Synchronise

		/// <summary>
		/// Synchronises the bedTimes with potentially new ones from the server
		/// </summary>
		/// <param name="beds">A complete set of up-to-date ward statuses</param>
		public void Synchronise(IList<Bed> beds)
		{
			if (beds == null) return;

			List<TopRoom> rooms = beds.GroupBy(bed => bed.Room.Id, bed => bed).Select(g =>
			{
				var first = g.First();
				return new TopRoom(first.Room, g.ToList());
			}).ToList();

			//sync
			//its important that beds get synced before rooms do
			ScheduleBeds.Synchronise(beds);
			//	ScheduleBeds.SetSupplementaryData(beds.SelectMany(b => b.AvailableTimes), (o) => new OrderViewModel(o, null, Main.PopupOrder, Main.PopupBed, Main.DoDismissPopup, ToggleRfidView, GetDefaultLocation, ToggleNotesVisibility, ToggleHistoryVisibility, (order) => order.Notifications.Select(n => new NotificationItemViewModel(n)), TimelineItemType.NoteIn | TimelineItemType.NoteOut | TimelineItemType.ProcedureTimeUpdated | TimelineItemType.OrderAssigned | TimelineItemType.OrderCompleted | TimelineItemType.NotificationAcknowlegement));

			ScheduleRooms.Synchronise(rooms);
			//	ScheduleRooms.SetSupplementaryData(beds.SelectMany(b => b.AvailableTimes), (o) => new OrderViewModel(o, null, Main.PopupOrder, Main.PopupBed, Main.DoDismissPopup, ToggleRfidView, GetDefaultLocation, ToggleNotesVisibility, ToggleHistoryVisibility, (order) => order.Notifications.Select(n => new NotificationItemViewModel(n)), TimelineItemType.NoteIn | TimelineItemType.NoteOut | TimelineItemType.ProcedureTimeUpdated | TimelineItemType.OrderAssigned | TimelineItemType.OrderCompleted | TimelineItemType.NotificationAcknowlegement));

			LastSynchronised = ScheduleRooms.LastSyncronized;

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
			ScheduleRooms.AsParallel().ForEach(patient =>
			{
				patient.Synchronise(locationConnections);
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
			ShowCardPanel = false;
			ShowActionBar = false;
			ShowRfidPanel = false;
			ShowNotesPanel = false;
			ShowHistoryPanel = false;

			SelectedBed = null;
			SelectedRoom = null;

			ScheduleRooms.ClearBedsSelectionType();
			ScheduleBeds.ClearBedsSelectionType();
		}

		protected override void HandleChangeToItemSelection(BedViewModel b, ScreenSelectionType? selectionType)
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

		internal void HandleNewWardSelection(LocationSummaryViewModel location)
		{
			SelectedLocation = location;
		}

		internal void ToggleBedSelection(BedViewModel bed)
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

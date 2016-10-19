using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using GalaSoft.MvvmLight;
using WCS.Shared.Controls; 
using WCS.Shared.Schedule;

namespace WCS.Shared.Discharge.Schedule
{
	/// <summary>
	/// Describes  a Room and all it's beds
	/// </summary>
	public class DischargeRoomViewModel : ViewModelBase, ISynchroniseable<DischargeRoomViewModel>, IComparer<DischargeRoomViewModel>
	{
		public event Action<DischargeBedViewModel, ScreenSelectionType?> TrySelectBed;

		private CollectionViewSource _timelineSource;

		private DischargeBedObservableCollection _scheduleItems;
		private DischargeBedViewModel _selectedBed;
		private DeviceInfoViewModel _wardLocationPresence;

		private ScreenSelectionType? _selectionType;
		private bool _mrsaRisk;
		private bool _latexAllergy;
		private bool _radiationRisk;

		public DischargeRoomViewModel(DischargeTopRoom room, Func<BedDischargeData, DischargeBedViewModel> transformFunction)
		{
			CoreRoom = room;

			ScheduleItems = new DischargeBedObservableCollection(transformFunction);
			ScheduleItems.TrySelect += HandleTrySelectBed;

			WardLocationPresence = new DeviceInfoViewModel { Location = room.Room.Ward, LocationFullName = room.Room.Ward, LocationType = DeviceInfoViewModel.Mode.Ward };
			 
			Synchronise(room);

			Application.Current.Dispatcher.InvokeIfRequired((() =>
			{
				TimelineSource = new CollectionViewSource();
				TimelineSource.SortDescriptions.Add(new SortDescription("Bed", ListSortDirection.Ascending));
				TimelineSource.Source = ScheduleItems;

			}));

			SelectionType = null;
		}

		#region IIdentity

		public int Id
		{
			get { return CoreRoom.Id; }
		}

		public int GetFingerprint()
		{
			return CoreRoom.GetFingerprint();

		}

		#endregion

		public DischargeRoomViewModel Room { get; set; }
		public DischargeTopRoom CoreRoom { get; set; }

		protected Func<LocationSummary> GetDefaultLocationCallback { get; set; }
		protected Action<DischargeRoomViewModel> LocateRoomCallback { get; set; }

		public WcsViewModel Main { get; set; }

		public DischargeBedObservableCollection ScheduleItems
		{
			get { return _scheduleItems; }
			set
			{
				_scheduleItems = value;
				this.DoRaisePropertyChanged(() => ScheduleItems, RaisePropertyChanged);
			}
		}

		public ScreenSelectionType? SelectionType
		{
			get { return _selectionType; }
			set
			{
				_selectionType = value;
				this.DoRaisePropertyChanged(() => SelectionType, RaisePropertyChanged);
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

		public DischargeBedViewModel SelectedBed
		{
			get { return _selectedBed; }
			set
			{
				_selectedBed = value;
				this.DoRaisePropertyChanged(() => SelectedBed, RaisePropertyChanged);
			}
		}

		public string RoomName
		{
			get { return CoreRoom.Room.Name; }
		}

		public string LocationCode
		{
			get { return CoreRoom.Room.Ward; }
		}

		public DischargeBedViewModel Bed
		{
			get { return null; }
		}

		public DeviceInfoViewModel WardLocationPresence
		{
			get { return _wardLocationPresence; }
			private set
			{
				_wardLocationPresence = value;
				this.DoRaisePropertyChanged(() => WardLocationPresence, RaisePropertyChanged);
			}
		}

		public bool MrsaRisk
		{
			get { return _mrsaRisk; }
			set
			{
				_mrsaRisk = value;
				this.DoRaisePropertyChanged(() => MrsaRisk, RaisePropertyChanged);
			}
		}

		public bool LatexAllergy
		{
			get { return _latexAllergy; }
			set
			{
				_latexAllergy = value;
				this.DoRaisePropertyChanged(() => LatexAllergy, RaisePropertyChanged);
			}
		}

		public bool RadiationRisk
		{
			get { return _radiationRisk; }
			set
			{
				_radiationRisk = value;
				this.DoRaisePropertyChanged(() => RadiationRisk, RaisePropertyChanged);
			}
		}

		#region Synchronise
		 
		public void Synchronise(DischargeRoomViewModel room)
		{
			if (_disposed)
				return;

			if (room.Id != CoreRoom.Id)
				return;

			if (room.GetFingerprint() == GetFingerprint())
				return;

			Synchronise(room.CoreRoom);
		}

		/// <summary>
		/// Synchronises the underlying room.
		/// </summary>
		/// <param name="room">The room</param>
		internal void Synchronise(DischargeTopRoom room)
		{
			if (_disposed)
				return;

			if (room.Id != CoreRoom.Id)
				return;

			CoreRoom = room;

			this.DoRaisePropertyChanged(() => RoomName, RaisePropertyChanged);
			this.DoRaisePropertyChanged(() => LocationCode, RaisePropertyChanged);

			ScheduleItems.Synchronise(room.Discharges);

			WardLocationPresence.Synchronise(room);
		}
 
		/// <summary>
		/// Synchronises the ward statuses with potentially new ones from the server
		/// </summary>
		/// <param name="locationConnections">A complete set of up-to-date ward statuses</param>
		public void Synchronise(IEnumerable<Presence> locationConnections)
		{
			if (locationConnections == null) return;

			var locations = locationConnections.ToDictionary(ws => ws.LocationCode, ws => ws);
			lock (ScheduleItems.SyncRoot)
			{
				ScheduleItems.AsParallel().ForEach(order =>
				{ 
					if (locations.ContainsKey(WardLocationPresence.Location))
					{
						var locationConnection = locations[CoreRoom.Room.Ward];
						WardLocationPresence.Synchronise(locationConnection);
						ScheduleItems.ForEach(b => b.WardLocationPresence.Synchronise(locationConnection));
					}
				});
			}
		} 

		#endregion

		internal IEnumerable<string> GetAlertMessages()
		{
			return Enumerable.Empty<string>();
		}

		internal IEnumerable<string> GetUpdateMessages()
		{
			return Enumerable.Empty<string>();
		}

		internal IEnumerable<string> GetOkMessages()
		{
			return Enumerable.Empty<string>();
		}

		internal void Tombstone()
		{
			lock (ScheduleItems.SyncRoot)
			{
				ScheduleItems.ForEach(o => o.HideAllNotes());
			}
		}

		internal void ClearAllBeds()
		{
			lock (ScheduleItems.SyncRoot)
			{
				ScheduleItems.ClearAll();
			}
		}

		internal void HidePopups()
		{
			ScheduleItems.AsParallel().ForEach(o => o.HidePopups());
		}

		internal void HideAllNotes()
		{
			ScheduleItems.AsParallel().ForEach(o => o.HideAllNotes());
		}

		internal void HandleMinuteTimerTick()
		{
			ScheduleItems.AsParallel().ForEach(o => o.HandleMinuteTimerTick());
		}

		#region Selection

		internal void HandleTrySelectBed(DischargeBedViewModel bed, ScreenSelectionType? existingSelectionType)
		{
			var tsb = TrySelectBed;
			if (tsb != null)
				tsb.Invoke(bed, existingSelectionType);
		}

		internal void ToggleOrderSelection(DischargeBedViewModel bed)
		{
			SelectedBed = bed;
			ScheduleItems.SetBedAsSelected(bed);
			SelectionType = ScheduleItems.Any(o => o.SelectionType == ScreenSelectionType.Selected)
				? ScreenSelectionType.Selected : ScreenSelectionType.DeSelected;
		}

		internal void ClearBedsSelectionType()
		{
			SelectedBed = null;
			ScheduleItems.ClearBedsSelectionType();
			SelectionType = null;
		}

		#endregion
		
		#region IDispose

		private volatile bool _disposed = false;

		protected override void Dispose(bool disposing)
		{
			if (!this._disposed)
			{
				_disposed = true;


				if (_scheduleItems != null)
				{
					_scheduleItems.UnfilteredCollection.ForEach(o => o.Dispose());
				}
			}
		}

		#endregion

		public int Compare(DischargeRoomViewModel x, DischargeRoomViewModel y)
		{
			return x.Id.CompareTo(y.Id);
		}
	}

}

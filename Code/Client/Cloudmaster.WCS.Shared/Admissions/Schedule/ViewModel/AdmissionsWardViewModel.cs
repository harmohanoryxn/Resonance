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

namespace WCS.Shared.Admissions.Schedule
{
	/// <summary>
	/// Describes  an Admission Ward and all it's beds
	/// </summary>
	public class AdmissionsWardViewModel : ViewModelBase, ISynchroniseable<AdmissionsWardViewModel>, IComparer<AdmissionsWardViewModel>
	{
		public event Action<AdmissionBedViewModel, ScreenSelectionType?> TrySelectBed;

		private CollectionViewSource _bedSource;

		private AdmissionsBedObservableCollection _scheduleItems;
		private AdmissionBedViewModel _selectedBed;
		private DeviceInfoViewModel _wardLocationPresence;

		private ScreenSelectionType? _selectionType;

		public AdmissionsWardViewModel(AdmissionsTopWard ward, Func<BedDischargeData, AdmissionBedViewModel> transformFunction)
		{
			CoreWard = ward;

			ScheduleItems = new AdmissionsBedObservableCollection(transformFunction);
			ScheduleItems.TrySelect += HandleTrySelectBed;

			WardLocationPresence = new DeviceInfoViewModel { Location = ward.Ward.Name, LocationFullName = ward.Ward.FullName, LocationType = DeviceInfoViewModel.Mode.Ward };
			 
			Synchronise(ward);

			Application.Current.Dispatcher.InvokeIfRequired((() =>
			{
				BedSource = new CollectionViewSource();
				BedSource.SortDescriptions.Add(new SortDescription("Bed", ListSortDirection.Ascending));
				BedSource.Source = ScheduleItems;

			}));

			SelectionType = null;
		}

		#region IIdentity

		public int Id
		{
			get { return CoreWard.Id; }
		}

		public int GetFingerprint()
		{
			return CoreWard.GetFingerprint();

		}

		#endregion

		public AdmissionsWardViewModel Room { get; set; }
		public AdmissionsTopWard CoreWard { get; set; }

		protected Func<LocationSummary> GetDefaultLocationCallback { get; set; }
		protected Action<AdmissionsWardViewModel> LocateRoomCallback { get; set; }

		public WcsViewModel Main { get; set; }

		public AdmissionsBedObservableCollection ScheduleItems
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
		 
		public CollectionViewSource BedSource
		{
			get { return _bedSource; }
			set
			{
				_bedSource = value;
				this.DoRaisePropertyChanged(() => BedSource, RaisePropertyChanged);
			}
		}

		public AdmissionBedViewModel SelectedBed
		{
			get { return _selectedBed; }
			set
			{
				_selectedBed = value;
				this.DoRaisePropertyChanged(() => SelectedBed, RaisePropertyChanged);
			}
		}

		public string WardName
		{
			get { return CoreWard.Ward.FullName; }
		}

		public string WardCode
		{
			get { return CoreWard.Ward.Name; }
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

		#region Synchronise
		 
		public void Synchronise(AdmissionsWardViewModel room)
		{
			if (_disposed)
				return;

			if (room.Id != CoreWard.Id)
				return;

			if (room.GetFingerprint() == GetFingerprint())
				return;

			Synchronise(room.CoreWard);
		}

		/// <summary>
		/// Synchronises the underlying room.
		/// </summary>
		/// <param name="ward">The room</param>
		internal void Synchronise(AdmissionsTopWard ward)
		{
			if (_disposed)
				return;

			if (ward.Id != CoreWard.Id)
				return;

			CoreWard = ward;

			this.DoRaisePropertyChanged(() => WardName, RaisePropertyChanged);
			this.DoRaisePropertyChanged(() => WardCode, RaisePropertyChanged);

			ScheduleItems.Synchronise(ward.Beds);

			WardLocationPresence.Synchronise(ward);
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
						var locationConnection = locations[CoreWard.Ward.Name];
						WardLocationPresence.Synchronise(locationConnection);
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

		 
		internal void ClearAllBeds()
		{
			lock (ScheduleItems.SyncRoot)
			{
				ScheduleItems.ClearAll();
			}
		}
		 
		internal void HandleMinuteTimerTick()
		{
			ScheduleItems.AsParallel().ForEach(o => o.HandleMinuteTimerTick());
		}

		#region Selection

		internal void HandleTrySelectBed(AdmissionBedViewModel bed, ScreenSelectionType? existingSelectionType)
		{
			var tsb = TrySelectBed;
			if (tsb != null)
				tsb.Invoke(bed, existingSelectionType);
		}

		internal void ToggleOrderSelection(AdmissionBedViewModel bed)
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

		public int Compare(AdmissionsWardViewModel x, AdmissionsWardViewModel y)
		{
			return x.Id.CompareTo(y.Id);
		}
	}

}

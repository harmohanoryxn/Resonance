using System;
using System.Linq;
using System.Threading.Tasks;
using Cloudmaster.WCS.DataServicesInvoker;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using GalaSoft.MvvmLight.Command;
using WCS.Core;
using WCS.Shared.Notes;
using WCS.Shared.Orders;
using WCS.Shared.Schedule;

namespace WCS.Shared.Beds
{
	/// <summary>
	/// Handles all the manipulation and updating of an item that appears in the schedule
	/// </summary>
	public class BedViewModel : UpdatingViewModel, ISynchroniseable<BedViewModel>
	{
		public event Action<BedViewModel, ScreenSelectionType?> TrySelect;
		public event Action ShouldRecalculateStatistics;

		private Bed _bed;
		private Action<BedViewModel> _popupBedCallback;
		private Action _dismissPopupCallback;
		private ConversationNotesViewModel _conversationNotesViewModel;
		private OrderViewModel _underlyingOrder;

		private TimelineCoordinator _orderHeaderTimelineCoordinator;
		private TimelineCoordinator _timelineCoordinator;

		private Action _toggleNotesCallback;
		private Action _toggleHistoryCallback;
		private Action _toggleInfoCardCallback;
		private DeviceInfoViewModel _wardLocationPresence;

		private string _hostLocation;
		private bool _isUpdating;
		private bool _canAllowUpdatingStatus;
		private bool _bedRequiresCleaning;
		private bool _isHighlighted;
		private ScreenSelectionType? _selectionType;


		public BedViewModel(Bed bed, Action<BedViewModel> popupCallback, Action dismissPopupCallback, Action toggleNotesCallback, Action toggleHistoryCallback, Action toggleInfoCardCallback)
		{
			_popupBedCallback = popupCallback;
			_dismissPopupCallback = dismissPopupCallback;
			_toggleNotesCallback = toggleNotesCallback;
			_toggleHistoryCallback = toggleHistoryCallback;
			_toggleInfoCardCallback = toggleInfoCardCallback;

			_bed = bed;

			TimelineCoordinator = new TimelineCoordinator(TimelineItemType.NoteIn | TimelineItemType.NoteOut | TimelineItemType.ProcedureTimeUpdated | TimelineItemType.OrderAssigned | TimelineItemType.PatientOccupied | TimelineItemType.CleaningService | TimelineItemType.BedMarkedAsDirty | TimelineItemType.Discharge);
			TimelineCoordinator.Location = _hostLocation;

			TimelineCoordinator.Synchronise(bed);

			OrderHeaderTimeline = new TimelineCoordinator(TimelineItemType.FreeRoom);
			OrderHeaderTimeline.Synchronise(bed);

			ConversationNotesViewModel = new ConversationNotesViewModel(_bed);
			ConversationNotesViewModel.BedUpdateAvailable += SynchroniseWithUpdate;

			WardLocationPresence = new DeviceInfoViewModel { Location = bed.Location.Name, LocationFullName = bed.Location.FullName, LocationType = DeviceInfoViewModel.Mode.Ward };

			BedRequiresCleaning = bed.CurrentStatus != BedStatus.Clean;
			CanAllowUpdatingStatus = true;
			SelectionType = null;
		}


		#region ISynchroniseable

		public int Id
		{
			get { return _bed.BedId; }
		}

		public int GetFingerprint()
		{
			return _bed.GetFingerprint();
		}

		/// <summary>
		/// Set and resets all of the view models properties with a new or modified bed
		/// </summary>
		/// <param name="bed">The bed</param>
		public void Synchronise(BedViewModel bed)
		{
			if (_disposed)
				return;

			if (bed.Id != _bed.BedId)
				return;

			Task.Factory.StartNew(() => Synchronise(bed._bed)).LogExceptionIfThrownAndIgnore();
		}

		/// <summary>
		/// Synchronises the bed as well as signalling that the labels need updating 
		/// </summary>
		/// <param name="bed">The bed</param>
		public void SynchroniseWithUpdate(Bed bed)
		{
			MarkUpdateAsArrived();

			Synchronise(bed);

			var srs = ShouldRecalculateStatistics;
			if (srs != null)
				srs.Invoke();
		}

		/// <summary>
		/// Set and resets all of the view models properties with a new or modified bed
		/// </summary>
		/// <param name="bed">The bed</param>
		public void Synchronise(Bed bed)
		{
			if (_disposed)
				return;

			if (bed.BedId != _bed.BedId)
				return;

			if (CheckIfUpdateIsPending) return;

			if (bed.GetFingerprint() == _bed.GetFingerprint())
				return;

			IsUpdating = true;

			_bed = bed;

			_timelineCoordinator.Synchronise(bed);

			_conversationNotesViewModel.Synchronise(bed);

			BedRequiresCleaning = bed.CurrentStatus != BedStatus.Clean;

			IsUpdating = false;

			this.DoRaisePropertyChanged(() => Id, RaisePropertyChanged);
			this.DoRaisePropertyChanged(() => Status, RaisePropertyChanged);
			this.DoRaisePropertyChanged(() => IsBedClean, RaisePropertyChanged);
			this.DoRaisePropertyChanged(() => BedRequiresCleaning, RaisePropertyChanged);
			this.DoRaisePropertyChanged(() => Bed, RaisePropertyChanged);
			this.DoRaisePropertyChanged(() => Room, RaisePropertyChanged);
			this.DoRaisePropertyChanged(() => LocationCode, RaisePropertyChanged);
			this.DoRaisePropertyChanged(() => MrsaRisk, RaisePropertyChanged);
			this.DoRaisePropertyChanged(() => StatusText, RaisePropertyChanged);
			this.DoRaisePropertyChanged(() => EstimatedDischargeDate, RaisePropertyChanged);
			this.DoRaisePropertyChanged(() => TimeToAvailability, RaisePropertyChanged);

		}

		#endregion

		public DeviceInfoViewModel WardLocationPresence
		{
			get { return _wardLocationPresence; }
			private set
			{
				_wardLocationPresence = value;
				this.DoRaisePropertyChanged(() => WardLocationPresence, RaisePropertyChanged);
			}
		}

		public TimelineCoordinator TimelineCoordinator
		{
			get { return _timelineCoordinator; }
			private set { _timelineCoordinator = value; }
		}

		public TimelineCoordinator OrderHeaderTimeline
		{
			get { return _orderHeaderTimelineCoordinator; }
			private set { _orderHeaderTimelineCoordinator = value; }
		}

		public int? PatientId
		{
			get { return null; }
		}


		public string SearchString
		{
			get
			{
				return _bed.Name;
			}
		}
		public bool IsUpdating
		{
			get { return _isUpdating; }
			set
			{
				_isUpdating = value;
				this.DoRaisePropertyChanged(() => IsUpdating, RaisePropertyChanged);
			}
		}

		public ConversationNotesViewModel ConversationNotesViewModel
		{
			get { return _conversationNotesViewModel; }
			set
			{
				_conversationNotesViewModel = value;
				this.DoRaisePropertyChanged(() => ConversationNotesViewModel, RaisePropertyChanged);
			}
		}

		public RelayCommand PopupBedCommand
		{
			get { return new RelayCommand(DoPopupBed); }
		}

		public RelayCommand TrySelectCommand
		{
			get { return new RelayCommand(DoTrySelect); }
		}

		public bool MrsaRisk
		{
			get { return _bed.CriticalCareIndicators.IsMrsaRisk; }
			private set
			{
				if (_bed.CriticalCareIndicators.IsMrsaRisk != value)
				{
					_bed.CriticalCareIndicators.IsMrsaRisk = value;
					this.DoRaisePropertyChanged(() => MrsaRisk, RaisePropertyChanged);
				}
			}
		}

		public bool RadiationRisk
		{
			get { return _bed.CriticalCareIndicators.IsRadiationRisk; }
			private set
			{
				if (_bed.CriticalCareIndicators.IsRadiationRisk != value)
				{
					_bed.CriticalCareIndicators.IsRadiationRisk = value;
					this.DoRaisePropertyChanged(() => RadiationRisk, RaisePropertyChanged);
				}
			}
		}

		public bool LatexRisk
		{
			get { return _bed.CriticalCareIndicators.HasLatexAllergy; }
			private set
			{
				if (_bed.CriticalCareIndicators.HasLatexAllergy != value)
				{
					_bed.CriticalCareIndicators.HasLatexAllergy = value;
					this.DoRaisePropertyChanged(() => LatexRisk, RaisePropertyChanged);
				}
			}
		}

		public bool IsAvailableNowForCleaning
		{
			get
			{
				var items = OrderHeaderTimeline.TimelineItems.Where(ti => ti.TimelineType == TimelineItemType.FreeRoom).ToList();
				var times = items.Cast<ITimeDefinition>().ToList();
				return TimeDefinition.ContainsTime(times, DateTime.Now.TimeOfDay) && Status != BedStatus.Clean;
			}
		}

		public bool BedRequiresCleaning
		{
			get { return _bedRequiresCleaning; }
			set
			{
				_bedRequiresCleaning = value;
				this.DoRaisePropertyChanged(() => BedRequiresCleaning, RaisePropertyChanged);
			}
		}

		public string Bed
		{
			get { return _bed.Name; }
		}

		public string Room
		{
			get { return _bed.Room.Name; }
		}

		public string LocationCode
		{
			get { return _bed.Location.Name; }
		}

		public DateTime? EstimatedDischargeDate
		{
			get { return _bed.EstimatedDischargeDate; }
		}

		public BedStatus Status
		{
			get
			{
				return _bed.CurrentStatus;
			}
		}

		public RelayCommand MarkAsCleanCommand
		{
			get { return new RelayCommand(DoMarkAsClean); }
		}
		public RelayCommand MarkAsDirtyCommand
		{
			get { return new RelayCommand(DoMarkAsDirty); }
		}
		public bool IsBedClean
		{
			get { return _bed.CurrentStatus == BedStatus.Clean; }
		}

		public bool IsHighlighted
		{
			get { return _isHighlighted; }
			set
			{
				_isHighlighted = value;
				this.DoRaisePropertyChanged(() => IsHighlighted, RaisePropertyChanged);
			}
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public string StatusText
		{
			get
			{
				var times = OrderHeaderTimeline.TimelineItems.Where(ti => ti.TimelineType == TimelineItemType.FreeRoom).OfType<ITimeDefinition>().ToList();
				if (TimeDefinition.ContainsTime(times, DateTime.Now.TimeOfDay))
				{
					return "Available Now";
				}
				var nextAvailableTime = TimeDefinition.GetNextTime(times, DateTime.Now.TimeOfDay);
				
				if (EstimatedDischargeDate.HasValue && nextAvailableTime.HasValue && EstimatedDischargeDate.Value.TimeOfDay < nextAvailableTime)
					return string.Format("Estimated discharge a {0}", EstimatedDischargeDate.Value.TimeOfDay.ToWcsHoursMinutesFormat());

				if (!nextAvailableTime.HasValue)
					return "No Availability";

				return string.Format("Available at {0}", nextAvailableTime.Value.ToWcsHoursMinutesFormat());
			}

		}

		public bool CanAllowUpdatingStatus
		{
			get { return _canAllowUpdatingStatus; }
			set
			{
				_canAllowUpdatingStatus = value;
				this.DoRaisePropertyChanged(() => CanAllowUpdatingStatus, RaisePropertyChanged);
			}
		}

		public ScreenSelectionType? SelectionType
		{
			get { return _selectionType; }
			set
			{
				_selectionType = value;
				this.DoRaisePropertyChanged(() => SelectionType, RaisePropertyChanged);

				IsHighlighted = _selectionType.HasValue && _selectionType.Value == ScreenSelectionType.Selected;
			}
		}

		public RelayCommand ToggleNotesCommand
		{
			get { return new RelayCommand(DoToggleNotes); }
		}

		public RelayCommand ShowHistoryCommand
		{
			get { return new RelayCommand(DoShowHistory); }
		}

		public RelayCommand ShowInfoCommand
		{
			get { return new RelayCommand(DoShowInfo); }
		}


		public int TimeToAvailability { 
			get
			{
				var times = OrderHeaderTimeline.TimelineItems.Where(ti => ti.TimelineType == TimelineItemType.FreeRoom).OfType<ITimeDefinition>().ToList();
				var nextAvailableTime = TimeDefinition.GetNextTime(times, DateTime.Now.TimeOfDay);
				if (!nextAvailableTime.HasValue) return Int32.MaxValue;
				return Math.Max(Convert.ToInt32((DateTime.Now.TimeOfDay - nextAvailableTime.Value).TotalMinutes),0);

			}
		}

		#region IDispose

		private volatile bool _disposed = false;

		protected override void Dispose(bool disposing)
		{
			if (!this._disposed)
			{
				_disposed = true;

				if (_timelineCoordinator != null)
				{
					_timelineCoordinator.Dispose();
					_timelineCoordinator = null;
				}
				if (_conversationNotesViewModel != null)
				{
					_conversationNotesViewModel.BedUpdateAvailable -= SynchroniseWithUpdate;
					_conversationNotesViewModel.Dispose();
					_conversationNotesViewModel = null;
				}
				if (_orderHeaderTimelineCoordinator != null)
				{
					_orderHeaderTimelineCoordinator.Dispose();
					_orderHeaderTimelineCoordinator = null;
				}
				if (_underlyingOrder != null)
				{
					_underlyingOrder.Dispose();
					_underlyingOrder = null;
				}

				base.Dispose(disposing);
			}
		}
		#endregion

		public void HideAllNotes()
		{
			ConversationNotesViewModel.DismissCommand.Execute(null);
		}
		private void DoTrySelect()
		{
			var ts = TrySelect;
			if (ts != null)
				ts.Invoke(this, SelectionType);
		}

		private void DoPopupBed()
		{
			if (_popupBedCallback != null)
				_popupBedCallback.Invoke(this);

			DoTrySelect();
		}

		internal void HidePopups()
		{
			ConversationNotesViewModel.DismissCommand.Execute(null);
		}

		private void DoMarkAsClean()
		{
			FlagAsAwaitingUpdate();

			var invoker = WcsViewModel.MefContainer.GetExportedValue<IWcsAsyncInvoker>();
			if (invoker != null)
				invoker.MarkBedAsCleanAsync(_bed.BedId, SynchroniseWithUpdate);

			DoDismissPopup();
		}

		private void DoMarkAsDirty()
		{
			FlagAsAwaitingUpdate();

			var invoker = WcsViewModel.MefContainer.GetExportedValue<IWcsAsyncInvoker>();
			if (invoker != null)
				invoker.MarkBedAsDirtyAsync(_bed.BedId, SynchroniseWithUpdate);

			DoDismissPopup();
		}

		private void DoToggleNotes()
		{
			if (_toggleNotesCallback != null)
				_toggleNotesCallback.Invoke();
		}

		private void DoShowHistory()
		{
			if (_toggleHistoryCallback != null)
				_toggleHistoryCallback.Invoke();
		}

		private void DoShowInfo()
		{
			if (_toggleInfoCardCallback != null)
				_toggleInfoCardCallback.Invoke();
		}

		internal void HandleMinuteTimerTick()
		{
			this.DoRaisePropertyChanged(() => IsAvailableNowForCleaning, RaisePropertyChanged);
			this.DoRaisePropertyChanged(() => StatusText, RaisePropertyChanged);
		}


		public void DoDismissPopup()
		{
			if (_dismissPopupCallback != null)
				_dismissPopupCallback.Invoke();
		}

		internal void ToggleBedSelection(BedViewModel bed)
		{
			SelectionType = SelectionType == ScreenSelectionType.DeSelected ? ScreenSelectionType.Selected : ScreenSelectionType.DeSelected;
		}

		internal void ClearBedsSelectionType()
		{
			SelectionType = null;
		}
	}
}

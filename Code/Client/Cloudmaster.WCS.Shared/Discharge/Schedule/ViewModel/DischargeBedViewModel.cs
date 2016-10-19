using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Cloudmaster.WCS.DataServicesInvoker;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using GalaSoft.MvvmLight.Command;
using WCS.Core;
using WCS.Shared.Notes;
using WCS.Shared.Orders;
using WCS.Shared.Schedule;
using WCS.Shared.Timeline;

namespace WCS.Shared.Discharge.Schedule
{
	/// <summary>
	/// Handles all the manipulation and updating of a discharge for a bed/admission that appears in the schedule
	/// </summary>
	public class DischargeBedViewModel : UpdatingViewModel, ISynchroniseable<DischargeBedViewModel>
	{
		public event Action<DischargeBedViewModel> StartingManipulation;
		public event Action<DischargeBedViewModel> EndedManipulation;
		public event Action<DischargeBedViewModel, ScreenSelectionType?> TrySelect;
		public event Action ShouldRecalculateStatistics;
		private event Action<DateTime> RequestUpdateEstimatedDischargeTime;

		private BedDischargeData _discharge;
		private Action<DischargeBedViewModel> _popupCallback;
		private Action _dismissPopupCallback;
		private Action _toggleInfoCardCallback;
		private ConversationNotesViewModel _conversationNotesViewModel;

		private TimelineCoordinator _timelineCoordinator;

		private Action _toggleNotesCallback;
		private Action _toggleHistoryCallback;
		private DeviceInfoViewModel _wardLocationPresence;

		private string _hostLocation;
		private bool _isUpdating;
		private bool _canAllowUpdatingStatus;
		private bool _bedRequiresCleaning;
		private bool _isHighlighted;
		private ScreenSelectionType? _selectionType;
		
		private DateTime _estimatedTimeWhenStartToSlide;


		public DischargeBedViewModel(BedDischargeData discharge, Action<DischargeBedViewModel> popupCallback, Action dismissPopupCallback, Action toggleNotesCallback, Action toggleHistoryCallback, Action toggleInfoCardCallback)
		{
			_popupCallback = popupCallback;
			_dismissPopupCallback = dismissPopupCallback;
			_toggleNotesCallback = toggleNotesCallback;
			_toggleHistoryCallback = toggleHistoryCallback;
			_toggleInfoCardCallback = toggleInfoCardCallback;

			_discharge = discharge;

			TimelineCoordinator = new TimelineCoordinator(TimelineItemType.NoteIn | TimelineItemType.NoteOut | TimelineItemType.ProcedureTimeUpdated | TimelineItemType.OrderAssigned | TimelineItemType.PatientOccupied | TimelineItemType.CleaningService | TimelineItemType.FreeRoom | TimelineItemType.BedMarkedAsDirty | TimelineItemType.Discharge);
			TimelineCoordinator.Location = _hostLocation;

			//TimelineCoordinator.Synchronise(discharge);

			//ConversationNotesViewModel = new ConversationNotesViewModel(_discharge);
			//ConversationNotesViewModel.BedUpdateAvailable += SynchroniseWithUpdate;

			WardLocationPresence = new DeviceInfoViewModel { Location = discharge.Location.Name, LocationFullName = discharge.Location.FullName, LocationType = DeviceInfoViewModel.Mode.Ward };

			//BedRequiresCleaning = discharge.CurrentStatus != BedStatus.Clean;

			CanAllowUpdatingStatus = true;
			SelectionType = null;

			// creates a Rx throttled source that manages out update procedure calls to the server
			var dischargeSource = Observable.FromEvent<DateTime>(ev => RequestUpdateEstimatedDischargeTime += ev, ev => RequestUpdateEstimatedDischargeTime -= ev).Throttle(TimeSpan.FromSeconds(4));
			_requestUpdateEstimatedDischargeTimeSource = dischargeSource.Subscribe(UpdateDischargeTime);
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

		/// <summary>
		/// Synchronises the bed as well as signalling that the labels need updating 
		/// </summary>
		/// <param name="discharge">The bed</param>
		public void SynchroniseWithUpdate(BedDischargeData discharge)
		{
			MarkUpdateAsArrived();

			Synchronise(discharge);

			var srs = ShouldRecalculateStatistics;
			if (srs != null)
				srs.Invoke();
		}

		/// <summary>
		/// Set and resets all of the view models properties with a new or modified bed
		/// </summary>
		/// <param name="discharge">The bed</param>
		public void Synchronise(BedDischargeData discharge)
		{
			if (_disposed)
				return;

			if (discharge.BedId != _discharge.BedId)
				return;

			if (CheckIfUpdateIsPending) return;

			if (discharge.GetFingerprint() == _discharge.GetFingerprint())
				return;

			IsUpdating = true;

			_discharge = discharge;

			//_timelineCoordinator.Synchronise(discharge);

			//_conversationNotesViewModel.Synchronise(discharge);

			//BedRequiresCleaning = discharge.CurrentStatus != BedStatus.Clean;

			IsUpdating = false;

			this.DoRaisePropertyChanged(() => Id, RaisePropertyChanged);
			this.DoRaisePropertyChanged(() => Status, RaisePropertyChanged);
			this.DoRaisePropertyChanged(() => IsBedClean, RaisePropertyChanged);
			this.DoRaisePropertyChanged(() => BedRequiresCleaning, RaisePropertyChanged);
			this.DoRaisePropertyChanged(() => Bed, RaisePropertyChanged);
			this.DoRaisePropertyChanged(() => Room, RaisePropertyChanged);
			this.DoRaisePropertyChanged(() => LocationCode, RaisePropertyChanged);
			this.DoRaisePropertyChanged(() => MrsaRisk, RaisePropertyChanged);
			this.DoRaisePropertyChanged(() => HasAdmission, RaisePropertyChanged);
			this.DoRaisePropertyChanged(() => EstimatedDischargeTime, RaisePropertyChanged);
			this.DoRaisePropertyChanged(() => StatusText, RaisePropertyChanged);
		}


		/// <summary>
		/// Set and resets all of the view models properties with a new or modified bed
		/// </summary>
		/// <param name="bed">The bed</param>
		public void Synchronise(DischargeBedViewModel bed)
		{
			if (_disposed)
				return;

			if (bed.Id != _discharge.BedId)
				return;

			Task.Factory.StartNew(() => Synchronise(bed._discharge)).LogExceptionIfThrownAndIgnore();
		}

		public TimelineCoordinator TimelineCoordinator
		{
			get { return _timelineCoordinator; }
			private set { _timelineCoordinator = value; }
		}


		public int? PatientId
		{
			get { return null; }
		}


		public string SearchString
		{
			get
			{
				return _discharge.Name;
			}
		}

		public int Id
		{
			get { return _discharge.BedId; }
		}


		public bool MrsaRisk
		{
			get { return true; }
			//get { return _discharge.IsMrsaPositive; }
			//private set
			//{
			//    if (_discharge.IsMrsaPositive != value)
			//    {
			//        _discharge.IsMrsaPositive = value;
			//        this.DoRaisePropertyChanged(() => MrsaRisk, RaisePropertyChanged);
			//    }
			//}
		}

		public bool IsAvailableNowForCleaning
		{
			get { return true; }
			//get
			//{
			//    var items = OrderHeaderTimeline.TimelineItems.Where(ti => ti.TimelineType == TimelineItemType.FreeRoom).ToList();
			//    var times = items.Cast<ITimeDefinition>().ToList();
			//    return TimeDefinition.ContainsTime(times, DateTime.Now.TimeOfDay) && Status != BedStatus.Clean;
			//}
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

		public void HideAllNotes()
		{
			ConversationNotesViewModel.DismissCommand.Execute(null);
		}

		public string Bed
		{
			get { return _discharge.Name; }
		}

		public string Room
		{
			get { return _discharge.Room.Name; }
		}

		public string LocationCode
		{
			get { return _discharge.Location.Name; }
		}

		public BedStatus Status
		{
			get { return BedStatus.Clean; }
			//get
			//{
			//    return _discharge.CurrentStatus;
			//}
		}


		public bool IsBedClean
		{
			get { return true; }
			//get { return _discharge.CurrentStatus == BedStatus.Clean; }
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
				if (!HasAdmission) return "Empty Bed";
				if (EstimatedDischargeTime.HasValue) return String.Format("{0}", EstimatedDischargeTime.Value.TimeOfDay.ToOrderTimeFormat());
				return "No Discharge Time";
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

		public RelayCommand SetDischargeCommand
		{
			get { return new RelayCommand(DoSetDischarge); }
		}

		

		#region Manipulation

		#region Touch

		private TouchManipulation TouchManipulation { get; set; }

		public void StartTouchManipulation(double xInternalClickOffset, double startPosition, double timePixelRatio)
		{
			SignalStartManipulation();

			_estimatedTimeWhenStartToSlide = (EstimatedDischargeTime.HasValue ? EstimatedDischargeTime.Value : DateTime.Now).RoundDownToNearestHour();
			TouchManipulation = new TouchManipulation(xInternalClickOffset, startPosition, !EstimatedDischargeTime.HasValue);
		}

		internal void UpdateTouchManipulation(double x, double timePixelRatio)
		{
			SignalEndManipulation();

			TouchManipulation.Update(x);

			if (TouchManipulation.IsInitialDrag)
				EstimatedDischargeTime = DateTime.Now;
			else
			{
				if (TouchManipulation.DragDelta > 0.0)
				{
					var fromHours = TimeSpan.FromHours((TouchManipulation.DragDelta/timePixelRatio));
					var newtime = _estimatedTimeWhenStartToSlide.Add(fromHours);

					if ((newtime - DateTime.Now.RoundDownToNearestHour()).TotalHours < 12)
						EstimatedDischargeTime = newtime;
					else if ((newtime - DateTime.Now.RoundDownToNearestHour()).TotalHours < 13)
						EstimatedDischargeTime = DateTime.Now.RoundDownToNearestHour().AddDays(1);
					else if ((newtime - DateTime.Now.RoundDownToNearestHour()).TotalHours < 14)
						EstimatedDischargeTime = DateTime.Now.RoundDownToNearestHour().AddDays(2);
					else if ((newtime - DateTime.Now.RoundDownToNearestHour()).TotalHours < 15)
						EstimatedDischargeTime = DateTime.Now.RoundDownToNearestHour().AddDays(3);
				}
			}
		}


		public void ConfirmTouchManipulation()
		{
			TouchManipulation = null;

			//OrderCoordinator.Order.IsScheduled = true;

			SignalEndManipulation();

			UpdateEstimatedDischargeTime();
		}

		#endregion

		#region Mouse

		private MouseManipulation MouseManipulation { get; set; }

		public bool IsMouseManipulation { get { return MouseManipulation != null; } }

		public bool HasMouseManipulationMoved { get { return IsMouseManipulation && MouseManipulation.HasMoved; } }

		public void StartMouseManipulation(double xInternalClickOffset, double startPosition, double timePixelRatio)
		{
			SignalStartManipulation();

			_estimatedTimeWhenStartToSlide = (EstimatedDischargeTime.HasValue ? EstimatedDischargeTime.Value : DateTime.Now).RoundDownToNearestHour();

			MouseManipulation = new MouseManipulation(xInternalClickOffset, startPosition, !EstimatedDischargeTime.HasValue);
		}

		internal void UpdateMouseManipulation(double delta, double timePixelRatio)
		{
			MouseManipulation.Update(delta);

			if (MouseManipulation.IsInitialDrag)
				EstimatedDischargeTime = DateTime.Now;
			else
			{
				var fromHours = TimeSpan.FromHours((MouseManipulation.DragDelta / timePixelRatio));
				var newtime = _estimatedTimeWhenStartToSlide.Add(fromHours);

				if((newtime - DateTime.Now.RoundDownToNearestHour()).TotalHours < 12)
					EstimatedDischargeTime = newtime;
				else if ((newtime - DateTime.Now.RoundDownToNearestHour()).TotalHours < 13)
					EstimatedDischargeTime = DateTime.Now.RoundDownToNearestHour().AddDays(1);
				else if ((newtime - DateTime.Now.RoundDownToNearestHour()).TotalHours < 14)
					EstimatedDischargeTime = DateTime.Now.RoundDownToNearestHour().AddDays(2);
				else if ((newtime - DateTime.Now.RoundDownToNearestHour()).TotalHours < 15)
					EstimatedDischargeTime = DateTime.Now.RoundDownToNearestHour().AddDays(3);
			}
		}

		public void AbortMouseManipulation()
		{
			MouseManipulation = null;

			SignalEndManipulation();

			UpdateEstimatedDischargeTime();
		}

		public void ConfirmMouseManipulation()
		{
			MouseManipulation = null;
			
			SignalEndManipulation();

			UpdateEstimatedDischargeTime();
		}

		/// <summary>
		/// Makes a call to the server and updates the admission's discharge time. THis call takes the request and puts it into the throttle
		/// </summary>
		public void UpdateEstimatedDischargeTime()
		{
			FlagAsAwaitingUpdate(); 
				
			var ruedt = RequestUpdateEstimatedDischargeTime;
			if (ruedt != null && EstimatedDischargeTime.HasValue)
				ruedt(EstimatedDischargeTime.Value);
		}
		/// <summary>
		/// Makes a call to the server and updates the discharge with the local Estimated Time
		/// </summary>
		/// <param name="newEstimatedDischargeTime">The new estimated discharge time.</param>
		public void UpdateDischargeTime(DateTime newEstimatedDischargeTime)
		{
			Task.Factory.StartNew(() =>
			{
				var invoker = WcsViewModel.MefContainer.GetExportedValue<IWcsAsyncInvoker>();
				if (invoker != null)
				{
					FlagAsAwaitingUpdate();
					invoker.UpdateEstimatedDischargeDateAsync(_discharge.CurrentAdmission.AdmissionId, newEstimatedDischargeTime, SynchroniseWithUpdate);
				}
			}).LogExceptionIfThrownAndIgnore();
		}

		#endregion


		private void SignalStartManipulation()
		{
			var sm = StartingManipulation;
			if (sm != null)
				sm.Invoke(this);
		}

		private void SignalEndManipulation()
		{
			var em = EndedManipulation;
			if (em != null)
				em.Invoke(this);
		}

		#endregion

		#region IDispose

		private volatile bool _disposed = false;
		private IDisposable _requestUpdateEstimatedDischargeTimeSource;

		protected override void Dispose(bool disposing)
		{
			if (!this._disposed)
			{
				_disposed = true;

				if (_requestUpdateEstimatedDischargeTimeSource != null)
					_requestUpdateEstimatedDischargeTimeSource.Dispose();


				//if (_timelineCoordinator != null)
				//{
				//    _timelineCoordinator.Dispose();
				//    _timelineCoordinator = null;
				//}
				//if (_conversationNotesViewModel != null)
				//{
				//    _conversationNotesViewModel.BedUpdateAvailable -= SynchroniseWithUpdate;
				//    _conversationNotesViewModel.Dispose();
				//    _conversationNotesViewModel = null;
				//}

				base.Dispose(disposing);
			}
		}
		#endregion

		public int GetFingerprint()
		{
			return _discharge.GetFingerprint();
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

		private void DoTrySelect()
		{
			var ts = TrySelect;
			if (ts != null)
				ts.Invoke(this, SelectionType);
		}

		private void DoPopupBed()
		{
			if (_popupCallback != null)
				_popupCallback.Invoke(this);

			DoTrySelect();
		}

		internal void HidePopups()
		{
			ConversationNotesViewModel.DismissCommand.Execute(null);
		}

		public bool HasAdmission
		{
			get { return _discharge.CurrentAdmission != null; }
		}

		public DateTime? EstimatedDischargeTime
		{
			get
			{
				if (_discharge.CurrentAdmission == null) return null;
				return _discharge.CurrentAdmission.EstimatedDischargeDateTime;
			}
			set
			{
				if (value.HasValue)
					value = value.Value.RoundDownToNearestHour();
				
				_discharge.CurrentAdmission.EstimatedDischargeDateTime = value;

				this.DoRaisePropertyChanged(() => EstimatedDischargeTime, RaisePropertyChanged);
				this.DoRaisePropertyChanged(() => StatusText, RaisePropertyChanged);
			}
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

		private void DoSetDischarge()
		{
			UpdateDischargeTime(DateTime.Now);
		}
		
		internal void HandleMinuteTimerTick()
		{
			this.DoRaisePropertyChanged(() => EstimatedDischargeTime, RaisePropertyChanged);
			this.DoRaisePropertyChanged(() => IsAvailableNowForCleaning, RaisePropertyChanged);
			this.DoRaisePropertyChanged(() => StatusText, RaisePropertyChanged);
		}
 

		public void DoDismissPopup()
		{
			if (_dismissPopupCallback != null)
				_dismissPopupCallback.Invoke();
		}

		internal void ToggleBedSelection(DischargeBedViewModel bed)
		{
			SelectionType = SelectionType == ScreenSelectionType.DeSelected ? ScreenSelectionType.Selected : ScreenSelectionType.DeSelected;
		}

		internal void ClearBedsSelectionType()
		{
			SelectionType = null;
		}
	}
}

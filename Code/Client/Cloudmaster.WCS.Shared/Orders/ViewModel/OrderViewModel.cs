using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Cloudmaster.WCS.DataServicesInvoker;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using Cloudmaster.WCS.DataServicesInvoker.Types;
using GalaSoft.MvvmLight.Command;
using WCS.Core;
using WCS.Shared.Notes;
using WCS.Shared.Schedule;
using WCS.Shared.Ward.Schedule;
using System.Globalization;

namespace WCS.Shared.Orders
{

	/// <summary>
	/// Handles all the manipulation and updating of an item that appears in the schedule
	/// </summary>
	public class OrderViewModel : UpdatingViewModel, ISynchroniseable<OrderViewModel>
	{
		public event Action<OrderViewModel> StartingManipulation;
		public event Action<OrderViewModel> EndedManipulation;
		public event Action<Order, PatientViewModel> ItemRquiresSync;
		public event Action<OrderViewModel, ScreenSelectionType?> TrySelect;
		public event Action<OrderViewModel> ToggleTracking;
		public event Action ShouldRecalculateStatistics;
		private event Action<TimeSpan> RequestUpdateProcedureTime;

		private Order _order;
		private Action<PatientViewModel> _locatePatientCallback;
		private Action<OrderViewModel> _popupOrderCallback;
		private Action _dismissPopupCallback;
		private Action _toggleNotesCallback;
		private Action _toggleHistoryCallback;
		private Action _toggleInfoCardCallback;
		private Func<LocationSummary> _getDefaultLocationCallback;
		private ObservableCollection<INotificationItem> _notifications;
		private ConversationNotesViewModel _conversationNotesViewModel;
		private OrderItemViewModel _orderViewModel;
		private LocationClockViewModel _locationTimer;

		private List<TimeDefinition> _outPatientTimes;
		private bool _showTimelineSummary;
		private ScreenSelectionType? _selectionType;
		private bool _isUpdating;
		private bool _showAllOders;
		private bool _hasUnacknowledgedItems;
		private bool _hasUnacknowledgedPhysioNotification;
		private bool _isPrimaryOrder;
		private bool _showAvailabilityOverlay;
		private bool _showTracking;
		private TimeSpan? _lastSyncProcedureTime ;

		protected TimelineCoordinator _orderHeaderTimelineCoordinator;
		protected TimelineCoordinator _timelineCoordinator;
		protected OrderCoordinator _orderCoordinator;

		private NotificationType? _notificationTypeFilter;


		public OrderViewModel(Order order, PatientViewModel patient, Action<OrderViewModel> popupOrderCallback, Action dismissPopupCallback, Action<PatientViewModel> locatePatientCallback, Func<LocationSummary> getDefaultLocationCallback, Action toggleNotesCallback, Action toggleHistoryCallback, Action toggleInfoCardCallback, NotificationType? notificationTypeFilter, TimelineItemType timelineItems)
		{
			_locatePatientCallback = locatePatientCallback;
			_popupOrderCallback = popupOrderCallback;
			_dismissPopupCallback = dismissPopupCallback;
			_getDefaultLocationCallback = getDefaultLocationCallback;
			_notificationTypeFilter = notificationTypeFilter;
			_toggleNotesCallback = toggleNotesCallback;
			_toggleHistoryCallback = toggleHistoryCallback;
			_toggleInfoCardCallback = toggleInfoCardCallback;

			Patient = patient;

			_showTimelineSummary = true;
			_showAllOders = false;

			SelectionType = null;

			_order = order;

			OrderCoordinator = new OrderCoordinator();
			OrderCoordinator.OrderUpdateAvailable += SynchroniseWithUpdate;

			OrderHeaderTimeline = new TimelineCoordinator(TimelineItemType.PatientOccupied);
			OrderHeaderTimeline.Synchronise(_order, _notificationTypeFilter);

			TimelineCoordinator = new TimelineCoordinator(timelineItems);
			TimelineCoordinator.Synchronise(_order, _notificationTypeFilter);

			_orderViewModel = new OrderItemViewModel(_order, patient, DoDoConversationalNotes);
			_orderViewModel.ProcedureTimeChanged += HandleProcedureTimeChanged;

			// build the notifications
			var notifications = (_notificationTypeFilter.HasValue ? _order.Notifications.Where(n => n.NotificationType == _notificationTypeFilter.Value) : _order.Notifications).Select(n => new NotificationItemViewModel(n)).ToList();
			notifications.ForEach(n => n.OrderUpdateAvailable += SynchroniseWithUpdate);
			_notifications = new ObservableCollection<INotificationItem>(notifications);


			_notifications.OrderByDescending(n => n.PriorTime).ForEach(n => _orderCoordinator.AddNewAppointmentItem(n));
			_orderCoordinator.AddNewAppointmentItem(_orderViewModel);

			ConversationNotesViewModel = new ConversationNotesViewModel(_order);
			ConversationNotesViewModel.OrderUpdateAvailable += SynchroniseWithUpdate;

            if (_getDefaultLocationCallback.Invoke() != null)
			    LocationTimer = new LocationClockViewModel(_getDefaultLocationCallback.Invoke().WaitingRoomCode);

			// THis should be called to align the notifications start dates with the Order's startdate
			_orderCoordinator.Order.RaiseStartDateChanged();

			HasUnacknowledgedItems = GetUnacknowledgedItems.Where(i => i.AppointmentType == OrderScheduleItemType.Notification || i.AppointmentType == OrderScheduleItemType.Order).Any();
			HasUnacknowledgedPhysioNotification = GetUnacknowledgedItems.Where(i => i.AppointmentType == OrderScheduleItemType.PhysioNotification).Any();

			OutPatientTimes = new List<TimeDefinition>();

			ShowAvailabilityOverlay = false;
			IsPrimaryOrder = true;
			_lastSyncProcedureTime = OrderCoordinator.Order.StartTime;

            RequestUpdateProcedureTime += OnPreRequestUpdateProcedureTime;

			// creates a Rx throttled source that manages out update procedure calls to the server
			var orderSource = Observable.FromEvent<TimeSpan>(ev => RequestUpdateProcedureTime += ev, ev => RequestUpdateProcedureTime -= ev).Throttle(TimeSpan.FromSeconds(6));
			_requestUpdateProcedureTimeSource = orderSource.Subscribe(UpdateProcedureTime);
		}

	    

	    /// <summary>
		/// Synchronises the order as well as signalling that the labels need updating 
		/// </summary>
		/// <param name="order">The order</param>
		public void SynchroniseWithUpdate(Order order)
		{

			MarkUpdateAsArrived();

			Synchronise(order);


			var irs = ItemRquiresSync;
			if (irs != null)
				irs.Invoke(order, OrderCoordinator.Order.Patient);

			var srs = ShouldRecalculateStatistics;
			if (srs != null)
				srs.Invoke();
		}

		/// <summary>
		/// Set and resets all of the view models properties with a new or modified Order
		/// </summary>
		/// <param name="order">The order</param>
		public void Synchronise(Order order)
		{
			if (_disposed)
				return;

			if (order.OrderId != _order.OrderId)
				return;

			if (CheckIfUpdateIsPending) return;


			if (order.GetFingerprint() == _order.GetFingerprint())
				return;

			//var differences = Order.FindDifferences(order, _order);

			IsUpdating = true;

			// this bit makes sure that the update sound is only played for orders for this devices default location
			if (_getDefaultLocationCallback != null)
			{
				var defaultLocation = _getDefaultLocationCallback();
				if (defaultLocation != null)
				{
					if (defaultLocation.Code == order.DepartmentCode)
					{
						Sound.UpdateRecieved.Play();
					}
				}
			}
			_order = order;
		
			_orderCoordinator.Synchronise(order, _notificationTypeFilter);
			_timelineCoordinator.Synchronise(order, _notificationTypeFilter);
			_orderHeaderTimelineCoordinator.Synchronise(order, _notificationTypeFilter);

			_notifications.ForEach(n =>
									{
										if (!_orderCoordinator.AppointmentItems.Contains(n))
											_orderCoordinator.AddNewAppointmentItem(n);
									});
			_conversationNotesViewModel.Synchronise(order);

			HasUnacknowledgedItems = GetUnacknowledgedItems.Where(i => i.AppointmentType == OrderScheduleItemType.Notification || i.AppointmentType == OrderScheduleItemType.Order).Any();
			HasUnacknowledgedPhysioNotification = GetUnacknowledgedItems.Where(i => i.AppointmentType == OrderScheduleItemType.PhysioNotification).Any();

			_lastSyncProcedureTime = OrderCoordinator.Order.StartTime;

			Task.Factory.StartNew(() =>
									{
										new ManualResetEventSlim().Wait(1000);
										IsUpdating = false;
									}).LogExceptionIfThrownAndIgnore();

			this.DoRaisePropertyChanged(() => AdmissionStatusFlag, RaisePropertyChanged);
			this.DoRaisePropertyChanged(() => Id, RaisePropertyChanged);
			this.DoRaisePropertyChanged(() => OrderNumber, RaisePropertyChanged);
			this.DoRaisePropertyChanged(() => OrderDepartmentCode, RaisePropertyChanged);
			this.DoRaisePropertyChanged(() => OrderDepartmentName, RaisePropertyChanged);
			this.DoRaisePropertyChanged(() => IsHighPriority, RaisePropertyChanged);
			this.DoRaisePropertyChanged(() => IsHidden, RaisePropertyChanged);
			this.DoRaisePropertyChanged(() => OrderingDoctor, RaisePropertyChanged);
			this.DoRaisePropertyChanged(() => AdmittingDoctor, RaisePropertyChanged);
			this.DoRaisePropertyChanged(() => AttendingDoctor, RaisePropertyChanged);
			this.DoRaisePropertyChanged(() => AdmissionWardCode, RaisePropertyChanged);
			this.DoRaisePropertyChanged(() => AdmissionWardName, RaisePropertyChanged);
			this.DoRaisePropertyChanged(() => OrderDepartmentCode, RaisePropertyChanged);
			this.DoRaisePropertyChanged(() => OrderDepartmentName, RaisePropertyChanged);
			this.DoRaisePropertyChanged(() => AdmitDateTime, RaisePropertyChanged);

		}

		/// <summary>
		/// Set and resets all of the view models properties with a new or modified Order
		/// </summary>
		/// <param name="order">The order</param>
		public void Synchronise(OrderViewModel order)
		{
			if (_disposed)
				return;

			if (order.Id != _order.OrderId)
				return;

			if (order.GetFingerprint() == GetFingerprint())
				return;

			Synchronise(order._order);
		}

		/// <summary>
		/// Updates the order with RFID detections 
		/// </summary>
		/// <param name="detections">The order</param>
		public void Synchronise(IList<Detection> detections)
		{
			if (_disposed)
				return;

			_orderCoordinator.Synchronise(detections);

			if (detections.Any())
			{
				LocationTimer.SetLatestDetection(detections.OrderBy(d => d.Timestamp).Last());
			}
		}

		public int GetFingerprint()
		{
			return _order.GetFingerprint();
		}

		/// <summary>
		/// Propagates the out patient times to the Order. This should paint the order as black when the order is scheduled for that time
		/// </summary>
		/// <param name="times">The times.</param>
		internal void PropagatesOutPatientTimes(List<TimeDefinition> times)
		{
			OutPatientTimes = times;
		}

		public PatientViewModel Patient { get; set; }

		public int PatientId
		{
			get
			{
				return OrderCoordinator.Order.Patient.PatientId;
			}
		}

		public AdmissionType AdmissionType
		{
			get
			{
				return OrderCoordinator.Order.Patient.AdmissionType;
			}
		}

		public string SearchString
		{
			get
			{
				return OrderCoordinator.Order.Patient.SearchString;
			}
		}

		public int Id
		{
			get
			{
				if (_orderCoordinator == null) return -1;
				return _orderCoordinator.Order.Id;
			}
		}

		public string OrderDepartmentCode
		{
			get
			{
				if (_orderCoordinator == null) return "";
				return _order.DepartmentCode;
			}
		}
		public string OrderDepartmentName
		{
			get
			{
				if (_orderCoordinator == null) return "";
				return _order.DepartmentName;
			}
		}

		public string AdmissionWardCode
		{
			get
			{
				if (_orderCoordinator == null) return "";
				return _orderCoordinator.Order.WardCode;
			}
		}

		public string AdmissionWardName
		{
			get
			{
				if (_orderCoordinator == null) return "";
				return _orderCoordinator.Order.WardName;
			}
		}

		public void HideAllNotes()
		{
			ConversationNotesViewModel.DismissCommand.Execute(null);
		}

		public MultiSelectAdmissionStatusFlag AdmissionStatusFlag
		{
			get
			{
				if (_orderCoordinator == null) return MultiSelectAdmissionStatusFlag.Unknown;
				return OrderCoordinator.Order.AdmissionStatusFlag;
				//if (OrderCoordinator.Order.CompletedTime.HasValue)
				//    return AdmissionStatusFlag.Completed;
				//else if (!OrderCoordinator.Order.CompletedTime.HasValue && (!OrderCoordinator.Order.ProcedureTime.HasValue || OrderCoordinator.Order.ProcedureTime.Value.TimeOfDay.Hours < 5))
				//    return AdmissionStatusFlag.Unscheduled;
				////else if (!OrderCoordinator.Order.CompletedTime.HasValue)
				////    return Schedule.AdmissionStatusFlag.Arrived;
				////else if (!OrderCoordinator.Order.CompletedTime.HasValue && OrderCoordinator.Order.ProcedureTime.HasValue && OrderCoordinator.Order.ProcedureTime.Value.TimeOfDay.Hours >= 5)
				////    return Schedule.AdmissionStatusFlag.Waiting;
				//else
				//    return AdmissionStatusFlag.NotCompleted;
			}
		}

		public OrderStatus OrderStatus
		{
			get
			{
				if (_orderCoordinator == null) return OrderStatus.Completed;
				return OrderCoordinator.Order.OrderStatus;
			}
		}

		public OrderCoordinator OrderCoordinator
		{
			get { return _orderCoordinator; }
			private set { _orderCoordinator = value; }
		}

		public TimelineCoordinator OrderHeaderTimeline
		{
			get { return _orderHeaderTimelineCoordinator; }
			private set { _orderHeaderTimelineCoordinator = value; }
		}

		public TimelineCoordinator TimelineCoordinator
		{
			get { return _timelineCoordinator; }
			private set { _timelineCoordinator = value; }
		}

		public bool IsPrimaryOrder
		{
			get { return _isPrimaryOrder; }
			set
			{
				_isPrimaryOrder = value;
				this.DoRaisePropertyChanged(() => IsPrimaryOrder, RaisePropertyChanged);
				this.DoRaisePropertyChanged(() => IsReadOnly, RaisePropertyChanged);
			}
		}

		public bool IsReadOnly
		{
			get { return !IsPrimaryOrder || ((AdmissionType | AdmissionType.Out) == AdmissionType); }
		}

		public bool IsHighPriority
		{
			get { return _order.Admission.Location.IsEmergency; }
		}

		public bool IsHidden { get { return _order.IsHidden; } }


		public bool ShowTracking
		{
			get { return _showTracking; }
			set
			{
				_showTracking = value;
				this.DoRaisePropertyChanged(() => ShowTracking, RaisePropertyChanged);
			}
		}

		public bool ShowAllOders
		{
			get { return _showAllOders; }
			set
			{
				_showAllOders = value;
				this.DoRaisePropertyChanged(() => ShowAllOders, RaisePropertyChanged);
			}
		}

		public string OrderNumber
		{
			get
			{
				if (_orderCoordinator == null) return "";
				return _orderCoordinator.Order.OrderNumber;
			}
		}

		public RelayCommand ToggleAdditionalOrdersCommand
		{
			get { return new RelayCommand(DoToggleAdditionalOrders); }
		}


		public RelayCommand ToggleNotesCommand
		{
			get { return new RelayCommand(DoToggleNotes); }
		}
		public string OrderingDoctor
		{
			get { return _order.Admission.PrimaryDoctor; }
		}

		public string AdmittingDoctor
		{
            get { return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(_order.Admission.AdmittingDoctor ?? string.Empty); }
		}

		public string AttendingDoctor
		{
			get { return _order.Admission.AttendingDoctor; }
		}

        public string History
        {
            get { return CultureInfo.CurrentCulture.TextInfo.ToTitleCase((_order.History ?? "-").ToLower()); }
        }

        public string Diagnosis
        {
            get { return CultureInfo.CurrentCulture.TextInfo.ToTitleCase((_order.Diagnosis ?? "-").ToLower()); }
        }

        public string CurrentCardiologist
        {
            get { return CultureInfo.CurrentCulture.TextInfo.ToTitleCase((_order.CurrentCardiologist ?? string.Empty).ToLower()); }
        }

        public bool RequiresSupervision
        {
            get { return _order.RequiresSupervision; }
        }

        public bool RequiresFootwear
        {
            get { return _order.RequiresFootwear; }
        }

        public bool RequiresMedicalRecords
        {
            get { return _order.RequiresMedicalRecords; }
        }

		public DateTime AdmitDateTime
		{
			get { return _order.Admission.AdmitDateTime; }
		}

		public List<TimeDefinition> OutPatientTimes
		{
			get { return _outPatientTimes; }
			set
			{
				_outPatientTimes = value;
				this.DoRaisePropertyChanged(() => OutPatientTimes, RaisePropertyChanged);
			}
		}


		public RelayCommand HideOrderCommand
		{
			get { return new RelayCommand(DoHideOrder); }
		}

		public RelayCommand UnhideOrderCommand
		{
			get { return new RelayCommand(DoUnhideOrder); }
		}


		#region IDispose

		private volatile bool _disposed = false;
		private IDisposable _requestUpdateProcedureTimeSource;

		protected override void Dispose(bool disposing)
		{
			if (!this._disposed)
			{
				_disposed = true;

				// _orderViewModel is be disposed in the coordinator
				_orderViewModel.ProcedureTimeChanged -= HandleProcedureTimeChanged;
				_orderViewModel = null;

				if (_requestUpdateProcedureTimeSource != null)
					_requestUpdateProcedureTimeSource.Dispose();

				if (_orderCoordinator != null)
				{
					_orderCoordinator.OrderUpdateAvailable -= SynchroniseWithUpdate;

					_orderCoordinator.Dispose();
					_orderCoordinator = null;
				}
				if (_orderHeaderTimelineCoordinator != null)
				{
					_orderHeaderTimelineCoordinator.Dispose();
					_orderHeaderTimelineCoordinator = null;
				}
				if (_timelineCoordinator != null)
				{
					_timelineCoordinator.Dispose();
					_timelineCoordinator = null;
				}
				if (_conversationNotesViewModel != null)
				{
					_conversationNotesViewModel.OrderUpdateAvailable -= SynchroniseWithUpdate;
					_conversationNotesViewModel.Dispose();
					_conversationNotesViewModel = null;
				}

				if (_notifications != null)
				{
					_notifications.ForEach(n =>
					{
						if (n != null)
						{
							n.OrderUpdateAvailable -= SynchroniseWithUpdate;
							n.Dispose();
						}
					});
					_notifications.Clear();
					_notifications = null;
				}

				base.Dispose(disposing);
			}
		}
		#endregion

		public bool IsUpdating
		{
			get { return _isUpdating; }
			set
			{
				_isUpdating = value;
				this.DoRaisePropertyChanged(() => IsUpdating, RaisePropertyChanged);

			}
		}

		public bool ShowTimelineSummary
		{
			get { return _showTimelineSummary; }
			set
			{
				_showTimelineSummary = value;
				this.DoRaisePropertyChanged(() => ShowTimelineSummary, RaisePropertyChanged);
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

		public LocationClockViewModel LocationTimer
		{
			get { return _locationTimer; }
			set
			{
				_locationTimer = value;
				this.DoRaisePropertyChanged(() => LocationTimer, RaisePropertyChanged);
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

		public bool HasUnacknowledgedItems
		{
			get { return _hasUnacknowledgedItems; }
			set
			{
				_hasUnacknowledgedItems = value;
				this.DoRaisePropertyChanged(() => HasUnacknowledgedItems, RaisePropertyChanged);
			}
		}

		public bool HasUnacknowledgedPhysioNotification
		{
			get { return _hasUnacknowledgedPhysioNotification; }
			set
			{
				_hasUnacknowledgedPhysioNotification = value;
				this.DoRaisePropertyChanged(() => HasUnacknowledgedPhysioNotification, RaisePropertyChanged);
			}
		}


		public ObservableCollection<INotificationItem> Notifications
		{
			get { return _notifications; }
		}

		public RelayCommand AcknowledgeOrderAndPrepNotificationsCommand
		{
			get { return new RelayCommand(DoAcknowledgeOrderAndPrepNotifications); }
		}

		public RelayCommand AcknowledgePhysioNotificationCommand
		{
			get { return new RelayCommand(DoAcknowledgePhysioNotification); }
		}

		public RelayCommand LocatePatientCommand
		{
			get { return new RelayCommand(DoLocatePatient); }
		}

		public RelayCommand ShowInfoCommand
		{
			get { return new RelayCommand(DoShowInfo); }
		}

		public RelayCommand ShowHistoryCommand
		{
			get { return new RelayCommand(DoShowHistory); }
		}

		public RelayCommand ToggleTrackingCommand
		{
			get { return new RelayCommand(DoToggleTracking); }
		}

		public RelayCommand PopupOrderCommand
		{
			get { return new RelayCommand(DoPopupOrder); }
		}

		public RelayCommand TrySelectCommand
		{
			get { return new RelayCommand(DoTrySelect); }
		}

		public bool ShowAvailabilityOverlay
		{
			get { return _showAvailabilityOverlay; }
			set
			{
				_showAvailabilityOverlay = value;
				this.DoRaisePropertyChanged(() => ShowAvailabilityOverlay, RaisePropertyChanged);
			}
		}

		#region Manipulation

		#region Touch

		private TouchManipulation TouchManipulation { get; set; }

		public void StartTouchManipulation(double xInternalClickOffset, double startPosition, double timePixelRatio)
		{
			SignalStartManipulation();

			var needsInitialBoost = !_orderCoordinator.Order.ProcedureTime.HasValue || _orderCoordinator.Order.ProcedureTime.Value.TimeOfDay.Hours < 4;
			TouchManipulation = new TouchManipulation(xInternalClickOffset, startPosition, needsInitialBoost);
		}

		internal void UpdateTouchManipulation(double x, double timePixelRatio)
		{
			SignalEndManipulation();

			TouchManipulation.Update(x);

			var fromHours = TimeSpan.FromHours((TouchManipulation.DragDelta / timePixelRatio));
			if (TouchManipulation.IsInitialDrag)
				fromHours = fromHours.Add(new TimeSpan(4, 0, 0));

			OrderCoordinator.Order.StartTime = fromHours;

			//var invoker = WcsViewModel.MefContainer.GetExportedValue<IWcsAsyncInvoker>();
			//if (invoker != null)
			//    invoker.UpdateProcedureTimeAsync(OrderCoordinator.Order.Id,
			//        OrderCoordinator.Order.StartTime.HasValue ? OrderCoordinator.Order.StartTime.Value : default(TimeSpan),
			//        OrderCoordinator.Order.ProcedureTimeLastUpdated.HasValue ? OrderCoordinator.Order.ProcedureTimeLastUpdated.Value : default(DateTime),
			//        SynchroniseWithUpdate);
		}

		public void ConfirmTouchManipulation()
		{
			TouchManipulation = null;

			OrderCoordinator.Order.IsScheduled = true;

			SignalEndManipulation();

			UpdateProcedureTime();
		}

		#endregion

		#region Mouse

		private MouseManipulation MouseManipulation { get; set; }

		public bool IsMouseManipulation { get { return MouseManipulation != null; } }

		public bool HasMouseManipulationMoved { get { return IsMouseManipulation && MouseManipulation.HasMoved; } }

		public void StartMouseManipulation(double xInternalClickOffset, double startPosition, double timePixelRatio)
		{
			SignalStartManipulation();

			var needsInitialBoost = !_orderCoordinator.Order.ProcedureTime.HasValue || _orderCoordinator.Order.ProcedureTime.Value.TimeOfDay.Hours < 4;
			MouseManipulation = new MouseManipulation(xInternalClickOffset, startPosition, needsInitialBoost);
		}

		internal void UpdateMouseManipulation(double delta, double timePixelRatio)
		{
			MouseManipulation.Update(delta);

			var fromHours = TimeSpan.FromHours((MouseManipulation.DragDelta / timePixelRatio));
			if (MouseManipulation.IsInitialDrag)
				fromHours = fromHours.Add(new TimeSpan(4, 0, 0));

			OrderCoordinator.Order.StartTime = fromHours;
		}

		public void AbortMouseManipulation()
		{
			MouseManipulation = null;

			OrderCoordinator.Order.IsScheduled = true;

			SignalEndManipulation();

			UpdateProcedureTime();
		}

		public void ConfirmMouseManipulation()
		{
			MouseManipulation = null;

			OrderCoordinator.Order.IsScheduled = true;

			SignalEndManipulation();

			UpdateProcedureTime();
		}

		/// <summary>
		/// Makes a call to the server and updates the order with the local Procedure Time. THis call takes the request and puts it into the throttle
		/// </summary>
		public void UpdateProcedureTime()
		{
			var rupt = RequestUpdateProcedureTime;
			if (rupt != null && OrderCoordinator.Order.StartTime.HasValue)
				rupt(OrderCoordinator.Order.StartTime.Value);
		}

        private void OnPreRequestUpdateProcedureTime(TimeSpan newProcedureTime)
        {
            if (_lastSyncProcedureTime != newProcedureTime)
            {
                FlagAsAwaitingUpdate();
            }
        }

		/// <summary>
		/// Makes a call to the server and updates the order with the local Procedure Time
		/// </summary>
		/// <param name="newProcedureTime">The new procedure time.</param>
		public void UpdateProcedureTime(TimeSpan newProcedureTime)
		{
			Task.Factory.StartNew(() =>
			{
				var invoker = WcsViewModel.MefContainer.GetExportedValue<IWcsAsyncInvoker>();

				if (invoker != null)
				{
					 if (_lastSyncProcedureTime != newProcedureTime)
                     {
                        invoker.UpdateProcedureTimeAsync(OrderCoordinator.Order.Id, newProcedureTime, SynchroniseWithUpdate);
                    }
				}
			}).LogExceptionIfThrownAndIgnore();
		}

        /// <summary>
        /// Test call to mimic update procedure time, nessecary for unit testing against flickering and locking without making server calls
        /// </summary>
        public void UpdateProcedureTimeTest(Order order, int durationInMilliseconds)
        {
            Task.Factory.StartNew(() =>
                                      {
                                          FlagAsAwaitingUpdate();

                                          Thread.Sleep(durationInMilliseconds);

                                          SynchroniseWithUpdate(order);

                                      }).LogExceptionIfThrownAndIgnore();
        }

		#endregion

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

		/// <summary>
		/// Gets any unacknowledged items from the order 
		/// </summary>
		private List<IOrderItem> GetUnacknowledgedItems
		{
			get
			{
				if (Notifications == null || OrderCoordinator == null)
					return Enumerable.Empty<IOrderItem>().ToList();

				if (!OrderCoordinator.Order.StartTime.HasValue || OrderCoordinator.Order.StartTime.Value < TimeSpan.FromHours(5))
					return Enumerable.Empty<IOrderItem>().ToList();

				try
				{
					var items = Notifications.Where(n => !n.IsAcknowledged && n.StartTime.HasValue && n.StartTime < DateTime.Now.TimeOfDay).Select(s => s as IOrderItem).ToList();
					if (!OrderCoordinator.Order.IsAcknowledged && OrderCoordinator.Order.StartTime.HasValue && OrderCoordinator.Order.StartTime < DateTime.Now.TimeOfDay)
						items.Add(OrderCoordinator.Order);
					return items;
				}
				catch (Exception)
				{
					return Enumerable.Empty<IOrderItem>().ToList();
				}
			}
		}

		/// <summary>
		/// Handles the procedure time changed event from the orders do that it can set the start time in the notification
		/// </summary>
		/// <param name="newProcedureTime">The new procedure time.</param>
		private void HandleProcedureTimeChanged(DateTime? newProcedureTime)
		{
			FlagAsAwaitingUpdate();

			var chChaChaChaChanges = new List<Tuple<int, TimeSpan>>();

			if (newProcedureTime.HasValue)
			{
				Notifications.ForEach(n =>
										{
											n.AbsorbNewOrderProcedureTime(newProcedureTime.Value);
											chChaChaChaChanges.Add(new Tuple<int, TimeSpan>(n.Id, n.StartTime.Value));
										});
				chChaChaChaChanges.ForEach(cha => TimelineCoordinator.AbsorbNewNotificationStartTime(cha.Item1, cha.Item2));
			}

            MarkUpdateAsArrived();
		}


		private void DoTrySelect()
		{
			var ts = TrySelect;
			if (ts != null)
                ts.Invoke(this, SelectionType);
		}

		private void DoLocatePatient()
		{
			if (_locatePatientCallback != null)
				_locatePatientCallback.Invoke(OrderCoordinator.Order.Patient);
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

		private void DoToggleTracking()
		{
			ShowTracking = !ShowTracking;

			var tt = ToggleTracking;
			if (tt != null)
				tt.Invoke(this);
		}

		private void DoPopupOrder()
		{
			if (_popupOrderCallback != null)
				_popupOrderCallback.Invoke(this);
		}


		internal void HidePopups()
		{
			ConversationNotesViewModel.DismissCommand.Execute(null);
		}

		private void DoToggleAdditionalOrders()
		{
			ShowAllOders = !ShowAllOders;
		}
		private void DoToggleNotes()
		{
			if (_toggleNotesCallback != null)
				_toggleNotesCallback.Invoke();
		}

		private void DoHideOrder()
		{
			FlagAsAwaitingUpdate();

			var invoker = WcsViewModel.MefContainer.GetExportedValue<IWcsAsyncInvoker>();
			if (invoker != null)
				invoker.HideOrderAsync(_order.OrderId, SynchroniseWithUpdate);

		}
		private void DoUnhideOrder()
		{
			FlagAsAwaitingUpdate();

			var invoker = WcsViewModel.MefContainer.GetExportedValue<IWcsAsyncInvoker>();
			if (invoker != null)
				invoker.UnhideOrderAsync(_order.OrderId, SynchroniseWithUpdate);

		}


		/// <summary>
		/// Acknowledges all of the acknowledged notifications that need acknowledgment and the order is it need acknowledgment
		/// </summary>
		private void DoAcknowledgeOrderAndPrepNotifications()
		{
			FlagAsAwaitingUpdate();

			var invoker = WcsViewModel.MefContainer.GetExportedValue<IWcsAsyncInvoker>();
			if (invoker != null)
			{
				var unacknowledgedItems = GetUnacknowledgedItems;
				unacknowledgedItems.Where(ui => ui.AppointmentType == OrderScheduleItemType.Notification).ForEach(n => invoker.AcknowledgeNotificationAsync(n.Id, SynchroniseWithUpdate));
				unacknowledgedItems.Where(ui => ui.AppointmentType == OrderScheduleItemType.Order).ForEach(o => invoker.AcknowledgeOrderAsync(o.Id, SynchroniseWithUpdate));
			}
		}
		private void DoAcknowledgePhysioNotification()
		{
			var invoker = WcsViewModel.MefContainer.GetExportedValue<IWcsAsyncInvoker>();
			if (invoker != null)
			{
				var unacknowledgedItems = GetUnacknowledgedItems;
				unacknowledgedItems.Where(ui => ui.AppointmentType == OrderScheduleItemType.PhysioNotification).ForEach(n => invoker.AcknowledgeNotificationAsync(n.Id, SynchroniseWithUpdate));
			}
		}


		private void DoDoConversationalNotes()
		{
			if (ConversationNotesViewModel != null)
				ConversationNotesViewModel.ToggleNotesCommand.Execute(null);
		}

		public bool IsScheduled
		{
			get
			{
				return OrderCoordinator.Order.ProcedureTime.HasValue && (OrderCoordinator.Order.ProcedureTime.Value.TimeOfDay >= TimeSpan.FromHours(5)) && (OrderStatus != OrderStatus.Completed && OrderStatus != OrderStatus.Cancelled);
			}
		}

		internal bool HasOverdueUnacknowledgedNotifications
		{
			get
			{
				return HasUnacknowledgedItems;
				//return Notifications.Any(n => !n.IsAcknowledged && n.StartTime.HasValue && n.StartTime.Value <= DateTime.Now.TimeOfDay);
			}
		}

		internal void HandleMinuteTimerTick()
		{
			HasUnacknowledgedItems = GetUnacknowledgedItems.Where(i => i.AppointmentType == OrderScheduleItemType.Notification || i.AppointmentType == OrderScheduleItemType.Order).Any();
			HasUnacknowledgedPhysioNotification = GetUnacknowledgedItems.Where(i => i.AppointmentType == OrderScheduleItemType.PhysioNotification).Any();

			OrderCoordinator.HandleMinuteTimerTick();
			TimelineCoordinator.HandleMinuteTimerTick();
		}



	}
}

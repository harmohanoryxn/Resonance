using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using Cloudmaster.WCS.DataServicesInvoker.Types;
using GalaSoft.MvvmLight.Command;
using WCS.Core;
using WCS.Shared.Controls;
using WCS.Shared.Orders;
using WCS.Shared.Schedule;
using WCS.Shared.Ward.Schedule;

namespace WCS.Shared.Department.Schedule
{
	public class DepartmentScheduleViewModel : WcsScheduleViewModel<OrderViewModel>
	{
		public event Action SynchronisationComplete;

		private CollectionViewSource _timelineSource;

//		private OrderViewModel _selectedScheduleItem;

		private DepartmentPatientViewModel _selectedPatient;
		private OrderViewModel _selectedOrder;

		private ObservableCollection<LocationSummaryViewModel> _alternativeLocations;
		private LocationSummaryViewModel _selectedAltervativeLocation;

		private List<TimeDefinition> _outPatientTimes;

		private MultiSelectAdmissionStatusFlag _admissionStatusFlagFilter;
		private OrderStatus _orderStatusFilter;

		private OverlayType _overlayType;

		private int _allAdmissionType;
		private int _inAdmissionType;
		private int _outAdmissionType;
		private int _dayAdmissionType;
		private int _allScheduledPatients;
		private int _completedPatients;
		private int _notCompletedPatients;
		private int _unscheduledPatients;


		private bool _showInProgress;
		private bool _showCompleted;
		private bool _showCancelled;
		private bool _showHidden;

		private DepartmentPatientObservableCollection _scheduleItems;

		public DepartmentScheduleViewModel()
		{
			ScheduleItems = new DepartmentPatientObservableCollection(OnPatientToDepartmentViewModelTransform);
			ScheduleItems.RequestClearSelection += HandleRequestClearSelection;
			ScheduleItems.TrySelect += HandleChangeToItemSelection;
			ScheduleItems.ShouldRecalculateStatistics += HandleRequestToRecalculateStatistics;
			ScheduleItems.StartingManipulation += HandleStartingManipulation;
			ScheduleItems.EndedManipulation += HandleEndedManipulation;


			Application.Current.Dispatcher.InvokeIfRequired((() =>
				{
					TimelineSource = new CollectionViewSource();
                    TimelineSource.SortDescriptions.Add(new SortDescription("Patient.OrderCreated", ListSortDirection.Ascending));
					TimelineSource.Source = ScheduleItems;
				}));

			OutPatientTimes = new List<TimeDefinition>();

			_allAdmissionType = 0;
			_inAdmissionType = 0;
			_outAdmissionType = 0;
			_dayAdmissionType = 0;

			_allScheduledPatients = 0;
			_completedPatients = 0;
			_notCompletedPatients = 0;
			_unscheduledPatients = 0;

			OverlayType = OverlayType.OfficeHours;
            ScheduleItems.PatientAdmissionType = MultiSelectAdmissionTypeFlag.In;
			AdmissionStatusFlagFilter = AdmissionStatusFlagFilter.Set(MultiSelectAdmissionStatusFlag.Registered | MultiSelectAdmissionStatusFlag.Admitted);

			ShowInProgress = true;
		}

	    private DepartmentPatientViewModel OnPatientToDepartmentViewModelTransform(TopPatient p)
	    {
	        return new DepartmentPatientViewModel(p, ToggleRfidView, GetDefaultLocation, OnOrderToPatientViewModelTransform);
	    }

	    private OrderViewModel OnOrderToPatientViewModelTransform(Order o, PatientViewModel pvm)
	    {
	        return new OrderViewModel(o, pvm, Main.Popup, Main.DismissPopup, ToggleRfidView, GetDefaultLocation, ToggleNotesVisibility, ToggleHistoryVisibility, ToggleCardInfoVisibility, null, TimelineItemType.NoteIn | TimelineItemType.NoteOut | TimelineItemType.ProcedureTimeUpdated | TimelineItemType.OrderAssigned | TimelineItemType.OrderCompleted | TimelineItemType.NotificationAcknowlegement);
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

		public List<TimeDefinition> OutPatientTimes
		{
			get { return _outPatientTimes; }
			set
			{
				_outPatientTimes = value;
				this.DoRaisePropertyChanged(() => OutPatientTimes, RaisePropertyChanged);
			}
		}

		public DepartmentPatientObservableCollection ScheduleItems
		{
			get { return _scheduleItems; }
			set
			{
				_scheduleItems = value;
				this.DoRaisePropertyChanged(() => ScheduleItems, RaisePropertyChanged);
			}
		}

		#region Filtering

		#region Patient Name Filter

		private string _patientNameFilter;

		public string PatientNameFilter
		{
			get { return _patientNameFilter; }
			set
			{
				_patientNameFilter = value;
				this.DoRaisePropertyChanged(() => PatientNameFilter, RaisePropertyChanged);

				ScheduleItems.PatientNameFilter = _patientNameFilter;
			}
		}

		public RelayCommand ClearFilterCommand
		{
			get { return new RelayCommand(DoClearFilter); }
		}

		private void DoClearFilter()
		{
			PatientNameFilter = "";
		}

		#endregion

		public OverlayType OverlayType
		{
			get { return _overlayType; }
			set
			{
				_overlayType = value;
				this.DoRaisePropertyChanged(() => OverlayType, RaisePropertyChanged);
			}
		}

		public int AllAdmissionType
		{
			get { return _allAdmissionType; }
			set
			{
				_allAdmissionType = value;
				this.DoRaisePropertyChanged(() => AllAdmissionType, RaisePropertyChanged);
			}
		}

		public int InAdmissionType
		{
			get { return _inAdmissionType; }
			set
			{
				_inAdmissionType = value;
				this.DoRaisePropertyChanged(() => InAdmissionType, RaisePropertyChanged);
			}
		}

		public int OutAdmissionType
		{
			get { return _outAdmissionType; }
			set
			{
				_outAdmissionType = value;
				this.DoRaisePropertyChanged(() => OutAdmissionType, RaisePropertyChanged);
			}
		}

		public int DayAdmissionType
		{
			get { return _dayAdmissionType; }
			set
			{
				_dayAdmissionType = value;
				this.DoRaisePropertyChanged(() => DayAdmissionType, RaisePropertyChanged);
			}
		}

		public int AllScheduledPatients
		{
			get { return _allScheduledPatients; }
			set
			{
				_allScheduledPatients = value;
				this.DoRaisePropertyChanged(() => AllScheduledPatients, RaisePropertyChanged);
			}
		}
		public int CompletedPatients
		{
			get { return _completedPatients; }
			set
			{
				_completedPatients = value;
				this.DoRaisePropertyChanged(() => CompletedPatients, RaisePropertyChanged);
			}
		}

		public int NotCompletedPatients
		{
			get { return _notCompletedPatients; }
			set
			{
				_notCompletedPatients = value;
				this.DoRaisePropertyChanged(() => NotCompletedPatients, RaisePropertyChanged);
			}
		}

		public int UnscheduledPatients
		{
			get { return _unscheduledPatients; }
			set
			{
				_unscheduledPatients = value;
				this.DoRaisePropertyChanged(() => UnscheduledPatients, RaisePropertyChanged);
			}
		}

		#region Admission Status Filter


		public RelayCommand<string> FilterAdmissionStatusCommand
		{
			get { return new RelayCommand<string>(DoFilterScheduleStatus); }
		}

		private void DoFilterScheduleStatus(string filterParameter)
		{
			switch (filterParameter)
			{
				case "Admitted":
					AdmissionStatusFlagFilter = AdmissionStatusFlagFilter.Toggle(MultiSelectAdmissionStatusFlag.Admitted);
					break;
				case "Registered":
					AdmissionStatusFlagFilter = AdmissionStatusFlagFilter.Toggle(MultiSelectAdmissionStatusFlag.Registered);
					break;
				case "Discharged":
					AdmissionStatusFlagFilter = AdmissionStatusFlagFilter.Toggle(MultiSelectAdmissionStatusFlag.Discharged);
					break;
			}
		}

		public MultiSelectAdmissionStatusFlag AdmissionStatusFlagFilter
		{
			get { return _admissionStatusFlagFilter; }
			set
			{
				_admissionStatusFlagFilter = value;
				//if ((_admissionStatusFlagFilter | value) == _admissionStatusFlagFilter)
				//    _admissionStatusFlagFilter = _admissionStatusFlagFilter ^ value;
				//else
				//    _admissionStatusFlagFilter = _admissionStatusFlagFilter | value;

				ScheduleItems.AdmissionStatusFlagFilter = _admissionStatusFlagFilter;

				this.DoRaisePropertyChanged(() => AdmissionStatusFlagFilter, RaisePropertyChanged);
			}
		}

		#endregion

		#region Order Status Filter

		public bool ShowInProgress
		{
			get { return _showInProgress; }
			set
			{
				_showInProgress = value;
				this.DoRaisePropertyChanged(() => ShowInProgress, RaisePropertyChanged);

				if (_showInProgress)
					OrderStatusFilter = OrderStatus.InProgress;
			}
		}

		public bool ShowCompleted
		{
			get { return _showCompleted; }
			set
			{
				_showCompleted = value;
				this.DoRaisePropertyChanged(() => ShowCompleted, RaisePropertyChanged);

				if (_showCompleted)
					OrderStatusFilter = OrderStatus.Completed;
			}
		}

		public bool ShowCancelled
		{
			get { return _showCancelled; }
			set
			{
				_showCancelled = value;
				this.DoRaisePropertyChanged(() => ShowCancelled, RaisePropertyChanged);

				if (_showCancelled)
					OrderStatusFilter = OrderStatus.Cancelled;
			}
		}

		public bool ShowHidden
		{
			get { return _showHidden; }
			set
			{
				_showHidden = value;
				this.DoRaisePropertyChanged(() => ShowHidden, RaisePropertyChanged);

				ScheduleItems.ShowHiddenOverride = _showHidden;

			}
		}



		public OrderStatus OrderStatusFilter
		{
			get { return _orderStatusFilter; }
			set
			{
				//if ((_orderStatusFilter | value) == _orderStatusFilter)
				//    _orderStatusFilter = _orderStatusFilter ^ value;
				//else
				//    _orderStatusFilter = _orderStatusFilter | value;

				_orderStatusFilter = value;

				ScheduleItems.OrderStatusFilter = _orderStatusFilter;

				this.DoRaisePropertyChanged(() => OrderStatusFilter, RaisePropertyChanged);
			}
		}

		public RelayCommand<string> FilterOrderStatusCommand
		{
			get { return new RelayCommand<string>(DoFilterOrderStatus); }
		}

		private void DoFilterOrderStatus(string orderStatus)
		{
			switch (orderStatus)
			{
				case "inProgressStatus":
					ScheduleItems.OrderStatusFilter = OrderStatus.InProgress;
					break;
				case "completedStatus":
					ScheduleItems.OrderStatusFilter = OrderStatus.Completed;
					break;
				case "cancelledStatus":
					ScheduleItems.OrderStatusFilter = OrderStatus.Cancelled;
					break;
			}
		}

		#endregion

		public RelayCommand<string> FilterPatientCommand
		{
			get { return new RelayCommand<string>(DoFilterOnAdmissionType); }
		}

		private void DoFilterOnAdmissionType(string wcsAdmissionType)
		{
			switch (wcsAdmissionType)
			{
				case "In":
                    ScheduleItems.PatientAdmissionType = MultiSelectAdmissionTypeFlag.In;
					break;
				case "Out":
                    ScheduleItems.PatientAdmissionType = MultiSelectAdmissionTypeFlag.Out;
					break;
				case "Day":
                    ScheduleItems.PatientAdmissionType = MultiSelectAdmissionTypeFlag.Day;
					break;
				case "All":
			        ScheduleItems.PatientAdmissionType = MultiSelectAdmissionTypeFlag.All;
					break;
			}

            TurnAvailabilityOverlayOnForOrders((OverlayType | OverlayType.Availability) == OverlayType && ScheduleItems.PatientAdmissionType == MultiSelectAdmissionTypeFlag.In);
		}

		#endregion

		/// <summary>
		/// Synchronises the orders with potentially new ones from the server
		/// </summary>
		/// <param name="orders">The orders.</param>
		/// <param name="patientListCallback">The patient list callback.</param>
		public void Synchronise(IList<Order> orders, Action<List<string>> patientListCallback)
		{
			if (orders == null) return;
			if (SelectedLocation == null) return;

			List<TopPatient> patients = orders.GroupBy(o => o.Admission.Patient.PatientId, o => o).Select(g =>
				{
					var first = g.First();
					return new TopPatient(first.Admission.Patient, first.Admission, g.ToList());
				}).ToList();
			 
			//sync
			ScheduleItems.Synchronise(patients);

			//// have to count all visible departments to the user
			var allOrders = ScheduleItems.UnfilteredCollection.SelectMany(patient => patient.ScheduleItems.UnfilteredCollection).ToList();
			AlternativeLocations.ForEach(loc => loc.Synchronise(allOrders, o => o.OrderDepartmentCode != null && o.OrderDepartmentCode.CompareTo(loc.Code) == 0));

			LastSynchronised = ScheduleItems.LastSyncronized;

			allOrders.ForEach(order => order.IsPrimaryOrder = (order.OrderDepartmentCode == _selectedAltervativeLocation.Code));

			OutPatientTimes = allOrders.Where(order => (order.OrderDepartmentCode == _selectedAltervativeLocation.Code) && ((order.AdmissionType | AdmissionType.Out) == order.AdmissionType) && order.OrderCoordinator.Order.ProcedureTime.HasValue).Select(si => new TimeDefinition(si.OrderCoordinator.Order.ProcedureTime.Value.TimeOfDay, si.OrderCoordinator.Order.Duration)).ToList();
			allOrders.ForEach(order => order.PropagatesOutPatientTimes(OutPatientTimes));

			UpdateStatistics();

			if (patientListCallback != null)
				patientListCallback(patients.Select(p => p.Patient.IPeopleNumber).ToList());
	
			var sc = SynchronisationComplete;
			if (sc != null)
				sc.Invoke();

		}

		/// <summary>
		/// Synchronises the RFID detections with potentially new ones from the server
		/// </summary>
		/// <param name="detections">The orders.</param>
		public void Synchronise(IList<Detection> detections)
		{
			if (detections == null) return;

			ScheduleItems.UnfilteredCollection.Cast<DepartmentPatientViewModel>().ForEach(p =>
																				{
																					var localDetections = detections.Where(d => d.PatientId == p.PatientId).ToList();
																					p.Synchronise(localDetections);
																				});
		}

		/// <summary>
		/// Synchronises the ward statuses with potentially new ones from the server
		/// </summary>
		/// <param name="locationConnections">A complete set of up-to-date ward statuses</param>
		public void Synchronise(IEnumerable<Presence> locationConnections)
		{
			ScheduleItems.AsParallel().ForEach(patient =>
			{
				patient.Synchronise(locationConnections);
			});
		}

		/// <summary>
		/// Updates the amounts for all the different patient types on the schedule
		/// </summary>
		protected override void UpdateStatistics()
		{
			var depts = _selectedAltervativeLocation.Code == "All" ? AlternativeLocations.Where(d => d.Code != "All").Select(d => d.Code) : new[] { _selectedAltervativeLocation.Code };

			var visibleOrders = (from order in ScheduleItems.UnfilteredCollection.SelectMany(ufc => ufc.ScheduleItems)
								 join department in depts on order.OrderDepartmentCode equals department
								 select order).ToList();


			UnscheduledPatients = visibleOrders.Count(o => o.AdmissionStatusFlag.IsSet(MultiSelectAdmissionStatusFlag.Discharged));
		}


		//public OrderViewModel SelectedScheduleItem
		//{
		//    get { return _selectedScheduleItem; }
		//    set
		//    {
		//        _selectedScheduleItem = value;
		//        this.DoRaisePropertyChanged(() => SelectedScheduleItem, RaisePropertyChanged);
		//    }
		//}

		public LocationSummaryViewModel SelectedLocation
		{
			get { return _selectedAltervativeLocation; }
			set
			{
				_selectedAltervativeLocation = value;
				this.DoRaisePropertyChanged(() => SelectedLocation, RaisePropertyChanged);

				ScheduleItems.Location = value.Code;

				ScheduleItems.ForEach(si => si.SelectedLocation = value);
				//	RefreshAppointmentsForWard();

				UpdateStatistics();

			}
		}

		public DepartmentPatientViewModel SelectedPatient
		{
			get { return _selectedPatient; }
			set
			{
				_selectedPatient = value;
				this.DoRaisePropertyChanged(() => SelectedPatient, RaisePropertyChanged);
			}
		}

		public OrderViewModel SelectedOrder
		{
			get { return _selectedOrder; }
			set
			{
				_selectedOrder = value;
				this.DoRaisePropertyChanged(() => SelectedOrder, RaisePropertyChanged);
			}
		}

		/// <summary>
		/// Resets the selected location back to the default location
		/// </summary>
		internal void ResetDefaultWard()
		{
			if (DefaultLocation != SelectedLocation)
			{
				Application.Current.Dispatcher.BeginInvoke(new Action(() =>
				{
					SelectedLocation = DefaultLocation;
				}));

			}
		}

		/// <summary>
		/// Resets the order status to InProcess if it is deselected
		/// </summary>
		internal void ResetDefaultOrderStatus()
		{
			if (OrderStatusFilter == OrderStatus.Completed || OrderStatusFilter == OrderStatus.Cancelled)
			{
				Application.Current.Dispatcher.BeginInvokeIfRequired((() =>
				{
					ShowInProgress = true;
				}));

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


		public RelayCommand<string> ToggleAdditionalOrdersCommand
		{
			get { return new RelayCommand<string>(DoToggleAdditionalOrders); }
		}
		public RelayCommand ToggleAvailabilityOverlayCommand
		{
			get { return new RelayCommand(DoToggleAvailabilityOverlay); }
		}
		public RelayCommand ToggleOfficeHoursOverlayCommand
		{
			get { return new RelayCommand(DoToggleOfficeHoursOverlay); }
		}


		private void DoToggleAdditionalOrders(string externalPatientID)
		{
			ScheduleItems.ForEach(patient => patient.DoToggleAdditionalOrders(patient.Patient.IPeopleNumber == externalPatientID));
		}


		private void DoToggleAvailabilityOverlay()
		{
			ToggleOverlay(OverlayType.Availability);

            var inOn = (OverlayType | OverlayType.Availability) == OverlayType && (ScheduleItems.PatientAdmissionType.Equals(MultiSelectAdmissionTypeFlag.In));
			TurnAvailabilityOverlayOnForOrders(inOn);
		}

		private void TurnAvailabilityOverlayOnForOrders(bool inOn)
		{
			ScheduleItems.ForEach(si => si.ShowAvailabilityOverlay = inOn);
		}

		private void DoToggleOfficeHoursOverlay()
		{
			ToggleOverlay(OverlayType.OfficeHours);
		}

		/// <summary>
		/// Toggles the overlay on or off when a user selected or deselects it
		/// </summary>
		/// <param name="overlayType">Type of the overlay.</param>
		private void ToggleOverlay(OverlayType overlayType)
		{
			if ((OverlayType | overlayType) == OverlayType)
				OverlayType = OverlayType ^ overlayType;
			else
				OverlayType = OverlayType | overlayType;
		}

		//public void RefreshAppointmentsForWard()
		//{
		////	Source.View.HitServerForData();
		//    ScheduleItems.Filter();
		//}

		public override void HandleLockedEvent()
		{
			SelectedLocation = DefaultLocation;
			PatientNameFilter = "";
			ScheduleItems.ForEach(o => o.HidePopups());
		}

		internal void HandleNewWardSelection(LocationSummaryViewModel location)
		{
			SelectedLocation = location;
		}

		public override IEnumerable<string> GetAlertMessages()
		{
			var messages = new List<string>();

			var unassignedOrders = ScheduleItems.SelectMany(i => i.ScheduleItems).Where(a => !a.IsScheduled);
			var defaultLocationUnassignedOrders = unassignedOrders.Where(order => String.CompareOrdinal(DefaultLocation.Code, order.OrderDepartmentCode) == 0);

			if (defaultLocationUnassignedOrders.Any())
			{
				messages.Add(string.Format("{0}", defaultLocationUnassignedOrders.Count()));
			}

			return messages;
		}

		protected void HandleRequestClearSelection()
		{

			SelectedOrder = null;
			SelectedPatient = null;

			ShowActionBar = false;
			ClearOrdersSelectionType();
		}

		protected override void HandleChangeToItemSelection(OrderViewModel order, ScreenSelectionType? selectionType)
		{
			if (!selectionType.HasValue && SelectedOrder == null)
			{
				SelectedOrder = order;
				SelectedPatient = ScheduleItems.FirstOrDefault(p => p.ScheduleItems.UnfilteredCollection.Contains(order));

				ShowActionBar = true;
				ToggleOrderSelection(order);
			}
			else if (selectionType.HasValue && selectionType.Value == ScreenSelectionType.DeSelected)
			{
				SelectedOrder = null;
				SelectedPatient = null;

				ShowActionBar = false;
				ClearOrdersSelectionType();
			}
		}

		public override void Tombstone()
		{
			lock (ScheduleItems.SyncRoot)
			{
				ScheduleItems.ForEach(o => o.HideAllNotes());
			}
		}
		protected override void ClearAll()
		{
			lock (ScheduleItems.SyncRoot)
			{
				ScheduleItems.ClearAll();
			}
		}

		protected override void HandleMinuteTimerTick()
		{
            HandleCancelSelection();

			lock (ScheduleItems.SyncRoot)
			{
				ScheduleItems.ForEach(o => o.HandleMinuteTimerTick());
			}
		}
		 

		protected override void HandleCancelSelection()
		{
			ShowActionBar = false;
			ShowRfidPanel = false;
			ShowNotesPanel = false;
			ShowHistoryPanel = false;
			ShowCardPanel = false;

			SelectedOrder = null;
			SelectedPatient = null;
	
			ScheduleItems.ClearOrdersSelectionType();
		}

		internal void ToggleOrderSelection(OrderViewModel order)
		{
			ScheduleItems.ToggleOrderSelection(order);
		}

		internal void ClearOrdersSelectionType()
		{
			ScheduleItems.ClearOrdersSelectionType();
		}
	}
}


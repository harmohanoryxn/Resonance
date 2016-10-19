using System;
using System.Collections.Generic;
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
	/// <summary>
	/// Describes  a patient and all it's orders
	/// </summary>
	public class DepartmentPatientViewModel : BasePatientViewModel, ISynchroniseable<DepartmentPatientViewModel>, IComparer<DepartmentPatientViewModel>
	{
		public event Action<OrderViewModel, ScreenSelectionType?> TrySelectOrder;

		private CollectionViewSource _timelineSource;
		private DepartmentOrderObservableCollection _scheduleItems;

		//private OrderViewModel _selectedScheduleItem;

		private LocationSummaryViewModel _selectedAltervativeLocation;

		private List<TimeDefinition> _outPatientTimes;

		private MultiSelectAdmissionStatusFlag _admissionStatusFlagFilter;
		private OrderStatus _orderStatusFilter;

		private OrderViewModel _selectedOrder;
		private OverlayType _overlayType;
		private ScreenSelectionType? _selectionType;

		private bool _showInProgress;
		private bool _showCompleted;
		private bool _showCancelled;

		private bool _showHidden;
		private bool _showAllOders;

		public DepartmentPatientViewModel(TopPatient patient, Action<PatientViewModel> locatePatientCallback, Func<LocationSummary> getDefaultLocationCallback, Func<Order, PatientViewModel, OrderViewModel> transformFunction)
			: base(patient, locatePatientCallback, getDefaultLocationCallback)
		{
			ScheduleItems = new DepartmentOrderObservableCollection(transformFunction);
			ScheduleItems.TrySelect += HandleTrySelectOrder;
			ScheduleItems.ToggleTracking += HandleToggleTracking;

			Synchronise(patient);

            if (Application.Current != null)
            {
                Application.Current.Dispatcher.InvokeIfRequired((() =>
                                                                     {
                                                                         TimelineSource = new CollectionViewSource();
                                                                         TimelineSource.SortDescriptions.Add(
                                                                             new SortDescription("IsPrimaryOrder",
                                                                                                 ListSortDirection.
                                                                                                     Descending));
                                                                         TimelineSource.Source = ScheduleItems;
                                                                     }));
            }
            else
            {
                TimelineSource = new CollectionViewSource();
                TimelineSource.SortDescriptions.Add(
                    new SortDescription("IsPrimaryOrder",
                                        ListSortDirection.
                                            Descending));
                TimelineSource.Source = ScheduleItems;
            }

		    SelectedLocation = new LocationSummaryViewModel(getDefaultLocationCallback(), true);

			OutPatientTimes = new List<TimeDefinition>();

			SelectionType = null;
			OverlayType = OverlayType.OfficeHours;
            ScheduleItems.PatientAdmissionType = MultiSelectAdmissionTypeFlag.In;
			AdmissionStatusFlagFilter = AdmissionStatusFlagFilter.Set(MultiSelectAdmissionStatusFlag.Registered | MultiSelectAdmissionStatusFlag.Admitted);
			ShowInProgress = true;
			_showHidden = false;
			_showAllOders = false;
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

		public bool ShowAllOders
		{
			get { return _showAllOders; }
			set
			{
				_showAllOders = value;
				this.DoRaisePropertyChanged(() => ShowAllOders, RaisePropertyChanged);
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


		internal void DoToggleAdditionalOrders(bool isTarget)
		{
			//if (!ShowAllOders && isTarget)
			ShowAllOders = !ShowAllOders && isTarget;

			ScheduleItems.AsParallel().ForEach(order =>
												{
													order.ShowAllOders = ShowAllOders;
												});
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

		public DepartmentOrderObservableCollection ScheduleItems
		{
			get { return _scheduleItems; }
			set
			{
				_scheduleItems = value;
				this.DoRaisePropertyChanged(() => ScheduleItems, RaisePropertyChanged);
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
				{
					OrderStatusFilter = OrderStatus.InProgress;
					ScheduleItems.OrderStatusFilter = OrderStatus.InProgress;
				}
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
				{
					OrderStatusFilter = OrderStatus.Completed;
					ScheduleItems.OrderStatusFilter = OrderStatus.Completed;
				}
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
				{
					OrderStatusFilter = OrderStatus.Cancelled;
					ScheduleItems.OrderStatusFilter = OrderStatus.Cancelled;
				}
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

		#region Synchronise

		/// <summary>
		/// Synchronises the patient
		/// </summary>
		/// <param name="patient">The patient</param>
		public void Synchronise(DepartmentPatientViewModel patient)
		{
			if (_disposed)
				return;

			if (patient.Id != CorePatient.Patient.PatientId)
				return;

			if (patient.GetFingerprint() == GetFingerprint())
				return;

			Synchronise(patient.CorePatient);
		}

		/// <summary>
		/// Synchronises the underlying patient.
		/// </summary>
		/// <param name="patient">The patient.</param>
		public void Synchronise(TopPatient patient)
		{
			if (_disposed)
				return;

			if (patient.Patient.PatientId != CorePatient.Patient.PatientId)
				return;

			AdmissionStatusFlag = patient.Admission.AdmissionStatusFlag;
			WardCode = patient.Admission.Location.Name;
			WardName = patient.Admission.Location.FullName;

			ScheduleItems.Synchronise(patient.Orders, Patient);

			WardLocationPresence.Synchronise(patient);
		}

		/// <summary>
		/// Synchronises the RFID detections with potentially new ones from the server
		/// </summary>
		/// <param name="detections">The orders.</param>
		public void Synchronise(IList<Detection> detections)
		{
			if (detections == null) return;

			Application.Current.Dispatcher.BeginInvokeIfRequired((() =>
			{
				var localDetections = detections.Where(d => d.PatientId == Patient.PatientId).ToList();

				if (localDetections.Any())
				{
					Patient.Synchronise(localDetections);
					
					LocationTimer.SetLatestDetection(localDetections.OrderBy(d => d.Timestamp).Last());
				}
			}));
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
					if (locations.ContainsKey(order.OrderCoordinator.Order.DepartmentLocationPresence.Location))
					{
						order.OrderCoordinator.Order.DepartmentLocationPresence.Synchronise(locations[order.OrderDepartmentCode]);
					}
					if (locations.ContainsKey(WardLocationPresence.Location))
					{
						WardLocationPresence.Synchronise(locations[Patient.Location]);
					}
				});
			}
		}

		/// <summary>
		/// Synchronises the bed. Retired functionality
		/// </summary>
		/// <param name="bed">The bed.</param>
		internal void Synchronise(Bed bed)
		{

		}

		#endregion
		 
		public IEnumerable<string> GetAlertMessages()
		{
			var messages = new List<string>();

			// Unscheduled ScheduleItems
			var unassignedOrders = ScheduleItems.Where(a => !a.IsScheduled);

			var defaultLocationUnassignedOrders = unassignedOrders.Where(order => String.CompareOrdinal(GetDefaultLocationCallback().Code, order.OrderDepartmentCode) == 0);

			if (defaultLocationUnassignedOrders.Any())
			{
				//	string postFix = (defaultLocationUnassignedOrders.Count() > 1) ? "s" : string.Empty;

				//messages.Add(string.Format("{0} Unscheduled Order{1}", defaultLocationUnassignedOrders.Count(), postFix));
				messages.Add(string.Format("{0}", defaultLocationUnassignedOrders.Count()));
			}


			return messages;
		}

		public void Tombstone()
		{
			lock (ScheduleItems.SyncRoot)
			{
				ScheduleItems.ForEach(o => o.HideAllNotes());
			}
		}
		protected void ClearAllOrders()
		{
			lock (ScheduleItems.SyncRoot)
			{
				ScheduleItems.ClearAll();
			}
		}


		private void TurnAvailabilityOverlayOnForOrders(bool inOn)
		{
			ScheduleItems.AsParallel().ForEach(si => si.ShowAvailabilityOverlay = inOn);
		}

		public LocationSummaryViewModel SelectedLocation
		{
			get { return _selectedAltervativeLocation; }
			set
			{
				_selectedAltervativeLocation = value;
				
				ScheduleItems.ForEach(order =>
										{
											order.IsPrimaryOrder = order.OrderDepartmentCode == value.Code;
										});

                ScheduleItems.Location = value.Code;

                this.DoRaisePropertyChanged(() => SelectedLocation, RaisePropertyChanged);
			}
		}

		public bool ShowAvailabilityOverlay
		{
			set
			{
				ScheduleItems.AsParallel().ForEach(o => o.ShowAvailabilityOverlay = value);
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

		internal void HandleTrySelectOrder(OrderViewModel order, ScreenSelectionType? existingSelectionType)
		{
			var tso = TrySelectOrder;
			if (tso != null)
				tso.Invoke(order, existingSelectionType);
		}

		internal void ToggleOrderSelection(OrderViewModel order)
		{
			SelectedOrder = order;
			ScheduleItems.SetOrderAsSelected(order);
			SelectionType = ScheduleItems.Any(o => o.SelectionType == ScreenSelectionType.Selected)
				? ScreenSelectionType.Selected : ScreenSelectionType.DeSelected;
		}

		internal void ClearOrdersSelectionType()
		{
			SelectedOrder = null;
			ScheduleItems.ClearOrdersSelectionType();
			SelectionType = null;
		}

		#endregion

		internal void HandleToggleTracking(OrderViewModel order)
		{
			ShowTracking = !ShowTracking;
		}


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


		public int Compare(DepartmentPatientViewModel x, DepartmentPatientViewModel y)
		{
			return x.Id.CompareTo(y.Id);
		}
	}

}

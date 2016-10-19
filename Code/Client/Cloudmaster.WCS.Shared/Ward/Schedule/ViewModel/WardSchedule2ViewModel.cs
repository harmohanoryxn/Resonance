using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using Cloudmaster.WCS.DataServicesInvoker.Types;
using GalaSoft.MvvmLight.Command;
using WCS.Shared.Controls;
using WCS.Shared.Orders;
using WCS.Shared.Schedule;

namespace WCS.Shared.Ward.Schedule
{
	public class WardScheduleViewModel : WcsScheduleViewModel<OrderViewModel>
	{
		private CollectionViewSource _timelineSource;

		private WardPatientObservableCollection _scheduleItems;

		private ObservableCollection<LocationSummaryViewModel> _alternativeLocations;
		private LocationSummaryViewModel _selectedAltervativeLocation;

		private WardPatientViewModel _selectedPatient;
		private OrderViewModel _selectedOrder;

		private string _wardTitle;
		private int _completedPatients;
		private int _notCompletedPatients;
		private int _unscheduledPatients;
		private OrderStatus _orderStatusFilter;

		private bool _showInProgress;
		private bool _showCompleted;
		private bool _showCancelled;
		private bool _showHidden;
		private bool _sortByName;
		private bool _sortByRoom;

		public event Action SynchronisationComplete;
		public WardScheduleViewModel()
		{
			ScheduleItems = new WardPatientObservableCollection((p) => new WardPatientViewModel(p, ToggleRfidView, GetDefaultLocation, (o, pvm) => new OrderViewModel(o, pvm, Main.Popup, Main.DismissPopup, ToggleRfidView, GetDefaultLocation, ToggleNotesVisibility, ToggleHistoryVisibility, ToggleCardInfoVisibility, NotificationType.Prep, TimelineItemType.NoteIn | TimelineItemType.NoteOut | TimelineItemType.ProcedureTimeUpdated | TimelineItemType.OrderAssigned | TimelineItemType.OrderCompleted | TimelineItemType.NotificationAcknowlegement)));
			ScheduleItems.TrySelect += HandleChangeToItemSelection;
			ScheduleItems.ShouldRecalculateStatistics += HandleRequestToRecalculateStatistics;

			Application.Current.Dispatcher.InvokeIfRequired((() =>
			{
				TimelineSource = new CollectionViewSource();
				TimelineSource.SortDescriptions.Add(new SortDescription("Patient.FamilyName", ListSortDirection.Ascending));
				TimelineSource.Source = ScheduleItems;
			}));

			_completedPatients = 0;
			_notCompletedPatients = 0;
			_unscheduledPatients = 0;

			ShowInProgress = true;
			SortByRoom = true;
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

		public WardPatientObservableCollection ScheduleItems
		{
			get { return _scheduleItems; }
			set
			{
				_scheduleItems = value;
				this.DoRaisePropertyChanged(() => ScheduleItems, RaisePropertyChanged);
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
					ScheduleItems.Location = _selectedAltervativeLocation.Code;

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

		#region Filters

		#region Order Status Filter

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

		#endregion

		#region Sort Filter

		public RelayCommand<string> SortCommand
		{
			get { return new RelayCommand<string>(DoSortCommand); }
		}

		private void DoSortCommand(string byWhat)
		{
			switch (byWhat)
			{
				case "By Name":
					Application.Current.Dispatcher.InvokeIfRequired((() =>
					                                                 	{
					                                                 		SortByName = true;
					                                                 		SortByRoom = false;
				TimelineSource.SortDescriptions.Clear();
				TimelineSource.SortDescriptions.Add(new SortDescription("Patient.FamilyName", ListSortDirection.Ascending));
				TimelineSource.View.Refresh();
			}));
					break;
				case "By Room":
					Application.Current.Dispatcher.InvokeIfRequired((() =>
					{
						SortByRoom = true;
						SortByName = false;
						TimelineSource.SortDescriptions.Clear();
						TimelineSource.SortDescriptions.Add(new SortDescription("Patient.Room", ListSortDirection.Ascending));
						TimelineSource.View.Refresh();
					}));
					break;
			}
		}


		public bool SortByName
		{
			get { return _sortByName; }
			set
			{
				_sortByName = value;
				this.DoRaisePropertyChanged(() => SortByName, RaisePropertyChanged);
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

			var allOrders = ScheduleItems.UnfilteredCollection.SelectMany(patient => patient.ScheduleItems.UnfilteredCollection).ToList();
			AlternativeLocations.ForEach(loc => loc.Synchronise(allOrders, o => o.AdmissionWardCode != null && o.AdmissionWardCode.CompareTo(loc.Code) == 0));

			LastSynchronised = ScheduleItems.LastSyncronized;

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

			//Application.Current.Dispatcher.BeginInvokeIfRequired((() =>
			//{
			lock (ScheduleItems.SyncRoot)
			{
				ScheduleItems.UnfilteredCollection.Cast<WardPatientViewModel>().ForEach(o =>
				{
					var localDetections = detections.Where(d => d.PatientId == o.PatientId).ToList();
					o.Synchronise(localDetections);
				});
			}
			//}));
		}

		/// <summary>
		/// Synchronises the ward statuses with potentially new ones from the server
		/// </summary>
		/// <param name="locationConnections">A complete set of up-to-date ward statuses</param>
		public void Synchronise(IEnumerable<Presence> locationConnections)
		{
            lock (ScheduleItems.SyncRoot)
            {
                ScheduleItems.AsParallel().ForEach(patient =>
                                                       {
                                                           patient.Synchronise(locationConnections);

                                                       });
            }

            //catch (InvalidOperationException ex)
            //{
            // TODO: When filtering an invalidoperationException can occur as another thread is modifying scheduleItems. Sync lock should protect against this
            //}
		}
            


		/// <summary>
		/// Updates the amounts for all the different patient types on the schedule
		/// </summary>
		protected override void UpdateStatistics()
		{
		}


		public WardPatientViewModel SelectedPatient
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

		/// <summary>
		/// Resets the sort back to By Name
		/// </summary>
		internal void ResetDefaultSort()
		{
			if (!SortByName)
			{
				Application.Current.Dispatcher.BeginInvokeIfRequired((() =>
				{
					DoSortCommand("By Name");
				}));

			}
		}

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

			var unackOrders = ScheduleItems.SelectMany(patient => patient.ScheduleItems).Count(order => order.HasUnacknowledgedItems);
			if (unackOrders > 0)
			{

				//string postFix = (unackOrders > 1) ? "s" : string.Empty;

				//messages.Add(string.Format("{0} Overdue Order{1}", unackOrders, postFix));
				messages.Add(string.Format("{0}", unackOrders));
			}

			return messages;
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

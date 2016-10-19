using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using Cloudmaster.WCS.DataServicesInvoker.Types;
using GalaSoft.MvvmLight.Command;
using WCS.Shared.Controls;
using WCS.Shared.Orders;
using WCS.Shared.Schedule;

namespace WCS.Shared.Ward.Schedule
{
	/// <summary>
	/// Describes  a  Ward patient and all it's orders
	/// </summary>
	public class WardPatientViewModel : BasePatientViewModel, ISynchroniseable<WardPatientViewModel>, IComparer<WardPatientViewModel>
	{
		public event Action<OrderViewModel, ScreenSelectionType?> TrySelectOrder;

		private WardOrderObservableCollection _scheduleItems;
		//private OrderViewModel _selectedScheduleItem;
		private CollectionViewSource _timelineSource;

		private LocationSummaryViewModel _selectedAltervativeLocation;

		private OrderStatus _orderStatusFilter;
		private ScreenSelectionType? _selectionType;
		private OrderViewModel _selectedOrder;
		
		private bool _showInProgress;
		private bool _showCompleted;
		private bool _showCancelled;

		private int _numberOfUpdates;
		private int _numberOfNewOrders;

		private bool _showHidden;

		public WardPatientViewModel(TopPatient patient, Action<PatientViewModel> locatePatientCallback, Func<LocationSummary> getDefaultLocationCallback, Func<Order, PatientViewModel, OrderViewModel> transformFunction)
			: base(patient, locatePatientCallback, getDefaultLocationCallback)
		{
			ScheduleItems = new WardOrderObservableCollection(transformFunction);
			ScheduleItems.TrySelect += HandleTrySelectOrder;
			ScheduleItems.ToggleTracking += HandleToggleTracking;

			Synchronise(patient);

			Application.Current.Dispatcher.InvokeIfRequired((() =>
			{
				TimelineSource = new CollectionViewSource();
				TimelineSource.SortDescriptions.Add(new SortDescription("IsPrimaryOrder", ListSortDirection.Descending));
				TimelineSource.Source = ScheduleItems;
			}));

			SelectedLocation = new LocationSummaryViewModel(getDefaultLocationCallback(),true);
			SelectionType = null;
			
			ShowInProgress = true;
			_showHidden = false; 

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
		 
		public  WardOrderObservableCollection ScheduleItems
		{
			get { return _scheduleItems; }
			set
			{
				_scheduleItems = value;
				this.DoRaisePropertyChanged(() => ScheduleItems, RaisePropertyChanged);
			}
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

		public ScreenSelectionType? SelectionType
		{
			get { return _selectionType; }
			set
			{
				_selectionType = value;
				this.DoRaisePropertyChanged(() => SelectionType, RaisePropertyChanged);
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

		public LocationSummaryViewModel SelectedLocation
		{
			get { return _selectedAltervativeLocation; }
			set
			{
				_selectedAltervativeLocation = value;
				this.DoRaisePropertyChanged(() => SelectedLocation, RaisePropertyChanged);

				// This will filter the appointments
				ScheduleItems.Location =   _selectedAltervativeLocation.Code ;
			}
		}

		#endregion

		#region Synchronise
		 
		/// <summary>
		/// Synchronises the patient
		/// </summary>
		/// <param name="patient">The patient</param>
		public void Synchronise(WardPatientViewModel patient)
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
		/// Synchronises the underlyging patient.
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

		public  IEnumerable<string> GetAlertMessages()
		{
			var messages = new List<string>();

			// Unscheduled ScheduleItems
			var unassignedOrders = ScheduleItems.Where(a => !a.IsScheduled);

			var defaultLocationUnassignedOrders = unassignedOrders.Where(order => String.CompareOrdinal(GetDefaultLocationCallback().Code, order.AdmissionWardCode) == 0);

			if (defaultLocationUnassignedOrders.Any())
			{
				//	string postFix = (defaultLocationUnassignedOrders.Count() > 1) ? "s" : string.Empty;

				//messages.Add(string.Format("{0} Unscheduled Order{1}", defaultLocationUnassignedOrders.Count(), postFix));
				messages.Add(string.Format("{0}", defaultLocationUnassignedOrders.Count()));
			}


			return messages;
		}


		public  IEnumerable<string> GetUpdateMessages()
		{
			if (_numberOfUpdates > 0)
			{
				return new[] { _numberOfUpdates.ToString() };
			}
			return Enumerable.Empty<string>();

		}

		public  IEnumerable<string> GetOkMessages()
		{
			if (_numberOfNewOrders > 0)
			{
				return new[] { _numberOfNewOrders.ToString() };
			}
			return Enumerable.Empty<string>();
		}
		 
		public  void Tombstone()
		{
			lock (ScheduleItems.SyncRoot)
			{
				ScheduleItems.ForEach(o => o.HideAllNotes());
			}
		}

		protected  void ClearAllOrders()
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


		public int Compare(WardPatientViewModel x, WardPatientViewModel y)
		{
			return x.Id.CompareTo(y.Id);
		}
	}
	 
}

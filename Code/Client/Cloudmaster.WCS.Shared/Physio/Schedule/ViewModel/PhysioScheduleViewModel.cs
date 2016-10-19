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
using WCS.Shared.Ward.Schedule;

namespace WCS.Shared.Physio.Schedule
{
	public class PhysioScheduleViewModel : WcsScheduleViewModel<OrderViewModel>
	{
		private CollectionViewSource _timelineSource;
		private PhysioPatientObservableCollection _scheduleItems;
		private PhysioPatientViewModel _selectedPatient;
		private OrderViewModel _selectedOrder;

		public event Action SynchronisationComplete;

		private OrderStatus _orderStatusFilter;
		private bool _showInProgress;
		private bool _showCompleted;
		private bool _showCancelled;
		private bool _showHidden;

		public PhysioScheduleViewModel()
		{
			ScheduleItems = new PhysioPatientObservableCollection((p) => new PhysioPatientViewModel(p, ToggleRfidView, GetDefaultLocation, (o, pvm) => new OrderViewModel(o, pvm, Main.Popup, Main.DismissPopup, ToggleRfidView, GetDefaultLocation, ToggleNotesVisibility, ToggleHistoryVisibility, ToggleCardInfoVisibility, NotificationType.Physio, TimelineItemType.NoteIn | TimelineItemType.NoteOut | TimelineItemType.ProcedureTimeUpdated | TimelineItemType.OrderAssigned | TimelineItemType.OrderCompleted | TimelineItemType.NotificationAcknowlegement)));
			ScheduleItems.TrySelectOrder += HandleChangeToItemSelection;

			Application.Current.Dispatcher.BeginInvokeIfRequired((() =>
			{
				TimelineSource = new CollectionViewSource();
				TimelineSource.SortDescriptions.Add(new SortDescription("Patient.FamilyName", ListSortDirection.Ascending));
				TimelineSource.Source = ScheduleItems;
			}));

			ShowInProgress = true;

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

		public PhysioPatientObservableCollection ScheduleItems
		{
			get { return _scheduleItems; }
			set
			{
				_scheduleItems = value;
				this.DoRaisePropertyChanged(() => ScheduleItems, RaisePropertyChanged);
			}
		}

		public PhysioPatientViewModel SelectedPatient
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

		public RelayCommand<string> ToggleAdditionalOrdersCommand
		{
			get { return new RelayCommand<string>(DoToggleAdditionalOrders); }
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
		#endregion

		/// <summary>
		/// Synchronises the orders with potentially new ones from the server
		/// </summary>
		/// <param name="orders">The orders.</param>
		/// <param name="patientListCallback">The patient list callback.</param>
		public void Synchronise(IList<Order> orders, Action<List<string>> patientListCallback)
		{
			if (orders == null) return;

			List<TopPatient> patients = orders.GroupBy(o => o.Admission.Patient.PatientId, o => o).Select(g =>
			{
				var first = g.First();
				return new TopPatient(first.Admission.Patient, first.Admission, g.ToList());
			//}).ToList();
			}).Where(p=>p.Patient.IsAssistanceRequired).ToList();

		
			//sync
			ScheduleItems.Synchronise(patients);

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

			lock (ScheduleItems.SyncRoot)
			{
				ScheduleItems.UnfilteredCollection.Cast<PhysioPatientViewModel>().ForEach(patient =>
				{
					var localDetections = detections.Where(d => d.PatientId == patient.PatientId).ToList();
					patient.Synchronise(localDetections);
				});
			}
		}

		/// <summary>
		/// Synchronises the Physio statuses with potentially new ones from the server
		/// </summary>
		/// <param name="locationConnections">A complete set of up-to-date Physio statuses</param>
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
		}

		public override void HandleLockedEvent()
		{
			ScheduleItems.AsParallel().ForEach(o => o.HidePopups());
		}

		public override IEnumerable<string> GetAlertMessages()
		{
			var messages = new List<string>();

			var unackOrders = ScheduleItems.SelectMany(patient => patient.ScheduleItems).Count(order => order.HasUnacknowledgedPhysioNotification);
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

		private void DoToggleAdditionalOrders(string externalPatientID)
		{
			ScheduleItems.ForEach(patient => patient.DoToggleAdditionalOrders(patient.Patient.IPeopleNumber == externalPatientID));
		}
	}
}

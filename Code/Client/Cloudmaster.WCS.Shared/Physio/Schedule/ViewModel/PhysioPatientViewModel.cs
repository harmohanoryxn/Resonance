using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using Cloudmaster.WCS.DataServicesInvoker.Types;
using WCS.Shared.Controls;
using WCS.Shared.Orders;
using WCS.Shared.Schedule;
using WCS.Shared.Ward.Schedule;

namespace WCS.Shared.Physio.Schedule
{
	/// <summary>
	/// Describes  a physio patient and all it's orders
	/// </summary>
	public class PhysioPatientViewModel : BasePatientViewModel, ISynchroniseable<PhysioPatientViewModel>, IComparer<PhysioPatientViewModel>
	{
		public event Action<OrderViewModel, ScreenSelectionType?> TrySelectOrder;

		private PhysioOrderObservableCollection _scheduleItems;
		//private OrderViewModel _selectedScheduleItem;
		private CollectionViewSource _timelineSource;

		private OrderViewModel _selectedOrder;
		private ScreenSelectionType? _selectionType;
		private bool _showAllOders;

		public PhysioPatientViewModel(TopPatient patient, Action<PatientViewModel> locatePatientCallback, Func<LocationSummary> getDefaultLocationCallback, Func<Order, PatientViewModel, OrderViewModel> transformFunction)
			: base(patient, locatePatientCallback, getDefaultLocationCallback)
		{
			ScheduleItems = new PhysioOrderObservableCollection(transformFunction);
			ScheduleItems.TrySelect += HandleTrySelectOrder;
			ScheduleItems.ToggleTracking += HandleToggleTracking;

			Synchronise(patient);

            if (Application.Current != null)
            {
                Application.Current.Dispatcher.BeginInvokeIfRequired((() =>
                                                                          {
                                                                              TimelineSource =
                                                                                  new CollectionViewSource();
                                                                              TimelineSource.SortDescriptions.Add(
                                                                                  new SortDescription(
                                                                                      "OrderCoordinator.Order.StartTime",
                                                                                      ListSortDirection.Ascending));
                                                                              TimelineSource.Source = ScheduleItems;
                                                                          }));
            }
            else
            {
                TimelineSource = new CollectionViewSource();
                TimelineSource.SortDescriptions.Add(
                    new SortDescription(
                        "OrderCoordinator.Order.StartTime",
                        ListSortDirection.Ascending));
                TimelineSource.Source = ScheduleItems;
            }

		    SelectionType = null;

			_showAllOders = false;
			ShowTracking = false;
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

		public PhysioOrderObservableCollection ScheduleItems
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

		public bool ShowAllOders
		{
			get { return _showAllOders; }
			set
			{
				_showAllOders = value;
				this.DoRaisePropertyChanged(() => ShowAllOders, RaisePropertyChanged);
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

		internal void DoToggleAdditionalOrders(bool isTarget)
		{
			ShowAllOders = !ShowAllOders && isTarget;

			ScheduleItems.AsParallel().ForEach(order =>
			{
				order.ShowAllOders = ShowAllOders;
			});
		}

		#region Synchronise

		/// <summary>
		/// Synchronises the patient
		/// </summary>
		/// <param name="patient">The patient</param>
		public void Synchronise(PhysioPatientViewModel patient)
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

			var localDetections = detections.Where(d => d.PatientId == Patient.PatientId).ToList();

			if (localDetections.Any())
			{
				Patient.Synchronise(localDetections);
			}
		}

		/// <summary>
		/// Synchronises the Physio statuses with potentially new ones from the server
		/// </summary>
		/// <param name="locationConnections">A complete set of up-to-date Physio statuses</param>
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

		#endregion

		public IEnumerable<string> GetAlertMessages()
		{
			var messages = new List<string>();

			// Unscheduled ScheduleItems
			var unassignedOrders = ScheduleItems.Where(order => !order.HasUnacknowledgedPhysioNotification).ToList();

			if (unassignedOrders.Any())
			{
				//	string postFix = (defaultLocationUnassignedOrders.Count() > 1) ? "s" : string.Empty;

				//messages.Add(string.Format("{0} Unscheduled Order{1}", defaultLocationUnassignedOrders.Count(), postFix));
				messages.Add(string.Format("{0}", unassignedOrders.Count()));
			}

			return messages;
		}

		public IEnumerable<string> GetUpdateMessages()
		{
			return Enumerable.Empty<string>();
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
			Patient.LocationTrackingCoordinator.HandleMinuteTimerTick();
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

		#region Tracking

		internal void HandleToggleTracking(OrderViewModel order)
		{
			ShowTracking = !ShowTracking;
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

		public int Compare(PhysioPatientViewModel x, PhysioPatientViewModel y)
		{
			return x.Id.CompareTo(y.Id);
		}
	}

}

using System;
using System.Collections.Specialized;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using Cloudmaster.WCS.DataServicesInvoker.Types;
using WCS.Core;
using WCS.Shared.Department.Schedule;
using WCS.Shared.Orders;
using WCS.Shared.Schedule;

namespace WCS.Shared.Department
{
	public class DepartmentPatientObservableCollection : WcsScheduleItemObservableCollection2<DepartmentPatientViewModel, OrderViewModel, TopPatient, Order>
	{
		private string _location;
        private MultiSelectAdmissionTypeFlag _patientAdmissionType;
		private string _patientName;
		private MultiSelectAdmissionStatusFlag _admissionStatusFlagFilter;
		private OrderStatus _orderStatusFilter;
		private bool _showHiddenOverride;

		public event Action RequestClearSelection;
		public event Action<OrderViewModel, ScreenSelectionType?> TrySelect;
		public event Action<OrderViewModel> StartingManipulation;
		public event Action<OrderViewModel> EndedManipulation;

		public event Action ShouldRecalculateStatistics;

		public DepartmentPatientObservableCollection(Func<TopPatient, DepartmentPatientViewModel> transformFunction)
			: base(transformFunction)
		{
		    _patientAdmissionType = MultiSelectAdmissionTypeFlag.In;
			AdmissionStatusFlagFilter = AdmissionStatusFlagFilter.Set(MultiSelectAdmissionStatusFlag.Registered | MultiSelectAdmissionStatusFlag.Admitted);
			OrderStatusFilter = OrderStatus.InProgress;
		}

		/// <summary>
		/// Gets or sets the department.
		/// </summary>
		/// <value>
		/// Will filter on none if set to ""
		/// </value>
		public string Location
		{
			get { return _location; }
			set
			{
				if (_location != value)
				{
					_location = value;

					UnfilteredCollection.ForEach(patient => patient.ScheduleItems.Location = value);

					Filter();

                    // Hide related orders by default when switching views

                    UnfilteredCollection.ForEach(patient => patient.ShowAllOders = false);
				}
			}
		}

		/// <summary>
		/// Gets or sets the patient status.
		/// </summary>
		/// <value>
		/// Will filter on none if set to ""
		/// </value>
		public MultiSelectAdmissionTypeFlag PatientAdmissionType
		{
			get { return _patientAdmissionType; }
			set
			{
				if (_patientAdmissionType != value)
				{
					_patientAdmissionType = value;

					UnfilteredCollection.ForEach(patient => patient.ScheduleItems.PatientAdmissionType = value);

					Filter();

                    // Hide related orders by default when switching views

                    UnfilteredCollection.ForEach(patient => patient.ShowAllOders = false);
				}
			}
		}

		public string PatientNameFilter
		{
			get { return _patientName; }
			set
			{
				if (_patientName != value)
				{
					_patientName = value.ToUpper();

					UnfilteredCollection.ForEach(patient => patient.ScheduleItems.PatientNameFilter = value);

					Filter();

                    // Hide related orders by default when switching views

                    UnfilteredCollection.ForEach(patient => patient.ShowAllOders = false);
				}
			}
		}

		public MultiSelectAdmissionStatusFlag AdmissionStatusFlagFilter
		{
			get { return _admissionStatusFlagFilter; }
			set
			{
				if (!_admissionStatusFlagFilter.Equals(value))
				{
					_admissionStatusFlagFilter = value;

					UnfilteredCollection.ForEach(patient => patient.ScheduleItems.AdmissionStatusFlagFilter = value);

					Filter();

                    // Hide related orders by default when switching views

                    UnfilteredCollection.ForEach(patient => patient.ShowAllOders = false);
				}
			}
		}

		public OrderStatus OrderStatusFilter
		{
			get { return _orderStatusFilter; }
			set
			{
				if (!_orderStatusFilter.Equals(value))
				{
					_orderStatusFilter = value;

					UnfilteredCollection.ForEach(patient => patient.ScheduleItems.OrderStatusFilter = value);

                    Filter();

                    // Hide related orders by default when switching views

                    UnfilteredCollection.ForEach(patient => patient.ShowAllOders = false);
				}
			}
		}

		public bool ShowHiddenOverride
		{
			get { return _showHiddenOverride; }
			set
			{
				if (_showHiddenOverride != value)
				{
					_showHiddenOverride = value;

					UnfilteredCollection.ForEach(patient => patient.ScheduleItems.ShowHiddenOverride = value);

					Filter();

                    // Hide related orders by default when switching views

                    UnfilteredCollection.ForEach(patient => patient.ShowAllOders = false);
				}
			}
		}


		/// <summary>
		/// Filters this list on based on the department code
		/// </summary>
		public override List<DepartmentPatientViewModel> DoCustomFiltering(List<DepartmentPatientViewModel> items)
		{
            items = items.Where(patient => patient.ScheduleItems.UnfilteredCollection.Any(o => o.IsHidden == ShowHiddenOverride)).ToList();

            // Ignore the Order status filter if Hidden is selected

            if (!ShowHiddenOverride)
                items = items.Where(patient => patient.ScheduleItems.UnfilteredCollection.Any(o => (OrderStatusFilter | o.OrderStatus) == OrderStatusFilter)).ToList();

            items = items.Where(patient => patient.ScheduleItems.UnfilteredCollection.Any(o => ((PatientAdmissionType | o.AdmissionType.ToMultiSelectAdmissionTypeFlag()) == PatientAdmissionType))).ToList();

			if (!string.IsNullOrEmpty(PatientNameFilter))
				items = items.Where(patient => !string.IsNullOrEmpty(patient.SearchString) && patient.SearchString.Contains(PatientNameFilter)).ToList();

			items = items.Where(patient => (AdmissionStatusFlagFilter | patient.AdmissionStatusFlag) == AdmissionStatusFlagFilter).ToList();

			if (!string.IsNullOrEmpty(_location))
			{
				var patientIds = items.Where(patient => patient.ScheduleItems.Any(o => _location.Contains(o.OrderDepartmentCode))).Select(o => o.PatientId).ToList();
				items = items.Where(patient => patientIds.Contains(patient.PatientId)).ToList();
			}
			return items;

		}

		public override void OnAfterFiltering(List<DepartmentPatientViewModel> items)
		{
			// if after filtering - in the event that the selected patient is filtered away - we raise the RequestClearSelection to signal the container to 
			// clear any selections
			if(items.All(patient=>patient.SelectionType.HasValue && patient.SelectionType.Value == ScreenSelectionType.DeSelected))
			{
				var rcs = RequestClearSelection;
				if (rcs != null)
					rcs.Invoke();
			}
		}

		private void HandleTrySelectItem(OrderViewModel order, ScreenSelectionType? existingSelectionType)
		{
			var ts = TrySelect;
			if (ts != null)
				ts.Invoke(order, existingSelectionType);
		}

		/// <summary>
		/// Fires then an scheduleItem in the collection has invalidated statistics
		/// </summary>
		private void HandleRequestToRecalculateStatistics()
		{
			var srs = ShouldRecalculateStatistics;
			if (srs != null)
				srs.Invoke();
		}


		private void HandleStartingManipulation(OrderViewModel order)
		{
			var sm = StartingManipulation;
			if (sm != null)
				sm(order);
		}

		private void HandleEndedManipulation(OrderViewModel order)
		{
			var em = EndedManipulation;
			if (em != null)
				em(order);
		}

		public override void AttachItem(DepartmentPatientViewModel item)
		{
			item.ScheduleItems.TrySelect += HandleTrySelectItem;
			item.ScheduleItems.ItemRquiresSync += Synchronise;
			item.ScheduleItems.ShouldRecalculateStatistics += HandleRequestToRecalculateStatistics;
			item.ScheduleItems.StartingManipulation += HandleStartingManipulation;
			item.ScheduleItems.EndedManipulation += HandleEndedManipulation;
			item.ScheduleItems.CollectionChanged += HandleOrdersChanged;
		}

		public override void DetachItem(DepartmentPatientViewModel item)
		{
			item.ScheduleItems.TrySelect -= HandleTrySelectItem;
			item.ScheduleItems.ItemRquiresSync -= Synchronise;
			item.ScheduleItems.ShouldRecalculateStatistics -= HandleRequestToRecalculateStatistics;
			item.ScheduleItems.StartingManipulation -= HandleStartingManipulation;
			item.ScheduleItems.EndedManipulation -= HandleEndedManipulation;
			item.ScheduleItems.CollectionChanged -= HandleOrdersChanged;
		}


		/// <summary>
		/// Gets fired when a nested collection gets changed and might force the top level list to refilter
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
		private void HandleOrdersChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			Task.Factory.StartNew(Filter).LogExceptionIfThrownAndIgnore();
		}


		internal void ToggleOrderSelection(OrderViewModel order)
		{
			this.ForEach(patient => patient.ToggleOrderSelection(order));
		}

		internal void ClearOrdersSelectionType()
		{
			this.ForEach(patient => patient.ClearOrdersSelectionType());
		}
	}
}
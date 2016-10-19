using System;
using System.Linq;
using System.Collections.Generic;
using Cloudmaster.WCS.DataServicesInvoker;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using Cloudmaster.WCS.DataServicesInvoker.Types;
using WCS.Shared.Orders;
using WCS.Shared.Schedule;
using WCS.Shared.Ward.Schedule;

namespace WCS.Shared.Department
{
	public class DepartmentOrderObservableCollection : WcsScheduleItemObservableCollection3<PatientViewModel, OrderViewModel, Order>
	{
		public event Action<OrderViewModel, ScreenSelectionType?> TrySelect;
		public event Action<OrderViewModel> StartingManipulation;
		public event Action<OrderViewModel> EndedManipulation;
		public event Action<OrderViewModel> ToggleTracking;
		public event Action<TopPatient> ItemRquiresSync;

		private string _location;
        private MultiSelectAdmissionTypeFlag _patientAdmissionType;
		private string _patientName;
		private MultiSelectAdmissionStatusFlag _admissionStatusFlagFilter;
		private OrderStatus _orderStatusFilter;
		private bool _showHiddenOverride;

		public event Action ShouldRecalculateStatistics;

		public DepartmentOrderObservableCollection(Func<Order, PatientViewModel, OrderViewModel> transformFunction)
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
					Filter();
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
					Filter();
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
					Filter();
				}
			}
		}

		public MultiSelectAdmissionStatusFlag AdmissionStatusFlagFilter
		{
			get { return _admissionStatusFlagFilter; }
			set
			{
				if (_admissionStatusFlagFilter != value)
				{
					_admissionStatusFlagFilter = value;
					Filter();
				}
			}
		}

		public OrderStatus OrderStatusFilter
		{
			get { return _orderStatusFilter; }
			set
			{
				if (_orderStatusFilter != value)
				{
					_orderStatusFilter = value;
					Filter();
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
					_showHiddenOverride = value; ;
					Filter();
				}
			}
		}


		/// <summary>
		/// Filters this list on based on the department code
		/// </summary>
		public override List<OrderViewModel> DoCustomFiltering(List<OrderViewModel> items)
		{
			items = items.Where(o => o.IsHidden == ShowHiddenOverride).ToList();

            if (!ShowHiddenOverride)
                items = items.Where(o => (OrderStatusFilter | o.OrderStatus) == OrderStatusFilter).ToList();

            items = items.Where(o => (PatientAdmissionType.IsSet(o.AdmissionType.ToMultiSelectAdmissionTypeFlag()))).ToList();

			if (!string.IsNullOrEmpty(PatientNameFilter))
				items = items.Where(o => !string.IsNullOrEmpty(o.SearchString) && o.SearchString.Contains(PatientNameFilter)).ToList();

			items = items.Where(o => o.AdmissionStatusFlag.IsSet(AdmissionStatusFlagFilter)).ToList();

			if (!string.IsNullOrEmpty(_location))
			{
				var patientIds = items.Where(o => _location.Contains(o.OrderDepartmentCode)).Select(o => o.PatientId).ToList();
				items = items.Where(o => patientIds.Contains(o.PatientId)).ToList();
			}
			return items;

		}

		public override void AttachItem(OrderViewModel item)
		{
			item.TrySelect += HandleTrySelectItem;
			item.ShouldRecalculateStatistics += HandleRequestToRecalculateStatistics;
			item.StartingManipulation += HandleStartingManipulation;
			item.EndedManipulation += HandleEndedManipulation;
			item.ItemRquiresSync += HandleRequiresSync;
			item.ToggleTracking += HandleToggleTracking;
		}

		public override void DetachItem(OrderViewModel item)
		{
			item.TrySelect -= HandleTrySelectItem;
			item.ShouldRecalculateStatistics -= HandleRequestToRecalculateStatistics;
			item.StartingManipulation -= HandleStartingManipulation;
			item.EndedManipulation -= HandleEndedManipulation;
			item.ItemRquiresSync -= HandleRequiresSync;
			item.ToggleTracking -= HandleToggleTracking;
		}

		internal void HandleRequiresSync(Order order, PatientViewModel patient)
		{
			var irs = ItemRquiresSync;
			if (irs != null)
				irs.Invoke(patient.Patient);

			Synchronise(order, patient);
		}

		internal void SetOrderAsSelected(OrderViewModel order)
		{
			this.ForEach(o =>
			{
				o.SelectionType = (order.Id == o.Id) ? ScreenSelectionType.Selected : ScreenSelectionType.DeSelected;
			});
		}

		internal void ClearOrdersSelectionType()
		{
			this.ForEach(o =>
			{
				o.SelectionType = null;
			});
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

		private void HandleToggleTracking(OrderViewModel order)
		{
			var tt = ToggleTracking;
			if (tt != null)
				tt.Invoke(order);
		}
	}
}
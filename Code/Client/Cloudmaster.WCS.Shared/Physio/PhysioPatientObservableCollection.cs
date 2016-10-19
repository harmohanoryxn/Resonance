using System;
using System.Collections.Specialized;
using System.Linq;
using System.Collections.Generic;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using Cloudmaster.WCS.DataServicesInvoker.Types;
using WCS.Shared.Orders;
using WCS.Shared.Schedule;
using WCS.Shared.Physio.Schedule;

namespace WCS.Shared.Physio
{
	public class PhysioPatientObservableCollection : WcsScheduleItemObservableCollection2<PhysioPatientViewModel, OrderViewModel, TopPatient, Order>
	{
		public event Action<OrderViewModel, ScreenSelectionType?> TrySelectOrder;

		private OrderStatus _orderStatusFilter;
		private bool _showHiddenOverride;

		public PhysioPatientObservableCollection(Func<TopPatient, PhysioPatientViewModel> transformFunction)
			: base(transformFunction)
		{
			_orderStatusFilter = OrderStatus.InProgress;
		}

		public OrderStatus OrderStatusFilter
		{
			get { return _orderStatusFilter; }
			set
			{
				if (_orderStatusFilter != value)
				{
					_orderStatusFilter = value;

					UnfilteredCollection.ForEach(patient => patient.ScheduleItems.OrderStatusFilter = value);

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

					UnfilteredCollection.ForEach(patient => patient.ScheduleItems.ShowHiddenOverride = value);

					Filter();
				}
			}
		}

		public override List<PhysioPatientViewModel> DoCustomFiltering(List<PhysioPatientViewModel> items)
		{
			items = items.Where(patient => patient.ScheduleItems.Any(o => o.IsHidden == ShowHiddenOverride)).ToList();

			items = items.Where(patient => patient.ScheduleItems.Any(o => (OrderStatusFilter | o.OrderStatus) == OrderStatusFilter)).ToList();
			
			return items;
		}
		public override void OnAfterFiltering(List<PhysioPatientViewModel> items)
		{ }

	 

		public override void AttachItem(PhysioPatientViewModel item)
		{
			item.TrySelectOrder += HandleTrySelectOrder;
		}

		public override void DetachItem(PhysioPatientViewModel item)
		{
			item.TrySelectOrder -= HandleTrySelectOrder;
		}

		internal void ToggleOrderSelection(OrderViewModel order)
		{
			this.ForEach(patient => patient.ToggleOrderSelection(order));
		}

		internal void ClearOrdersSelectionType()
		{
			this.ForEach(patient => patient.ClearOrdersSelectionType());
		}

		private void HandleTrySelectOrder(OrderViewModel order, ScreenSelectionType? existingSelectionType)
		{
			var tso = TrySelectOrder;
			if (tso != null)
				tso.Invoke(order, existingSelectionType);
		}
	}
}
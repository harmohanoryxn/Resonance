using System;
using System.Linq;
using System.Collections.Generic;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using WCS.Shared.Orders;
using WCS.Shared.Schedule;
using WCS.Shared.Ward.Schedule;

namespace WCS.Shared.Physio
{
	public class PhysioOrderObservableCollection : WcsScheduleItemObservableCollection3<PatientViewModel, OrderViewModel, Order>
	{
		public event Action<OrderViewModel, ScreenSelectionType?> TrySelect;
		public event Action<OrderViewModel> ToggleTracking;

		private OrderStatus _orderStatusFilter;
		private bool _showHiddenOverride;

		public PhysioOrderObservableCollection(Func<Order, PatientViewModel, OrderViewModel> transformFunction)
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

		public override List<OrderViewModel> DoCustomFiltering(List<OrderViewModel> items)
		{
			items = items.Where(o => o.IsHidden == ShowHiddenOverride).ToList();

			items = items.Where(o => (OrderStatusFilter | o.OrderStatus) == OrderStatusFilter).ToList();
			
			return items;
		} 

		public override void AttachItem(OrderViewModel item)
		{
			item.TrySelect += HandleTrySelectItem;
			item.ToggleTracking += HandleToggleTracking;
		}

		public override void DetachItem(OrderViewModel item)
		{
			item.TrySelect -= HandleTrySelectItem;
			item.ToggleTracking -= HandleToggleTracking;
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

		private void HandleToggleTracking(OrderViewModel order)
		{
			var tt = ToggleTracking;
			if (tt != null)
				tt.Invoke(order);
		}
	}
}
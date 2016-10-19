using System;
using System.Linq;
using System.Collections.Generic;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using WCS.Shared.Orders;
using WCS.Shared.Schedule;
using WCS.Shared.Ward.Schedule;

namespace WCS.Shared.Ward
{
	public class WardOrderObservableCollection : WcsScheduleItemObservableCollection3<PatientViewModel, OrderViewModel, Order>
	{
		private string _location;
		private string _patientName; 
		private OrderStatus _orderStatusFilter;
		private bool _showHiddenOverride;

		public event Action<OrderViewModel, ScreenSelectionType?> TrySelect;
		public event Action<OrderViewModel> ToggleTracking;

		public event Action ShouldRecalculateStatistics;

		public WardOrderObservableCollection(Func<Order, PatientViewModel, OrderViewModel> transformFunction)
			: base(transformFunction)
		{
			_orderStatusFilter = OrderStatus.InProgress;
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

			items = items.Where(o => (OrderStatusFilter | o.OrderStatus) == OrderStatusFilter).ToList();

			if (!string.IsNullOrEmpty(Location))
				items = items.Where(o => Location == o.AdmissionWardCode).ToList();

			if (!string.IsNullOrEmpty(PatientNameFilter))
				items = items.Where(app => !string.IsNullOrEmpty(app.SearchString) && app.SearchString.Contains(PatientNameFilter)).ToList();

			return items;
		} 

		public override void AttachItem(OrderViewModel item)
		{
			item.TrySelect += HandleTrySelectItem;
			item.ShouldRecalculateStatistics += HandleRequestToRecalculateStatistics;
			item.ItemRquiresSync += Synchronise;
			item.ToggleTracking += HandleToggleTracking;
		}

		public override void DetachItem(OrderViewModel item)
		{
			item.TrySelect -= HandleTrySelectItem;
			item.ShouldRecalculateStatistics -= HandleRequestToRecalculateStatistics;
			item.ItemRquiresSync -= Synchronise;
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
				ts.Invoke(order,existingSelectionType);
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

		private void HandleToggleTracking(OrderViewModel order)
		{
			var tt = ToggleTracking;
			if (tt != null)
				tt.Invoke(order);
		}
	}
}
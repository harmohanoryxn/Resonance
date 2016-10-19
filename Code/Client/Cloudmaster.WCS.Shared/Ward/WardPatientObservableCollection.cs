using System;
using System.Collections.Specialized;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using Cloudmaster.WCS.DataServicesInvoker.Types;
using WCS.Core;
using WCS.Shared.Orders;
using WCS.Shared.Schedule;
using WCS.Shared.Ward.Schedule;

namespace WCS.Shared.Ward
{
	public class WardPatientObservableCollection : WcsScheduleItemObservableCollection2<WardPatientViewModel, OrderViewModel, TopPatient, Order>
	{
		private string _location;
		private string _patientName;
		private MultiSelectAdmissionStatusFlag _admissionStatusFlagFilter;
		private OrderStatus _orderStatusFilter;
		private bool _showHiddenOverride;

		public event Action<OrderViewModel, ScreenSelectionType?> TrySelect;

		public event Action ShouldRecalculateStatistics;

		public WardPatientObservableCollection(Func<TopPatient, WardPatientViewModel> transformFunction)
			: base(transformFunction)
		{
			_admissionStatusFlagFilter = _admissionStatusFlagFilter.Set(MultiSelectAdmissionStatusFlag.Registered | MultiSelectAdmissionStatusFlag.Admitted);
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

					UnfilteredCollection.ForEach(patient => patient.ScheduleItems.PatientNameFilter = value);

					//Filter();
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

					UnfilteredCollection.ForEach(patient => patient.ScheduleItems.Location = value);

					//Filter();
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

		/// <summary>
		/// Filters this list on based on the  Ward code
		/// </summary>
		public override List<WardPatientViewModel> DoCustomFiltering(List<WardPatientViewModel> items)
		{
			items = items.Where(patient => patient.ScheduleItems.Any(o => o.IsHidden == ShowHiddenOverride)).ToList();

			items = items.Where(patient => patient.ScheduleItems.Any(o => (OrderStatusFilter | o.OrderStatus) == OrderStatusFilter)).ToList();

			if (Location != null && Location.Any())
				items = items.Where(patient => Location.Contains(patient.WardCode)).ToList();

			if (!string.IsNullOrEmpty(PatientNameFilter))
				items = items.Where(patient => !string.IsNullOrEmpty(patient.SearchString) && patient.SearchString.Contains(PatientNameFilter)).ToList();

			return items;
		}
		public override void OnAfterFiltering(List<WardPatientViewModel> items)
		{ }


		private void HandleTrySelectOrder(OrderViewModel order, ScreenSelectionType? existingSelectionType)
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


		public override void AttachItem(WardPatientViewModel item)
		{
			item.TrySelectOrder += HandleTrySelectOrder;
			item.ScheduleItems.ShouldRecalculateStatistics += HandleRequestToRecalculateStatistics;
			item.ScheduleItems.CollectionChanged += HandleOrdersChanged;
		}

		public override void DetachItem(WardPatientViewModel item)
		{
			item.TrySelectOrder -= HandleTrySelectOrder;
			item.ScheduleItems.ShouldRecalculateStatistics -= HandleRequestToRecalculateStatistics;
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
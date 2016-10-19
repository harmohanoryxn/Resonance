using System;
using System.Collections.Specialized;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using WCS.Core;
using WCS.Shared.Schedule;

namespace WCS.Shared.Discharge.Schedule
{
	public class DischargeRoomObservableCollection : WcsScheduleItemObservableCollection2<DischargeRoomViewModel, DischargeBedViewModel, DischargeTopRoom, BedDischargeData>
	{
		private string _location;
		public event Action<DischargeBedViewModel, ScreenSelectionType?> TrySelect;

		public event Action ShouldRecalculateStatistics;


		public DischargeRoomObservableCollection(Func<DischargeTopRoom, DischargeRoomViewModel> transformFunction)
			: base(transformFunction)
		{
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



		/// <summary>
		/// Filters this list on based on the  Ward code
		/// </summary>
		public override List<DischargeRoomViewModel> DoCustomFiltering(List<DischargeRoomViewModel> items)
		{
			if (!string.IsNullOrEmpty(Location))
				items = items.Where(room => Location.Contains(room.LocationCode)).ToList();

			return items;
		}
		public override void OnAfterFiltering(List<DischargeRoomViewModel> items)
		{ }

		private void HandleTrySelectItem(DischargeBedViewModel bed, ScreenSelectionType? existingSelectionType)
		{
			var ts = TrySelect;
			if (ts != null)
				ts.Invoke(bed, existingSelectionType);
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

		public override void AttachItem(DischargeRoomViewModel item)
		{
			item.TrySelectBed += HandleTrySelectItem;
			item.ScheduleItems.ShouldRecalculateStatistics += HandleRequestToRecalculateStatistics;
			item.ScheduleItems.CollectionChanged += HandleBedsChanged;
		}

		public override void DetachItem(DischargeRoomViewModel item)
		{
			item.TrySelectBed -= HandleTrySelectItem;
			item.ScheduleItems.ShouldRecalculateStatistics -= HandleRequestToRecalculateStatistics;
			item.ScheduleItems.CollectionChanged -= HandleBedsChanged;
		}
		/// <summary>
		/// Gets fired when a nested collection gets changed and might force the top level list to refilter
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
		private void HandleBedsChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			Task.Factory.StartNew(Filter).LogExceptionIfThrownAndIgnore();
		}

		internal void ToggleBedSelection(DischargeBedViewModel bed)
		{
			this.ForEach(room => room.ToggleOrderSelection(bed));
		}

		internal void ClearBedsSelectionType()
		{
			this.ForEach(room => room.ClearBedsSelectionType());
		}
	}
}
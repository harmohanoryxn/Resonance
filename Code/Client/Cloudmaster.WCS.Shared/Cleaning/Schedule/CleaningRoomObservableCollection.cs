using System;
using System.Collections.Specialized;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using WCS.Core;
using WCS.Shared.Beds;
using WCS.Shared.Schedule;

namespace WCS.Shared.Cleaning.Schedule
{
	public class CleaningRoomObservableCollection : WcsScheduleItemObservableCollection2<RoomViewModel, BedViewModel, TopRoom, Bed>
	{
		private string _location;
		public event Action<BedViewModel, ScreenSelectionType?> TrySelect;

		public event Action ShouldRecalculateStatistics;

		public IEnumerable<BedTime> BedTimes { get; private set; }

		public CleaningRoomObservableCollection(Func<TopRoom, RoomViewModel> transformFunction)
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
		public override List<RoomViewModel> DoCustomFiltering(List<RoomViewModel> items)
		{
			if (!string.IsNullOrEmpty(Location))
				items = items.Where(room => Location.Contains(room.LocationCode)).ToList();

			return items;
		}
		public override void OnAfterFiltering(List<RoomViewModel> items)
		{ }

		private void HandleTrySelectItem(BedViewModel bed, ScreenSelectionType? existingSelectionType)
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

		public override void AttachItem(RoomViewModel item)
		{
			item.TrySelectBed += HandleTrySelectItem;
			item.ScheduleItems.ShouldRecalculateStatistics += HandleRequestToRecalculateStatistics;
			item.ScheduleItems.CollectionChanged += HandleBedsChanged;
		}

		public override void DetachItem(RoomViewModel item)
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

		internal void ToggleBedSelection(BedViewModel bed)
		{
			this.ForEach(room => room.ToggleOrderSelection(bed));
		}

		internal void ClearBedsSelectionType()
		{
			this.ForEach(room => room.ClearBedsSelectionType());
		}
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using WCS.Shared.Beds;
using WCS.Shared.Cleaning.Schedule;
using WCS.Shared.Orders;
using WCS.Shared.Schedule;

namespace WCS.Shared.Ward.Schedule
{
	public class CleaningBedObservableCollection : WcsScheduleItemObservableCollection<BedViewModel, Bed>
	{
		public event Action<BedViewModel, ScreenSelectionType?> TrySelect;
		public event Action ShouldRecalculateStatistics;

		private string _location;

		public IEnumerable<BedTime> BedTimes { get; private set; }

		public CleaningBedObservableCollection(Func<Bed, BedViewModel> transformFunction)
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
					Filter();
				}
			}
		}

		/// <summary>
		/// Filters this list on based on the department code
		/// </summary>
		public override List<BedViewModel> DoCustomFiltering(List<BedViewModel> items)
		{
			if (!string.IsNullOrEmpty(Location))
				items = items.Where(b => Location == b.LocationCode).ToList();

			return items;
		}

		public override void AttachItem(BedViewModel item)
		{
			item.TrySelect += HandleTrySelectItem;
			item.ShouldRecalculateStatistics += HandleRequestToRecalculateStatistics;
		}

		public override void DetachItem(BedViewModel item)
		{
			item.TrySelect -= HandleTrySelectItem;
			item.ShouldRecalculateStatistics -= HandleRequestToRecalculateStatistics;
		}


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

		internal void SetBedAsSelected(BedViewModel bed)
		{
			this.ForEach(b =>
			{
				b.SelectionType = (bed.Id == b.Id) ? ScreenSelectionType.Selected : ScreenSelectionType.DeSelected;
				b.IsHighlighted = (bed.Id == b.Id);
			});
		}
  
		internal void ToggleBedSelection(BedViewModel bed)
		{
			this.ForEach(room => room.ToggleBedSelection(bed));
		}

		internal void ClearBedsSelectionType()
		{
			this.ForEach(room => room.ClearBedsSelectionType());
		}
	}
}
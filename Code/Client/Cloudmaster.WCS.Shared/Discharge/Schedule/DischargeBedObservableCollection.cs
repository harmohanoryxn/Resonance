using System;
using System.Collections.Generic;
using System.Linq;
using Cloudmaster.WCS.DataServicesInvoker.DataServices; 
using WCS.Shared.Schedule;

namespace WCS.Shared.Discharge.Schedule
{
	public class DischargeBedObservableCollection : WcsScheduleItemObservableCollection<DischargeBedViewModel, BedDischargeData>
	{
		public event Action<DischargeBedViewModel, ScreenSelectionType?> TrySelect;
		public event Action ShouldRecalculateStatistics;
		public event Action<DischargeBedViewModel> StartingManipulation;
		public event Action<DischargeBedViewModel> EndedManipulation;
	
		private string _location;

		public DischargeBedObservableCollection(Func<BedDischargeData, DischargeBedViewModel> transformFunction)
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
		public override List<DischargeBedViewModel> DoCustomFiltering(List<DischargeBedViewModel> items)
		{
			if (!string.IsNullOrEmpty(Location))
				items = items.Where(b => Location == b.LocationCode).ToList();

			return items;
		}

		public override void AttachItem(DischargeBedViewModel item)
		{
			item.TrySelect += HandleTrySelectItem;
			item.ShouldRecalculateStatistics += HandleRequestToRecalculateStatistics;
			item.StartingManipulation += HandleStartingManipulation;
			item.EndedManipulation += HandleEndedManipulation;
		}

		public override void DetachItem(DischargeBedViewModel item)
		{
			item.TrySelect -= HandleTrySelectItem;
			item.ShouldRecalculateStatistics -= HandleRequestToRecalculateStatistics;
			item.StartingManipulation -= HandleStartingManipulation;
			item.EndedManipulation -= HandleEndedManipulation;
		}
		 

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

		private void HandleStartingManipulation(DischargeBedViewModel discharge)
		{
			var sm = StartingManipulation;
			if (sm != null)
				sm(discharge);
		}

		private void HandleEndedManipulation(DischargeBedViewModel discharge)
		{
			var em = EndedManipulation;
			if (em != null)
				em(discharge);
		}

		internal void SetBedAsSelected(DischargeBedViewModel discharge)
		{
			this.ForEach(b =>
			{
				b.SelectionType = (discharge.Id == b.Id) ? ScreenSelectionType.Selected : ScreenSelectionType.DeSelected;
				b.IsHighlighted = (discharge.Id == b.Id);
			});
		}
  
		internal void ToggleBedSelection(DischargeBedViewModel bed)
		{
			this.ForEach(room => room.ToggleBedSelection(bed));
		}

		internal void ClearBedsSelectionType()
		{
			this.ForEach(room => room.ClearBedsSelectionType());
		}
	}
}
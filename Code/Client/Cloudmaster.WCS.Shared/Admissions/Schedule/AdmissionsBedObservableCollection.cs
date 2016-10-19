using System;
using System.Collections.Generic;
using System.Linq;
using Cloudmaster.WCS.DataServicesInvoker.DataServices; 
using WCS.Shared.Schedule;

namespace WCS.Shared.Admissions.Schedule
{
	public class AdmissionsBedObservableCollection : WcsScheduleItemObservableCollection<AdmissionBedViewModel, BedDischargeData>
	{
		public event Action<AdmissionBedViewModel, ScreenSelectionType?> TrySelect;
		public event Action ShouldRecalculateStatistics;
	
		private string _location;

		public AdmissionsBedObservableCollection(Func<BedDischargeData, AdmissionBedViewModel> transformFunction)
			: base(transformFunction)
		{
		}
 

		/// <summary>
		/// Filters this list on based on the department code
		/// </summary>
		public override List<AdmissionBedViewModel> DoCustomFiltering(List<AdmissionBedViewModel> items)
		{
			return items;
		}

		public override void AttachItem(AdmissionBedViewModel item)
		{
			item.TrySelect += HandleTrySelectItem;
			item.ShouldRecalculateStatistics += HandleRequestToRecalculateStatistics;
		}

		public override void DetachItem(AdmissionBedViewModel item)
		{
			item.TrySelect -= HandleTrySelectItem;
			item.ShouldRecalculateStatistics -= HandleRequestToRecalculateStatistics;
		}
		 

		private void HandleTrySelectItem(AdmissionBedViewModel bed, ScreenSelectionType? existingSelectionType)
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
		 
		internal void SetBedAsSelected(AdmissionBedViewModel admissions)
		{
			this.ForEach(b =>
			{
				b.SelectionType = (admissions.Id == b.Id) ? ScreenSelectionType.Selected : ScreenSelectionType.DeSelected;
				b.IsHighlighted = (admissions.Id == b.Id);
			});
		}
  
		internal void ToggleBedSelection(AdmissionBedViewModel bed)
		{
			this.ForEach(room => room.ToggleBedSelection(bed));
		}

		internal void ClearBedsSelectionType()
		{
			this.ForEach(room => room.ClearBedsSelectionType());
		}
	}
}
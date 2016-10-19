using System;
using System.Collections.Generic;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using WCS.Shared.Schedule;

namespace WCS.Shared.Admissions.Schedule
{
    public class AdmissionsPatientObservableCollection : WcsScheduleItemObservableCollection<WaitingPatientViewModel, Admission>
	{
		public event Action<WaitingPatientViewModel, ScreenSelectionType?> TrySelect;
		public event Action ShouldRecalculateStatistics;

        public AdmissionsPatientObservableCollection(Func<Admission, WaitingPatientViewModel> transformFunction)
			: base(transformFunction)
		{
		}

	 
		/// <summary>
		/// Filters this list on based on any local filters set from the BackStage
		/// </summary>
		public override List<WaitingPatientViewModel> DoCustomFiltering(List<WaitingPatientViewModel> items)
		{
			return items;
		}

		public override void AttachItem(WaitingPatientViewModel item)
		{
			item.TrySelect += HandleTrySelectItem;
			item.ShouldRecalculateStatistics += HandleRequestToRecalculateStatistics;
		}

		public override void DetachItem(WaitingPatientViewModel item)
		{
			item.TrySelect -= HandleTrySelectItem;
			item.ShouldRecalculateStatistics -= HandleRequestToRecalculateStatistics;
		}


		private void HandleTrySelectItem(WaitingPatientViewModel waitingPatient, ScreenSelectionType? existingSelectionType)
		{
			var ts = TrySelect;
			if (ts != null)
				ts.Invoke(waitingPatient, existingSelectionType);
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

		internal void SetBedAsSelected(WaitingPatientViewModel waitingPatient)
		{
			this.ForEach(b =>
			{
				b.SelectionType = (waitingPatient.Id == b.Id) ? ScreenSelectionType.Selected : ScreenSelectionType.DeSelected;
				b.IsHighlighted = (waitingPatient.Id == b.Id);
			});
		}
  
		internal void ToggleBedSelection(WaitingPatientViewModel waitingPatient)
		{
			this.ForEach(room => room.ToggleBedSelection(waitingPatient));
		}

		internal void ClearBedsSelectionType()
		{
			this.ForEach(room => room.ClearBedsSelectionType());
		}
	}
}
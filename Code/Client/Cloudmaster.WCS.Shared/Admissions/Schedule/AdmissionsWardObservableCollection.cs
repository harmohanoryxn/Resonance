using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using WCS.Core;
using WCS.Shared.Schedule;

namespace WCS.Shared.Admissions.Schedule
{
    public class AdmissionsWardObservableCollection : WcsScheduleItemObservableCollection2<AdmissionsWardViewModel, WaitingPatientViewModel, AdmissionsTopWard, BedDischargeData>
	{
		public event Action<AdmissionBedViewModel, ScreenSelectionType?> TrySelect;

		public event Action ShouldRecalculateStatistics;

		public AdmissionsWardObservableCollection(Func<AdmissionsTopWard, AdmissionsWardViewModel> transformFunction)
			: base(transformFunction)
		{
		}
 
		/// <summary>
		/// Filters this list on based on the  Ward code
		/// </summary>
		public override List<AdmissionsWardViewModel> DoCustomFiltering(List<AdmissionsWardViewModel> items)
		{
			return items;
		}
		public override void OnAfterFiltering(List<AdmissionsWardViewModel> items)
		{ }

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

		public override void AttachItem(AdmissionsWardViewModel item)
		{
			item.TrySelectBed += HandleTrySelectItem;
			item.ScheduleItems.ShouldRecalculateStatistics += HandleRequestToRecalculateStatistics;
			item.ScheduleItems.CollectionChanged += HandleBedsChanged;
		}

		public override void DetachItem(AdmissionsWardViewModel item)
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

		internal void ToggleBedSelection(AdmissionBedViewModel bed)
		{
			this.ForEach(room => room.ToggleOrderSelection(bed));
		}

		internal void ClearBedsSelectionType()
		{
			this.ForEach(room => room.ClearBedsSelectionType());
		}
	}
}
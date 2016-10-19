using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cloudmaster.WCS.DataServicesInvoker;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;

namespace WCS.Shared.Schedule
{
    /// <summary>
    /// View Model for frequent background polling for Admission data
    /// </summary>
    public class AdmissionsDataRecurringPoller
    {
		public event Action<AdmissionsData> AdmissionsBedDataUpdateAvailable;

        private Timer _timer;
        private int _timeout;

        private IWcsAsyncInvoker _invoker;
        private LocationCodes _locations;

		public AdmissionsDataRecurringPoller(IWcsAsyncInvoker invoker, IEnumerable<string> locations, int timeoutInSeconds)
        {
            _invoker = invoker;
            _timeout = timeoutInSeconds;

            _timer = new Timer(HandleTick);
            _locations = new LocationCodes();
            _locations.TryMerge(locations);

            _timer.Change(0, _timeout * 1000);
        }

        void HandleTick(object sender)
        {
            HitServerForData();
        }

        internal void SetLocations(IEnumerable<string> locations)
        {
            if (_locations.TryMerge(locations))
                HitServerForData();
        }

        public void HitServerForData()
        {
            if (_locations.Any())
				_invoker.GetAdmissionDataAsync(_locations, ReceiveGetCleaningBedDataAsyncResults);
        }

        public void SetTimeout(int timeoutInSeconds)
        {
            if (_timeout == timeoutInSeconds) return;

            _timer.Change(0, timeoutInSeconds * 1000);
        }

		private void ReceiveGetCleaningBedDataAsyncResults(AdmissionsData results)
        {
            var evt = AdmissionsBedDataUpdateAvailable;
            if (evt != null)
            {
                evt(results);
            }
        }

    }
}

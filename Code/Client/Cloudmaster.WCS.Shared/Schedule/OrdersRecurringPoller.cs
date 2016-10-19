using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cloudmaster.WCS.DataServicesInvoker;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;

namespace WCS.Shared.Schedule
{

    /// <summary>
    /// View Model for frequent background polling for Orders
    /// </summary>
    public class OrdersRecurringPoller
    {
        public event Action<IList<Order>> OrdersUpdateAvailable;


        private enum PollFOrWhen
        {
            Today,
            Yesterday
        }

        private Timer _timer;
        private int _timeout;
        private PollFOrWhen _when;

        private IWcsAsyncInvoker _invoker;
        private LocationCodes _locations;

        public OrdersRecurringPoller(IWcsAsyncInvoker invoker, DateTime ordersForDate, IEnumerable<string> locations, int timeoutInSeconds)
        {
            _invoker = invoker;
            _timeout = timeoutInSeconds;

            _locations = new LocationCodes();
            _locations.TryMerge(locations);

            _timer = new Timer(HandleTick);

            SetDate(ordersForDate);
        }

        internal void SetLocations(IEnumerable<string> locations)
        {
            if (_locations.TryMerge(locations))
                HitServerForData();
        }

        internal void SetDate(DateTime date)
        {
            _when = date.Date == DateTime.Today ? PollFOrWhen.Today : PollFOrWhen.Yesterday;

            if (_when == PollFOrWhen.Today)
            {
                _timer.Change(_timeout * 1000, _timeout * 1000);
            }
            else
            {
                _timer.Change(Timeout.Infinite, Timeout.Infinite);
            }

            HitServerForData();
        }

        void HandleTick(object sender)
        {
            HitServerForData();
        }

        public void HitServerForData()
        {
            DateTime fromDate = DiscoverPollingDate();
            _invoker.GetOrdersAsync(fromDate, fromDate.AddDays(1), _locations, RecieveOrderAsyncResults);
        }

        public void SetTimeout(int timeoutInSeconds)
        {
            if (_timeout == timeoutInSeconds) return;

            _timer.Change(0, timeoutInSeconds * 1000);
        }

        private void RecieveOrderAsyncResults(IList<Order> results)
        {
			var evt = OrdersUpdateAvailable;
            if (evt != null && results != null)
            {
                var pollingDate = DiscoverPollingDate();
                var resultsForDate = results.Where(o => o.ProcedureTime.HasValue && DateTime.Compare(o.ProcedureTime.Value.Date, pollingDate) == 0).ToList();

                evt(resultsForDate);
            }
        }


        /// <summary>
        /// Works out the polling date in a today or yesterday since
        /// </summary>
        /// <returns></returns>
        private DateTime DiscoverPollingDate()
        {
            return _when == PollFOrWhen.Today ? DateTime.Today.Date : DateTime.Today.Date.AddDays(-1);
        }
    }
}

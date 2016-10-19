using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using Cloudmaster.WCS.DataServicesInvoker;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;

namespace WCS.Shared.Schedule
{
	public class PresenceRecurringPoller
	{
		public event Action<IList<Presence>> LocationConnectionUpdateAvailable;

		private IWcsAsyncInvoker _invoker;
		private int _timeout;

		private Timer _timer;

		public PresenceRecurringPoller(IWcsAsyncInvoker invoker, int timeoutInSeconds)
		{
			_invoker = invoker;
			_timeout = timeoutInSeconds;

			_timer = new Timer(HandleTick);

			_timer.Change(0, _timeout * 1000);
		}

		void HandleTick(object sender)
		{
			HitServerForData();
		}

		public void HitServerForData()
		{
			_invoker.GetPresenceDataAsync(RecieveLocationConnectionAsyncResults);
		}

		public void SetTimeout(int timeoutInSeconds)
		{
			if (_timeout == timeoutInSeconds) return;

			_timer.Change(0, timeoutInSeconds * 1000);
		}

		private void RecieveLocationConnectionAsyncResults(IList<Presence> locationConnections)
		{
			var evt = LocationConnectionUpdateAvailable;
			if (evt != null)
			{
				evt(locationConnections);
			}
		}
	}
}

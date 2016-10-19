using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cloudmaster.WCS.DataServicesInvoker;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;

namespace WCS.Shared.Schedule
{
	/// <summary>
	/// View Model for frequent background polling for Configurations
	/// </summary>
	public class ConfigurationRecurringPoller
	{
		public event Action<PollingTimeouts> PollingTimeoutUpdateAvailable;

		private Timer _timer;
		private int _timeout;
		private int _shortcut;

		private IWcsAsyncInvoker _invoker;

		public ConfigurationRecurringPoller(IWcsAsyncInvoker invoker, int shortcut, int timeoutInSeconds)
		{
			_invoker = invoker;
			_timeout = timeoutInSeconds;

			_timer = new Timer(HandleTick);

			SetShortcut(shortcut);
		}

		public void SetShortcut(int shortcut)
		{
			_shortcut = shortcut;

			HitServerForData();
		}

		void HandleTick(object sender)
		{
			HitServerForData();
		}

		public void HitServerForData()
		{
			_invoker.GetLatestPollingTimeoutsAsync(_shortcut,RecieveAsyncResults);
		}

		public void SetTimeout(int timeoutInSeconds)
		{
			if (_timeout == timeoutInSeconds) return;

			_timer.Change(0, timeoutInSeconds * 1000);
		} 

		private void RecieveAsyncResults(PollingTimeouts results)
		{
			var evt = PollingTimeoutUpdateAvailable;
			if (evt != null)
			{
				evt(results);
			}
		}
		 
	}
}

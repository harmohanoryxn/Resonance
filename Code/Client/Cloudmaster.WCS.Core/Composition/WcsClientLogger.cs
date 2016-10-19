using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace WCS.Core.Composition
{
	[Export(typeof(IWcsClientLogger))]
	[PartCreationPolicy(CreationPolicy.Shared)]
	public class WcsClientLogger : IWcsClientLogger
	{
		public event Action<LogMessage> MessageArrived;
		public event Action<int> CallToServerDelta;

		public void LogMessage(string message)
		{
			var ma = MessageArrived;
			if (ma != null)
				ma.Invoke(new LogMessage(DateTime.Now, message));
		}


		public void Increment()
		{
			RegisterChange(1);
		}

		public void Decrement()
		{
			RegisterChange(-1);
		}

		private void RegisterChange(int change)
		{
			var csd = CallToServerDelta;
			if (csd != null)
				csd.Invoke(change);
		}
	}
}

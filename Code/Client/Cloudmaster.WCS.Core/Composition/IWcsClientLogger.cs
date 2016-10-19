using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WCS.Core.Composition
{
	public interface IWcsClientLogger
	{
		event Action<LogMessage> MessageArrived;
		event Action<int> CallToServerDelta;

		void LogMessage(string message);
		void Increment();
		void Decrement();
	}
}

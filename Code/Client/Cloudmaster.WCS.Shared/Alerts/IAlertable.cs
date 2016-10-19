using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WCS.Shared.Alerts
{
	public interface IAlertable
	{
		IEnumerable<string> GetAlertMessages();
	}
}

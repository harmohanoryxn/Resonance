using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WCS.Shared.Schedule
{
	public enum SynchronousStatus
	{
		Loading,
		UpToDate,
		DelayedUpTo1Hour,
		DelayedUpTo1Day,
		DelayedMoreThanDay
	}
}

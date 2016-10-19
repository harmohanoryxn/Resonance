using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WCS.Shared.Schedule
{
	public interface IUpdateableTimelineItem : ITimelineItem, IEquatable<IUpdateableTimelineItem>
	{
		void AbsorbNewNotificationStartTime(TimeSpan newNotificationStartTime);
	}
}

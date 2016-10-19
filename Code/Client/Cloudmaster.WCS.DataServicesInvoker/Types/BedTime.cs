using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WCS.Core;

namespace Cloudmaster.WCS.DataServicesInvoker.DataServices
{
	public partial class BedTime : IIdentifable, ITimeDefinition
	{
		public int Id
		{
			get { return GetFingerprint(); }
		}
		public int GetFingerprint()
		{
            return BedId.GetHashCode() ^ StartTime.GetHashCode() ^ EndTime.GetHashCode() ^ (IsDueToDischarge ? "dueToDischarge" : "notDueToDischarge").GetHashCode();
			
		}

		public TimeSpan BeginTime
		{
			get { return StartTime.TimeOfDay; }
		}


		public TimeSpan Duration
		{
			get { return EndTime - StartTime; }
		}


		public int CompareTo(ITimeDefinition other)
		{
			return BeginTime.CompareTo(other.BeginTime);
		}
	}
}

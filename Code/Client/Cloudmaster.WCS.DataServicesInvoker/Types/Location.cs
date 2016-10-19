using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cloudmaster.WCS.DataServicesInvoker.DataServices
{
	public partial class Location
	{
		public int GetFingerprint()
		{
				var fp = (IsEmergency ? "Emergency" : "NotEmergency").GetHashCode();
				if (!string.IsNullOrEmpty(FullName))
					fp = fp ^ FullName.GetHashCode();
				if (!string.IsNullOrEmpty(Name))
					fp = fp ^ Name.GetHashCode();
				if (!string.IsNullOrEmpty(Room))
					fp = fp ^ Room.GetHashCode();
				if (!string.IsNullOrEmpty(Bed))
					fp = fp ^ Bed.GetHashCode();
				return fp;
			
		}
	}
}

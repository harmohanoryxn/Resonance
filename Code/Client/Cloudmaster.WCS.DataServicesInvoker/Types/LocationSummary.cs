using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WCS.Core;

namespace Cloudmaster.WCS.DataServicesInvoker.DataServices
{
	public partial class LocationSummary : IIdentifable
	{
		public int Id
		{
			get { return LocationId; }
		}
		public int GetFingerprint()
		{
				var fp = LocationId.GetHashCode();
				if (!string.IsNullOrEmpty(Name))
					fp = fp ^ Name.GetHashCode();
				if (!string.IsNullOrEmpty(Code))
					fp = fp ^ Code.GetHashCode();
				return fp;
			
		}
	}
}

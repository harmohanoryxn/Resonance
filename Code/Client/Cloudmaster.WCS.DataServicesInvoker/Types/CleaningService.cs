using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WCS.Core;

namespace Cloudmaster.WCS.DataServicesInvoker.DataServices
{
	public partial class CleaningService : IIdentifable
	{
		public int Id
		{
			get { return CleaningServiceId; }
		}
		public int GetFingerprint()
		{
				return CleaningServiceId ^ CleaningServiceType.GetHashCode() ^ ServiceTime.GetHashCode() ;
			
		}
	}
}

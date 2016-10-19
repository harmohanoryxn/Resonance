using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WCS.Core;

namespace Cloudmaster.WCS.DataServicesInvoker.DataServices
{
	public partial class DetectionLocation : IIdentifable
	{
		public int Id
		{
			get { return DetectorId; }
		}
		public int GetFingerprint()
		{
            var fp = DetectorId.GetHashCode();
				if (!string.IsNullOrEmpty(LocationName))
					fp = fp ^ LocationName.GetHashCode();
				if (!string.IsNullOrEmpty(LocationCode))
					fp = fp ^ LocationCode.GetHashCode();
				return fp;
			
		}
	}
}

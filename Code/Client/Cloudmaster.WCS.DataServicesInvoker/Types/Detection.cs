using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WCS.Core;

namespace Cloudmaster.WCS.DataServicesInvoker.DataServices
{
	public partial class Detection : IIdentifable, IComparable<Detection>
	{
		public int Id
		{
			get { return DetectionId; }
		}
		public int GetFingerprint()
		{
                var fp = DetectionId.GetHashCode() ^ PatientId.GetHashCode() ^ Timestamp.GetHashCode() ^ Direction.GetHashCode();
				if (DetectionLocation != null)
                    fp = fp ^ DetectionLocation.GetFingerprint();
				return fp;
			
		}

		public int CompareTo(Detection other)
		{
			return Timestamp.CompareTo(other.Timestamp);
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WCS.Core;

namespace Cloudmaster.WCS.DataServicesInvoker.DataServices
{
	public partial class Notification : IIdentifable
	{
		public int Id
		{
			get { return NotificationId; }
		}

		public int GetFingerprint()
		{
				var fp = NotificationId ^ OrderId ^ PriorToProcedureTime.GetHashCode() ^ (RequiresAcknowledgement ? "RequiresAcknowledgement" : "NoRequiresAcknowledgement").GetHashCode() ^ (Acknowledged ? "Acknowledged" : "NoAcknowledged").GetHashCode() ^ Duration.GetHashCode() ^ AcknowledgedTime.GetHashCode();
				if (!string.IsNullOrEmpty(Description))
					fp = fp ^ Description.GetHashCode();
				if (!string.IsNullOrEmpty(AcknowledgedBy))
					fp = fp ^ AcknowledgedBy.GetHashCode();
				return fp;
		
		}
	}
}

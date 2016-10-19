using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WCS.Core;

namespace Cloudmaster.WCS.DataServicesInvoker.DataServices
{
	public partial class Update : IIdentifable
	{
		public int Id
		{
			get { return UpdateId; }
		}
		public int GetFingerprint()
		{
				var fp = UpdateId ^    Created.GetHashCode();
				if (!string.IsNullOrEmpty(Type))
					fp = fp ^ Type.GetHashCode();
				if (!string.IsNullOrEmpty(Value))
					fp = fp ^ Value.GetHashCode();
				if (!string.IsNullOrEmpty(Source))
					fp = fp ^ Source.GetHashCode();
				if (OrderId.HasValue)
					fp = fp ^ OrderId.Value;
				if (BedId.HasValue)
					fp = fp ^ BedId.Value;
				return fp;
			
		}
	}
}

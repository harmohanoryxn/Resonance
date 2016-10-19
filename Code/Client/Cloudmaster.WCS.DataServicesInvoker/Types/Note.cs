using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WCS.Core;

namespace Cloudmaster.WCS.DataServicesInvoker.DataServices
{
	public partial class Note : IIdentifable
	{
		public int Id
		{
			get { return NoteId; }
		}

		public int GetFingerprint()
		{
				int fp = NoteId ^ DateCreated.GetHashCode();
				if (!string.IsNullOrEmpty(Source))
					fp = fp ^ Source.GetHashCode();
				if (!string.IsNullOrEmpty(NoteMessage))
					fp = fp ^ NoteMessage.GetHashCode();
				if (BedId.HasValue)
					fp = fp ^ BedId.Value;
				if (OrderId.HasValue)
					fp = fp ^ OrderId.Value;
				return fp;
		
		}
 
	}
}

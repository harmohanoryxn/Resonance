using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cloudmaster.WCS.DataServicesInvoker.Types
{
	[Flags]
	public enum MultiSelectAdmissionTypeFlag : int
	{
        // These values must match Admission Status values are direct comparision

		Unknown = 0,
		In = 1,
		Out = 2,
		Day = 4,
        All = In | Out | Day
	}
}

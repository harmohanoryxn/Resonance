using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cloudmaster.WCS.DataServicesInvoker.Types
{
	[Flags]
	public enum MultiSelectAdmissionStatusFlag : int
	{
        // These values must match Admission Status values are direct comparision

		Unknown = 0,
		Registered = 1,
		Admitted = 2,
		Discharged = 4,

	}
}

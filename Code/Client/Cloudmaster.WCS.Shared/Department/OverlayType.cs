using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WCS.Shared.Department
{
	[Flags]
	public enum OverlayType
	{
		None = 0x0,
		Availability = 0x1,
		OfficeHours = 0x2 
	}
}

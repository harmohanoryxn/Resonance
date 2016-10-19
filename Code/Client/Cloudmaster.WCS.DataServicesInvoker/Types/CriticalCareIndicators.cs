using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cloudmaster.WCS.DataServicesInvoker.DataServices
{
	public partial class CriticalCareIndicators
	{
		public int GetFingerprint()
		{
			return (IsMrsaRisk ? "mrsa" : "nomrsa").GetHashCode() ^ (IsFallRisk ? "fall" : "nofall").GetHashCode() ^ (HasLatexAllergyField ? "latex" : "nolatex").GetHashCode() ^ (IsRadiationRisk ? "rad" : "noRad").GetHashCode();
			
		}
	}
}

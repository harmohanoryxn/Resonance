using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WCS.Core;

namespace Cloudmaster.WCS.DataServicesInvoker.DataServices
{
    // Need to use extension methods to add these methods to LocationCodes as the proxy class generated is not partial
    public static class PatientCodesExtensions
	{
        public static int GetFingerprint(this PatientCodes thisPatientCodes)
		{
			var fp = 0;
            thisPatientCodes.ForEach(l => fp = fp ^ l.GetHashCode());
			return fp;
		}

		public static bool TryMerge(this PatientCodes thisPatientCodes, IEnumerable<string> patients)
		{
			patients = patients.ToList();

			if (!patients.Any()) return false;
			int fpBefore = GetFingerprint(thisPatientCodes);
			thisPatientCodes.Clear();
			thisPatientCodes.Union(patients).ToList().ForEach(thisPatientCodes.Add);
			int fpAfter = GetFingerprint(thisPatientCodes);
			return fpBefore != fpAfter;
		}
	}
}

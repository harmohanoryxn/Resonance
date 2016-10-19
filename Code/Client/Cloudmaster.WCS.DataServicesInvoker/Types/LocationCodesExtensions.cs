using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WCS.Core;

namespace Cloudmaster.WCS.DataServicesInvoker.DataServices
{
    // Need to use extension methods to add these methods to LocationCodes as the proxy class generated is not partial
    public static class LocationCodesExtensions
	{
        public static int GetFingerprint(this LocationCodes thisLocationCodes)
		{
			var fp = 0;
            thisLocationCodes.ForEach(l => fp = fp ^ l.GetHashCode());
			return fp;
		}

        public static bool TryMerge(this LocationCodes thisLocationCodes, IEnumerable<string> locations)
        {
			locations = locations.ToList();

			if (!locations.Any()) return false;
			var fpBefore = GetFingerprint(thisLocationCodes);
			thisLocationCodes.Clear();
            thisLocationCodes.Union(locations).ToList().ForEach(thisLocationCodes.Add);
			int fpAfter = GetFingerprint(thisLocationCodes);
        	return fpBefore != fpAfter;
        }

     
	}

}

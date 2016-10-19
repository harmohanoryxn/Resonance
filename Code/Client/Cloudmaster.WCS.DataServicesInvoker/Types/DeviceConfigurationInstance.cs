using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WCS.Core;

namespace Cloudmaster.WCS.DataServicesInvoker.DataServices
{
	public partial class DeviceConfigurationInstance : IIdentifable
	{
		public int Id
		{
			get { return LocationCode.GetHashCode() ^ ShortcutKey; }
		}

		public int GetFingerprint()
		{
			var fp = ShortcutKey.GetHashCode() ^ (RequiresLoggingOn ? "yes" : "no").GetHashCode() ^ Type.GetHashCode();
			if (!string.IsNullOrEmpty(LocationName))
				fp = fp ^ LocationName.GetHashCode();
			if (!string.IsNullOrEmpty(LocationCode))
				fp = fp ^ LocationCode.GetHashCode();
			if (PollingTimeouts != null)
				fp = fp ^ PollingTimeouts.GetHashCode();

			if (VisibleLocations.Any()) fp = fp ^ VisibleLocations.Select(vl => vl.GetFingerprint()).ToList().Aggregate((accFp, nextFp) => nextFp + accFp);
			return fp;
		}

		/// <summary>
		/// Determines whether a config instance is valid or not. A bad config cannot be used
		/// </summary>
		public bool IsValid()
		{
            bool result = true;
            if (this.Type != DeviceConfigurationType.Physio)
            {
                result = VisibleLocations != null && VisibleLocations.Any();
            }

            return result;
		}

		public LocationSummary DefaultLocation
		{
			get
			{
				var def = (VisibleLocations.FirstOrDefault(vl => vl.Code == LocationCode));
				if (def != null)
					return def;
				def = (VisibleLocations.FirstOrDefault());
				if (def != null)
					return def;
				return null;
			}
		}

		public string DefaultLocationName
		{
			get
			{
				var def = DefaultLocation;
				if (def != null)
					return def.Name;
			 
				return "General";
			}
		}
	}
}

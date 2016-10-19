using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WCS.Services.DataServices;

namespace WCS.Services.DataServices.Data
{
    public static class RfidDetectorExtensions
	{
		public static DetectionLocation Convert(this WCS.Data.EF.RfidDetector rfidd)
		{
			var detection = new DetectionLocation();
			detection.DetectorId = rfidd.rfidDetectorId;

            //TODO: this is just a temporary fix to stop NullReference error from occurring, the DetectionLocation & RfidDetector classes should be fixed
            // to better handle Waiting Areas, at the moment they assume everything is just a Location
            if (rfidd.Location != null)
            {
                detection.LocationId = rfidd.Location.locationId;
                detection.LocationName = rfidd.Location.name;
                detection.LocationCode = rfidd.Location.code;
            }
            else if (rfidd.WaitingArea_waitingAreaId.HasValue)
            {
                detection.LocationId = rfidd.WaitingArea_waitingAreaId.Value;
                detection.LocationCode = rfidd.WaitingArea.code;
                detection.LocationName = rfidd.WaitingArea.name;
            }
            else
            {
                detection.LocationId = 0;
                detection.LocationCode = rfidd.externalId;
                detection.LocationName = rfidd.externalId;
            }
            return detection;
		}
	}
} 
 
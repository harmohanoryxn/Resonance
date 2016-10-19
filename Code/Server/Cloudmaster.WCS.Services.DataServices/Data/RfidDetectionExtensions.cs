using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WCS.Services.DataServices;

namespace WCS.Services.DataServices.Data
{
	public static class RfidDetectionExtensions
	{
		public static Detection Convert(this WCS.Data.EF.RfidDetection rfidd)
		{
			var detection = new Detection();
			return new Detection()
            {
                DetectionId = rfidd.rfidDetectionId,
                DetectionLocation = rfidd.RfidDetector.Convert(),
                Direction = (DetectionDirection) Enum.Parse(typeof(DetectionDirection), rfidd.RfidDirection.direction),
                PatientId = rfidd.patientId,
                Timestamp = rfidd.dateTimeDetected,
            };
		}
	}
} 
 
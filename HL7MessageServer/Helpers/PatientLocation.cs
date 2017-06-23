using HL7MessageServer.Classes;
using HL7MessageServer.Model;
using NHapi.Base.Model;
using NHapi.Base.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace H7Message
{
    public static class PatientLocation
    {
        public static int PatientLocationId(string location)
        {
            WCSHL7Entities wcs = new WCSHL7Entities();
            string assignedpatientlocation = location;
            
            int patientlocationId = 1;
            if (assignedpatientlocation == null || assignedpatientlocation == "")
            {

            }
            else
            {
                string patientlocation = ReturnLocation.location(assignedpatientlocation,0);
                if (patientlocation == "" || patientlocation == null)
                {

                }
                else
                {
                    //int patientlocationcheck = Convert.ToInt32(wcs.Locations.Where(l => l.name == patientlocation || l.name.Contains(patientlocation)).Select(locId => locId.locationId).FirstOrDefault());
                    //if (patientlocationcheck <= 0)
                    //{
                    //    Location loc = new Location();
                    //    loc.name = assignedpatientlocation;
                    //    loc.code = assignedpatientlocation;
                    //    loc.isEmergency = false;
                    //    loc.includeInMerge = true;
                    //    wcs.Locations.Add(loc);
                    //    wcs.SaveChanges();
                    //}

                    patientlocationId = Convert.ToInt32(wcs.Locations.Where(l => l.name == patientlocation || l.name.Contains(patientlocation)).Select(locId => locId.locationId).FirstOrDefault());
                }
            }
            
            return patientlocationId;
        }
    }
}

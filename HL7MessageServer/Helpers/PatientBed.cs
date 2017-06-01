using HL7MessageServer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HL7MessageServer.Helpers
{
    public static class PatientBed
    {
        public static int PBed(string BedNumber,string RoomNumber)
        {
            WCSHL7Entities wcs = new WCSHL7Entities();
            int bedid = 0;
            if (BedNumber != null || BedNumber != "" || RoomNumber !=null || RoomNumber!="")
            {
                int roomcheck = wcs.Beds.Where(c => c.name == BedNumber &&
                                c.Room_roomId==(wcs.Rooms.Where(r=>r.name==RoomNumber)
                                .Select(rm=>rm.roomId).FirstOrDefault())).Select(d => d.bedId).FirstOrDefault();
                if (roomcheck > 0)
                {
                    bedid = roomcheck;
                }
                else
                {
                    bedid = -1;
                }
            }
            else
            {
                bedid = 0;
            }

            return bedid;

        }
    }
}
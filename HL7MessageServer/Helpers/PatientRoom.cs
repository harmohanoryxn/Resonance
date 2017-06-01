using HL7MessageServer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HL7MessageServer.Helpers
{
    public static class PatientRoom
    {
        
        public static int PRoom(string RoomName)
        {
            WCSHL7Entities wcs = new WCSHL7Entities();
            int roomid = 0;
            if(RoomName!=null ||RoomName!="")
            {
                int roomcheck = wcs.Rooms.Where(c => c.name == RoomName).Select(d => d.roomId).FirstOrDefault();
                if(roomcheck > 0)
                {
                    roomid = roomcheck;
                }
                else
                {
                    roomid = -1;
                }
            }
            else
            {
                roomid = 0;
            }

            return roomid;

        }
    }
}
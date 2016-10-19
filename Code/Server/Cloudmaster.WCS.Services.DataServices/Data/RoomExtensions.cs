using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WCS.Services.DataServices.Data
{
	public static class RoomExtensions
	{
		public static Room Convert(this WCS.Data.EF.Room wcsRoom)
		{
			return new Room()
            {
                RoomId = wcsRoom.roomId,
                Name = wcsRoom.name,
                Ward = wcsRoom.Location.code
            };
		}
	}
}

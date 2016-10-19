using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WCS.Services.DataServices;

namespace WCS.Services.DataServices.Data
{
	public static class DeviceExtensions
	{
		public static Device Convert(this WCS.Data.EF.Device wcsDevice)
		{
			var device = new Device();
			device.Name = wcsDevice.name;
			device.OS = wcsDevice.os;
			device.Description = wcsDevice.description;
			device.LastIPAddress = wcsDevice.ipAddress;
			device.LastConnectionDateTime = wcsDevice.lastConnection;
			device.Location = wcsDevice.Location.code;
			return device;
		}
	}
} 
 
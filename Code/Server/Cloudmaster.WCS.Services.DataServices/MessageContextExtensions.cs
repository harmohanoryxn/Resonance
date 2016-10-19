using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Web;
using WCS.Core;

namespace WCS.Services.DataServices
{
	public static class MessageExtensions
	{
		public static WCSRequestHeaderData ExtractWCSHeaders(this Message request)
		{
			var httpRequestProperty = (HttpRequestMessageProperty)request.Properties[HttpRequestMessageProperty.Name];

			string deviceName = httpRequestProperty.Headers[WCSRequestHeaderData.DeviceName];
			string os = httpRequestProperty.Headers[WCSRequestHeaderData.OSHeader];
			string version = httpRequestProperty.Headers[WCSRequestHeaderData.ApplicationVersion];

			RemoteEndpointMessageProperty endpoint = (RemoteEndpointMessageProperty)request.Properties[RemoteEndpointMessageProperty.Name];

			string ipAddress = endpoint.Address;

			return new WCSRequestHeaderData() { Device = deviceName, OS = os, IPAddress = ipAddress, Version = version};
		}
	}
}
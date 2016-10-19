using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text;
using WCS.Core;
using WCS.Core.Composition;

namespace Cloudmaster.WCS.DataServicesInvoker
{
	/// <summary>
	/// Adds a WCS header to every outgoing request
	/// </summary>
	public class AddWcsHeaderMessageInspector : IClientMessageInspector
	{
		private IDeviceIdentity Device { get; set; }

		public AddWcsHeaderMessageInspector(IDeviceIdentity device)
		{
			Device = device;
 		}
	 
		public void AfterReceiveReply(ref Message reply,object correlationState)
		{
	 	}

		public object BeforeSendRequest(ref Message request,IClientChannel channel)
		{
			MessageHeader header = MessageHeader.CreateHeader("WCSHeader", "http://www.cloudmaster.ie", "WCSHeader");

			request.Headers.Add(header);

			var httpRequestProperty = new HttpRequestMessageProperty();
			httpRequestProperty.Headers.Add(WCSRequestHeaderData.DeviceName, Device.DeviceName);
			httpRequestProperty.Headers.Add(WCSRequestHeaderData.ApplicationVersion, Device.ApplicationVersion);
			httpRequestProperty.Headers.Add(WCSRequestHeaderData.OSHeader, Environment.OSVersion.ToString());
			
			request.Properties[HttpRequestMessageProperty.Name] = httpRequestProperty;

		
			return null;
		}
	}

}

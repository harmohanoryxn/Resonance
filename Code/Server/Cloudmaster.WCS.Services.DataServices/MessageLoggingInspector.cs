using System;
using System.Configuration;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using WCS.Core.Instrumentation;
using WCS.Services.DataServices.Data;

namespace WCS.Services.DataServices
{
	/// <summary>
	/// Logs all incoming requests
	/// </summary>
	public class MessageLoggingInspector : IDispatchMessageInspector
	{
	 
		public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
		{
			var headerData = request.ExtractWCSHeaders();

            string requiredClientVersion = ConfigurationManager.AppSettings["RequiredClientVersion"];

            if (headerData.Version != requiredClientVersion)
				throw new FaultException(new FaultReason("Client out of date"), new FaultCode("1001"));

			new Logger("MessageLoggingBehavior", true).DebugFormat("Request from {0} : {1}", headerData.Device, (request.Headers).Action);
            new ServerFacade().LogDeviceInformation(headerData, DateTime.Now);
			return null;
		}

		public void BeforeSendReply(ref Message reply, object correlationState)
		{
		}
		 
	}
}
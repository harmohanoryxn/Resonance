using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using WCS.Core.Instrumentation;
using WCS.Services.DataServices.Data;

namespace WCS.Services.DataServices
{
	/// <summary>
	/// Loggs all incoming requests endpoint behaviour
	/// </summary>
	public class MessageLoggingEnpointBehavior : IEndpointBehavior
	{
		public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
		{
		}

		public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
		{
		}

		public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
		{
			var inspector = new MessageLoggingInspector();
			endpointDispatcher.DispatchRuntime.MessageInspectors.Add(inspector);
		}

		public void Validate(ServiceEndpoint endpoint)
		{
		}
	}
}
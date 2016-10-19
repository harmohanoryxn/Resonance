using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace WCS.Services.DataServices
{
	/// <summary>
	/// Logs all incoming requests
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class MessageLoggingBehavior : Attribute, IServiceBehavior
	{
		public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
		{
		}

		public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
		{
			foreach (ChannelDispatcher chDisp in serviceHostBase.ChannelDispatchers)
			{
				foreach (EndpointDispatcher epDisp in chDisp.Endpoints)
				{
					epDisp.DispatchRuntime.MessageInspectors.Add(new MessageLoggingInspector());
				}
			}
		}
		 
		public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
		{
		}
	}
}
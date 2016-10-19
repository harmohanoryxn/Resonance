using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using WCS.Core.Composition;

namespace Cloudmaster.WCS.DataServicesInvoker
{
	/// <summary>
	/// Endpoints that adds WCS Header to an clients request
	/// </summary>
	public class AddWcsHeaderBehavior : IEndpointBehavior
	{
		private IDeviceIdentity Device { get; set; }

		public AddWcsHeaderBehavior(IDeviceIdentity device)
		{
			Device = device;
 		}

		#region IEndpointBehavior Members

		public void AddBindingParameters(ServiceEndpoint endpoint,BindingParameterCollection bindingParameters)
		{
		}

		public void ApplyClientBehavior(ServiceEndpoint endpoint,ClientRuntime clientRuntime)
		{
			clientRuntime.MessageInspectors.Add(new AddWcsHeaderMessageInspector(Device));
		}

		public void ApplyDispatchBehavior(ServiceEndpoint endpoint,EndpointDispatcher endpointDispatcher)
		{
		}

		public void Validate(ServiceEndpoint endpoint)
		{
		}

		#endregion
	}

}

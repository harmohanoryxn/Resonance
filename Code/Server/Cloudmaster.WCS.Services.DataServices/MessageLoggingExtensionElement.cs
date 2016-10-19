using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using WCS.Core.Instrumentation;
using WCS.Services.DataServices.Data;

namespace WCS.Services.DataServices
{
	/// <summary>
	/// Loggs all incoming requests extension
	/// </summary>
	public class MessageLoggingExtensionElement : BehaviorExtensionElement
	{
		protected override object CreateBehavior()
		{
			return new MessageLoggingEnpointBehavior();
		}

		public override Type BehaviorType
		{
			get { return typeof(MessageLoggingEnpointBehavior); }
		}
	}
}
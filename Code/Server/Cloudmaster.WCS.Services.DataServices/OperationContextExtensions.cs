using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Web;
using WCS.Core;

namespace WCS.Services.DataServices
{
	public static class OperationContextExtensions
	{
		public static string GetDeviceName(this OperationContext operationContext)
		{
			HttpRequestMessageProperty httpRequestProperty = (HttpRequestMessageProperty)operationContext.IncomingMessageProperties[HttpRequestMessageProperty.Name];

			return httpRequestProperty.Headers[WCSRequestHeaderData.DeviceName];
		}
	}
}
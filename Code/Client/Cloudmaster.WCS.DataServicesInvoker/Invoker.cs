using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using WCS.Core;
using WCS.Core.Composition;

namespace Cloudmaster.WCS.DataServicesInvoker
{
	class Invoker<T>
	{
		internal IWcsExceptionHandler ExceptionHandler { get; set; }
		internal IWcsClientLogger ClientLogger { get; set; }

		internal Invoker(IWcsExceptionHandler exceptionHandler, IWcsClientLogger logger)
		{
			ExceptionHandler = exceptionHandler;
			ClientLogger = logger;
		}
		 
		internal T Execute(string functionName, Func<T> executor, Action retryDelegate)
		{
            //TODO: Re-add after demo
            //if (!NetworkInterface.GetIsNetworkAvailable())
            //{
            //    ExceptionHandler.ProcessException(new ErrorDetails() { Message = "No network detected: Contact Network Administrator", CanRetry = false });
            //    ClientLogger.LogMessage("No network detected: Contact Network Administrator");
            //    return default(T);
            //}

		 
				var sb = new StringBuilder(string.Format("Call : {0}", functionName));

				try
				{
					 var ret = executor.Invoke();

					sb.Append("Succeed");
					ClientLogger.LogMessage(sb.ToString());

					return ret;

				}
				catch (EndpointNotFoundException)
				{
					sb.Append("Failure. ");
					sb.Append("Server not found");
					ClientLogger.LogMessage(sb.ToString());

					ExceptionHandler.ProcessException(new ErrorDetails()
					{
						Message = "ERROR: Unable to connect to server",
						CanRetry = true,
						RetryDelegate = retryDelegate
					});
				}
				catch (CommunicationException ce)
				{
					sb.Append("Failure. ");
					sb.Append(ce.Message);
					ClientLogger.LogMessage(sb.ToString());

					ExceptionHandler.ProcessException(new ErrorDetails()
					{
						Message = "An exception occurred on the server: Contact IT Department"
					});
				}

				return default(T);
		 

		}
	}
}

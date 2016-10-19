using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.ServiceModel;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using WCS.Core;
using WCS.Core.Composition;

namespace Cloudmaster.WCS.DataServicesInvoker
{
	class AsyncInvoker<T>
	{
		internal IWcsExceptionHandler ExceptionHandler { get; set; }
		internal IWcsClientLogger ClientLogger { get; set; }

		internal AsyncInvoker(IWcsExceptionHandler exceptionHandler, IWcsClientLogger logger)
		{
			ExceptionHandler = exceptionHandler;
			ClientLogger = logger;
		}

		internal void Execute(string functionName, Func<T> executor, Action<T> callback, Action retryDelegate)
		{
#if !DEBUG
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                ExceptionHandler.ProcessException(new ErrorDetails() { Message = "No network detected: Contact Network Administrator", CanRetry = false }, ServerStatus.Unavailable);
                ClientLogger.LogMessage("No network detected: Contact Network Administrator");
                return;
            }
#endif
			Task.Factory.StartNew(() =>
			                      	{
			                      		T config=default(T);
				var sb = new StringBuilder(string.Format("Call : {0}", functionName));
				try
				{
					ClientLogger.Increment();

					var startTime = DateTime.Now;
					config = executor.Invoke();
					var diff = DateTime.Now - startTime;

					sb.Append(string.Format(" Succeed (in {0} ms)", diff.Milliseconds));
					ClientLogger.LogMessage(sb.ToString());
					ExceptionHandler.ProcessException(null, ServerStatus.Alive);

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
					}, ServerStatus.Unavailable);
				}
				catch (FaultException e)
				{
					sb.Append("Failure. ");
					sb.Append(e.Message);
					ClientLogger.LogMessage(sb.ToString());

					if (e.Message == "Client out of date")
					{
						ExceptionHandler.ProcessException(new ErrorDetails()
						{
							Message = "The version of WCS is out of date: Contact IT Department",
							AllowRestart = true
						});
					}
					else
					{
						ExceptionHandler.ProcessException(new ErrorDetails()
						{
							Message = "An exception occurred on the server: Contact IT Department"
						});
					}
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
				finally
				{
					ClientLogger.Decrement();
				}

				callback.Invoke(config); 
			

			}).LogExceptionIfThrownAndIgnore();

		}

		internal void Execute(string functionName, Func<T> executor, Action retryDelegate)
		{
#if !DEBUG
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                ExceptionHandler.ProcessException(new ErrorDetails() { Message = "No network detected: Contact Network Administrator", CanRetry = false }, ServerStatus.Unavailable);
                ClientLogger.LogMessage("No network detected: Contact Network Administrator");
                return;
            }
#endif

			Task.Factory.StartNew(() =>
			{
					var sb = new StringBuilder(string.Format("Call : {0}", functionName));

				try
				{
					executor.Invoke();

					sb.Append("Succeed");
					ClientLogger.LogMessage(sb.ToString());

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
				catch (FaultException e)
				{
					sb.Append("Failure. ");
					sb.Append(e.Message);
					ClientLogger.LogMessage(sb.ToString());

					if (e.Message == "Client out of date")
					{
						ExceptionHandler.ProcessException(new ErrorDetails()
						{
							Message = "The version of WCS is out of date: Contact IT Department",
							AllowRestart = true
						});
					}
					else
					{
						ExceptionHandler.ProcessException(new ErrorDetails()
						{
							Message = "An exception occurred on the server: Contact IT Department"
						});
					}
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

			}).LogExceptionIfThrownAndIgnore();
	 
		}
         
	}

	class AsyncInvoker
	{
		internal IWcsExceptionHandler ExceptionHandler { get; set; }
		internal IWcsClientLogger ClientLogger { get; set; }

		internal AsyncInvoker(IWcsExceptionHandler exceptionHandler, IWcsClientLogger logger)
		{
			ExceptionHandler = exceptionHandler;
			ClientLogger = logger;
		}

		internal void Execute(string functionName, Action executor, Action callback, Action retryDelegate)
		{
#if !DEBUG
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                ExceptionHandler.ProcessException(new ErrorDetails() { Message = "No network detected: Contact Network Administrator", CanRetry = false }, ServerStatus.Unavailable);
                ClientLogger.LogMessage("No network detected: Contact Network Administrator");
                return;
            }
#endif

			Task.Factory.StartNew(() =>
			{
			 	var sb = new StringBuilder(string.Format("Call : {0}", functionName));

				try
				{
					executor.Invoke();

					sb.Append("Succeed");
					ClientLogger.LogMessage(sb.ToString());

					return;
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
				catch (FaultException e)
				{
					sb.Append("Failure. ");
					sb.Append(e.Message);
					ClientLogger.LogMessage(sb.ToString());

					if (e.Message == "Client out of date")
					{
						ExceptionHandler.ProcessException(new ErrorDetails()
						{
							Message = "The version of WCS is out of date: Contact IT Department",
							AllowRestart = true
						});
					}
					else
					{
						ExceptionHandler.ProcessException(new ErrorDetails()
						{
							Message = "An exception occurred on the server: Contact IT Department"
						});
					}
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
			 
			}).LogExceptionIfThrownAndIgnore();

		}

		internal void Execute(string functionName, Action executor, Action retryDelegate)
		{
#if !DEBUG
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                ExceptionHandler.ProcessException(new ErrorDetails() { Message = "No network detected: Contact Network Administrator", CanRetry = false }, ServerStatus.Unavailable);
                ClientLogger.LogMessage("No network detected: Contact Network Administrator");
                return;
            }
#endif

			Task.Factory.StartNew(() =>
			{
			 	var sb = new StringBuilder(string.Format("Call : {0}", functionName));

				try
				{
					executor.Invoke();

					sb.Append("Succeed");
					ClientLogger.LogMessage(sb.ToString());
					return;

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
				catch (FaultException e)
				{
					sb.Append("Failure. ");
					sb.Append(e.Message);
					ClientLogger.LogMessage(sb.ToString());

					if (e.Message == "Client out of date")
					{
						ExceptionHandler.ProcessException(new ErrorDetails()
						{
							Message = "The version of WCS is out of date: Contact IT Department",
							AllowRestart = true
						});
					}
					else
					{
						ExceptionHandler.ProcessException(new ErrorDetails()
						{
							Message = "An exception occurred on the server: Contact IT Department"
						});
					}
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
			 
			}).LogExceptionIfThrownAndIgnore();

		}
	}
}

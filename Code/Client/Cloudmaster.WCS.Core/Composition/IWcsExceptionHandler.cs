
using System;
using System.ComponentModel.Composition;

namespace WCS.Core.Composition
{
	public interface IWcsExceptionHandler
	{
		event Action<ErrorDetails> ExceptionArrived;
		event Action<ErrorDetails,ServerStatus> ServerStatusUpdate;

		void ProcessException(ErrorDetails ex);
		void ProcessException(ErrorDetails details, ServerStatus status);
	}

	public enum ServerStatus
	{
		Unavailable,Alive,Dead
	}
}

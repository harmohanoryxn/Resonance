using System;
using System.ComponentModel.Composition;

namespace WCS.Core.Composition
{
	[Export(typeof(IWcsExceptionHandler))]
	[PartCreationPolicy(CreationPolicy.Shared)]
	public class WcsExceptionHandler : IWcsExceptionHandler
	{
		public event Action<ErrorDetails,ServerStatus> ServerStatusUpdate;
		public event Action<ErrorDetails> ExceptionArrived;

		public void ProcessException(ErrorDetails ex)
		{
			if (ex == null) return;

			var ea = ExceptionArrived;
			if (ea != null)
				ea.Invoke(ex);
		}
		public void ProcessException(ErrorDetails ex,ServerStatus status)
		{
			var ssu = ServerStatusUpdate;
			if (ssu != null)
				ssu.Invoke(ex,status);
		}
	}
}

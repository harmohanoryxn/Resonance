using System;

namespace WCS.Core
{
	public class ErrorDetails
	{
		public string Message { get; set; }
		public bool CanRetry { get; set; }
		public bool AllowRestart { get; set; }
		public Action RetryDelegate { get; set; }
	}
}

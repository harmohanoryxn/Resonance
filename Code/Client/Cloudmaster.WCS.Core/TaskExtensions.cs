using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCS.Core.Instrumentation;

namespace WCS.Core
{
	public static class TaskExtensions
	{
		public static Task LogExceptionIfThrownAndIgnore(this Task task)
		{
			task.ContinueWith(c =>
			{
				var _logger = new Logger("Async Task ", false);
				_logger.Error(c.Exception.BuildExceptionInfo());
			},
				TaskContinuationOptions.OnlyOnFaulted |
				TaskContinuationOptions.ExecuteSynchronously);
			return task;
		}
	}
}

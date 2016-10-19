using System;
using System.Diagnostics;
using System.IO;
using log4net.Appender;
using log4net.Core;
using System.Net.Mail;
using System.Net;

namespace WCS.Core.Instrumentation
{
	/// <summary>
	/// Log4Net appender that dumps data into the event log as an error
	/// </summary>
	/// <remarks>
	/// This only exists because WCS is deployed by clickonce and the restrictive sandbox permissions prohibit the default appender from working
	/// </remarks>
	public class EventLogWriterAppender : AppenderSkeleton
	{
		protected override void Append(LoggingEvent loggingEvent)
		{
			var writer = new StringWriter(System.Globalization.CultureInfo.InvariantCulture);
			RenderLoggingEvent(writer, loggingEvent);

			if (EventLog.SourceExists("Application"))
				EventLog.WriteEntry("Application", writer.ToString(), EventLogEntryType.Error);
		}
	}
}

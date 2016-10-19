using System;
using System.Collections.Generic;

namespace WCS.Core.Instrumentation
{
	/// <summary>
	/// Provides a base for logging services supporting level filtering and flushing.
	/// </summary>
	public abstract class BaseLogger
	{
		#region Constructors

		/// <summary>
		/// Provides a means for subclasses to construct the object without forcing a level decision.
		/// </summary>
		protected BaseLogger()
		{
			this.InitializedAtUtc = DateTime.UtcNow;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets a timestamp for when the logger was initialised, which is useful for logging against a timeline.
		/// </summary>
		protected DateTime InitializedAtUtc { get; private set; }

		/// <summary>
		/// Gets or sets the name of the logger.
		/// </summary>
		public string Name { get; protected set; }

		/// <summary>
		/// Gets or sets whether or not debug level logging is enabled.
		/// </summary>
		public bool IsDebugEnabled { get; protected set; }

		/// <summary>
		/// Gets or sets whether or not info level logging is enabled.
		/// </summary>
		public bool IsInfoEnabled { get; protected set; }

		/// <summary>
		/// Gets or sets whether or not warn level logging is enabled.
		/// </summary>
		public bool IsWarnEnabled { get; protected set; }

		/// <summary>
		/// Gets or sets whether or not error level logging is enabled.
		/// </summary>
		public bool IsErrorEnabled { get; protected set; }

		/// <summary>
		/// Gets or sets whether or not fatal level logging is enabled.
		/// </summary>
		public bool IsFatalEnabled { get; protected set; }

		#endregion

		#region Methods

		/// <summary>
		/// Parses a string of the logging level to a LoggingLevel enum value.
		/// </summary>
		/// <param name="s">The string, such as DEBUG or Debug, or debug.</param>
		/// <param name="level">The level it equates to.</param>
		/// <returns>True if a match was found.</returns>
		public static bool TryParse(string s, out LoggingLevel level)
		{
			level = LoggingLevel.None;

			string m = s.ToUpperInvariant();

			if (m == "NONE")
			{
				level = LoggingLevel.None;
				return true;
			}

			if (m == "DEBUG")
			{
				level = LoggingLevel.Debug;
				return true;
			}

			if (m == "INFO")
			{
				level = LoggingLevel.Info;
				return true;
			}

			if (m == "WARN")
			{
				level = LoggingLevel.Warn;
				return true;
			}

			if (m == "ERROR")
			{
				level = LoggingLevel.Error;
				return true;
			}

			if (m == "FATAL")
			{
				level = LoggingLevel.Fatal;
				return true;
			}

			return false;
		}

		/// <summary>
		/// Returns the actual string to write out to the log, prefixed with time and data and the log level.
		/// </summary>
		/// <param name="levelNoun">Such as "DEBUG" or DebugNoun.</param>
		/// <param name="line">The actual string to log.</param>
		/// <returns>A string prefixed with extra information.</returns>
		protected virtual string Prefix(string levelNoun, string line)
		{
			var utcNow = DateTime.UtcNow;
			double msDelta = utcNow.Subtract(this.InitializedAtUtc).TotalMilliseconds;
			string now = utcNow.ToString(System.Globalization.CultureInfo.InvariantCulture);
			return String.Concat(msDelta.ToString(), " ", now, " ", this.Name, " [", levelNoun, "] ", line);
		}

		/// <summary>
		/// Writes a debug level log entry.
		/// </summary>
		public void DebugFormat(string format, params object[] args)
		{
			this.Debug(String.Format(format, args));
		}

		/// <summary>
		/// Writes a debug level log entry.
		/// </summary>
		/// <param name="line"></param>
		public void Debug(string line)
		{
			if (this.IsDebugEnabled)
				this.WriteLog(LoggingLevel.Debug, line);
		}

		/// <summary>
		/// Writes a info level log entry.
		/// </summary>
		public void InfoFormat(string format, params object[] args)
		{
			this.Info(String.Format(format, args));
		}

		/// <summary>
		/// Writes a info level log entry.
		/// </summary>
		public void Info(string line)
		{
			if (this.IsInfoEnabled)
				this.WriteLog(LoggingLevel.Info, line);
		}

		/// <summary>
		/// Writes a warn level log entry.
		/// </summary>
		public void WarnFormat(string format, params object[] args)
		{
			this.Warn(String.Format(format, args));
		}

		/// <summary>
		/// Writes a warn level log entry.
		/// </summary>        
		public void Warn(string line)
		{
			if (this.IsWarnEnabled)
				this.WriteLog(LoggingLevel.Warn, line);
			//this.WriteLog(LoggingLevel.Warn, this.Prefix(LoggingLevel.Warn.ToString().ToUpper(), line));
		}

		/// <summary>
		/// Writes a error level log entry.
		/// </summary>
		public void ErrorFormat(string format, params object[] args)
		{
			this.Error(String.Format(format, args));
		}

		/// <summary>
		/// Writes a error level log entry.
		/// </summary>
		public void Error(string line)
		{
			if (this.IsErrorEnabled)
				this.WriteLog(LoggingLevel.Error, line);
		}

		/// <summary>
		/// Writes a fatal level log entry.
		/// </summary>
		public void FatalFormat(string format, params object[] args)
		{
			this.Fatal(String.Format(format, args));
		}

		/// <summary>	
		/// Writes a fatal level log entry.
		/// </summary>
		public void Fatal(string line)
		{
			if (this.IsFatalEnabled)
				this.WriteLog(LoggingLevel.Fatal, line);
		}

		/// <summary>
		/// When overridden in a derived type, performs the actual writing out to the log destination.
		/// </summary>
		protected abstract void WriteLog(LoggingLevel level, string line);

		/// <summary>
		/// Perform fast flushing or persisting of outstanding log entries.
		/// </summary>
		public abstract void Flush();

		/// <summary>
		/// Calls ToString on each item in a collection and concatenates the results into a long string.
		/// </summary>
		/// <param name="collection">Any enumerable instance.</param>
		/// <returns>The string representation of all the items in the collection in one long string within square brackets.</returns>
		public static string CallToStringOnEach(IEnumerable<object> collection)
		{
			var sb = new System.Text.StringBuilder();
			foreach (var item in collection)
			{
				sb.Append("[");
				sb.Append(item.ToString());
				sb.Append("]");
			}
			return sb.ToString();
		}

		#endregion
	}

	/// <summary>
	/// Prescribes different levels of logging severity where each includes the levels above.
	/// </summary>
	public enum LoggingLevel
	{
		/// <summary>
		/// No logging, effectively disabled.
		/// </summary>
		None = 0,
		/// <summary>
		/// Logging is highly verbose detail used for troubleshooting.
		/// </summary>
		Debug = 1,
		/// <summary>
		/// Indicative of key points in an application being reached.
		/// </summary>
		Info = 2,
		/// <summary>
		/// Indicates a problem or event which is expected, but nonetheless undesirable.
		/// </summary>
		Warn = 3,
		/// <summary>
		/// Indicates an unexpected problem, an exception or other error.
		/// </summary>
		Error = 4,
		/// <summary>
		/// An error occurred which crashed the application, prevented a request response or could have resulted in corruption or data loss.
		/// </summary>
		Fatal = 5
	}
}

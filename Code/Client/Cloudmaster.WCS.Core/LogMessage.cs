using System;

namespace WCS.Core
{
	public struct LogMessage
	{
		private DateTime _timestamp;
		private string _message;

		public DateTime Timestamp
		{
			get { return _timestamp; }
		}

		public string Message
		{
			get { return _message; }
		}

		public LogMessage(DateTime timestamp, string message)
		{
			_timestamp = timestamp;
			_message = message;
		}
	}
}

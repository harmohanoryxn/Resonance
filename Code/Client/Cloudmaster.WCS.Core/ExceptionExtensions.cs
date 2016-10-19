using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WCS.Core
{
	public static class ExceptionExtensions
	{
		/// <summary>
		/// Turns an exception into a readable string, mining down into all the innerexceptions
		/// </summary>
		/// <param name="e">The exception</param>
		/// <returns></returns>
		public static string BuildExceptionInfo(this Exception e)
		{
			var sb = new StringBuilder();

			sb.Append(Indent("Exception.Type: "));
			sb.AppendLine(e.GetType().Name);

			sb.Append(Indent("Exception.Source: "));
			sb.AppendLine(String.IsNullOrEmpty(e.Source) ? String.Empty : e.Source);

			sb.Append(Indent("Exception.Message: "));
			sb.AppendLine(String.IsNullOrEmpty(e.Message) ? String.Empty : e.Message);

			sb.AppendLine(Indent("Exception.StackTrace: "));
			sb.AppendLine();

			if (e.StackTrace != null)
			{
				var stackLines = e.StackTrace.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
				int linePadding = 4;
				for (int i = 0; i < stackLines.Length; i++)
				{
					if (i == 0)
					{
						string stackLine = String.Concat("! ", stackLines[i]);
						// Prefix first stack line with * to denote throw site.                    
						sb.AppendLine(String.IsNullOrEmpty(stackLine)
										  ? Indent(String.Empty)
										  : Indent(stackLine));
					}
					else
					{
						string stackLine = stackLines[i];
						sb.AppendLine(String.IsNullOrEmpty(stackLine)
										  ? Indent(String.Empty)
										  : Indent(stackLine));
					}

				}
			}
			return sb.ToString();
		}

		/// <summary>
		/// Turns an exception into a readable string, mining down into the collection of innerexceptions
		/// </summary>
		/// <param name="ae">The exception</param>
		/// <returns></returns>
		public static string BuildExceptionInfo(this AggregateException ae)
		{
			var flatae = ae.Flatten();
			var sb = new StringBuilder();

				sb.Append(Indent("Exception.Type: "));
				sb.AppendLine(BuildExceptionInfo(flatae as Exception));

				flatae.InnerExceptions.ToList().ForEach(ie =>
			                                    	{
			                                    		sb.Append(Indent("Inner Exception: "));
			                                    		sb.AppendLine(BuildExceptionInfo(ie));
			                                    	});
			return sb.ToString();
		}

		private static string Indent(string text)
		{
			return text.Insert(0, "".PadLeft(2));
		}
	}
}

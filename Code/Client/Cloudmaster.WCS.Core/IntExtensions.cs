using System;

namespace WCS.Core
{
	public static class IntExtensions
	{
		public static string MakeIntoString(this int? val)
		{
			if (val.HasValue)
				return val.Value.ToString();
			else
				return "";
		}
		public static string MakeIntoString(this int val)
		{
				return val.ToString();
		}
	}
}

using System;
using System.Threading;

namespace WCS.Shared
{
	public static class EventExtensions
	{
		public static void Raise<T>(this T e, Object sender, ref EventHandler<T> eventDelegate) where T:EventArgs
		{
			EventHandler<T> tmp = Interlocked.CompareExchange(ref eventDelegate, null, null);
			if (tmp != null) tmp(sender, e);
		}
	}
}

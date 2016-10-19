using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;

namespace WCS.Shared.Controls
{
	public static class DispatcherExtensions
	{

		[DebuggerStepThrough]
		public static void InvokeIfRequired(this UIElement element, Action action)
		{
			if (!element.Dispatcher.CheckAccess())
			{
				element.Dispatcher.Invoke(DispatcherPriority.Normal, action);
			}
			else
			{
				action();
			}
		}

		[DebuggerStepThrough]
		public static void BeginInvokeIfRequired(this UIElement element, Action action)
		{

			if (!element.Dispatcher.CheckAccess())
			{
				element.Dispatcher.Invoke(DispatcherPriority.Normal, action);
			}
			else
			{
				action();
			}

		}

		 
		[DebuggerStepThrough]
		public static void InvokeIfRequired(this Dispatcher dispatcher, Action action)
		{
			if (!dispatcher.CheckAccess())
			{
				dispatcher.Invoke(DispatcherPriority.Normal, action);
			}
			else
			{
				action();
			}
		}

		[DebuggerStepThrough]
		public static void BeginInvokeIfRequired(this Dispatcher dispatcher, Action action)
		{
			if (!dispatcher.CheckAccess())
			{
				dispatcher.Invoke(DispatcherPriority.Normal, action);
			}
			else
			{
				action();
			}
		}



	}
}

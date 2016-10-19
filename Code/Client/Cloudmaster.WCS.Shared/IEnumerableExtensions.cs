using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace WCS.Shared
{
	public static class EnumerableExtensions
	{
		[DebuggerStepThrough]
		public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action) 
		{
			foreach (var item in enumerable)
			{
				action(item);
			}
		}

		[DebuggerStepThrough]
		public static void ForEach<T>(this ObservableCollection<T> enumerable, Action<T> action)
		{
			foreach (var item in enumerable)
			{
				action(item);
			}
		}
		//public static void Filter<T>(this ObservableCollection<T> enumerable)
		//{
		//    enumerable.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		//}
	}

	
}

using System;
using WCS.Core;

namespace WCS.Shared.Schedule
{
	public interface ISynchroniseable<in T> : IIdentifable, IDisposable
	{
		///// <summary>
		///// Set and resets all of the view models properties with a new or modified Order
		///// </summary>
		///// <param name="order">The order</param>
		void Synchronise(T order);
	}
}

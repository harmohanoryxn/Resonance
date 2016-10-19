using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using Cloudmaster.WCS.DataServicesInvoker;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using WCS.Core;
using WCS.Core.Composition;
using WCS.Shared.Department.Schedule;
using WCS.Shared.Schedule;
using WcsMef = WCS.Shared.WcsMef;

namespace Cloudmaster.WCS.Drone
{
	public abstract class BaseDroneEngineViewModel : IDisposable
	{
		public event Action<string> ReportActivity;
		protected static Random Random; 
		private IWcsAsyncInvoker _invoker;

		static  BaseDroneEngineViewModel()
		{
			Random = new Random(); 
		}

		public BaseDroneEngineViewModel(IWcsAsyncInvoker invoker)
		{
			 	_invoker = invoker;


			var tickSource = Observable.Interval(TimeSpan.FromSeconds(0.5));
			tickSource.Subscribe(DoRandomShit);

		}
		public void Report(string thing)
		{
			var onReportActivity = ReportActivity;
			if (onReportActivity != null)
				onReportActivity(thing);
		}

		protected abstract void DoRandomShit(long tick);

		protected bool RollDice(int sides)
		{
			return Random.Next(0, sides) % sides == 0;
		}
		public void Dispose()
		{
		}


		protected void RandomlyMovePatient()
		{
			if (_invoker != null)
				_invoker.RandomlyMovePatientAsync();
		}

		public void OrderCallback(Order o) { }
		public void BedCallback(Bed o) { }

	}
}

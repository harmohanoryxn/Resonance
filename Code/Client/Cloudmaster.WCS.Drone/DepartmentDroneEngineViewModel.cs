using System;
using System.Collections.Generic;
using System.Linq;
using Cloudmaster.WCS.DataServicesInvoker;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using WCS.Core;
using WCS.Core.Composition;
using WCS.Shared.Department.Schedule;
using WCS.Shared.Orders;

namespace Cloudmaster.WCS.Drone
{
	public sealed class DepartmentDroneEngineViewModel : BaseDroneEngineViewModel
	{
		private DepartmentViewModel _dvm ;

		public DepartmentDroneEngineViewModel(IWcsAsyncInvoker invoker, DeviceConfigurationInstance config, PollingTimeouts timeouts, string deviceName)
			: base(invoker)
		{
			_dvm = new DepartmentViewModel(false, DefaultDeviceIdentity.AppVersion, deviceName, DefaultDeviceIdentity.ServerName);
			_dvm.InitialiseConfiguration(config, timeouts);
		}
		 

		/// <summary>
		/// Does the random shit.
		/// </summary>
		/// <param name="tick">The sender.</param>
		protected override void DoRandomShit(long tick)
		{
			if (_dvm == null || _dvm.ScheduleViewModel.ScheduleItems == null || !_dvm.ScheduleViewModel.ScheduleItems.UnfilteredCollection.Any()) return;

			var orders = _dvm.ScheduleViewModel.ScheduleItems.SelectMany(order => order.ScheduleItems).ToList();

			if (RollDice(100))	// 1-in-10 to move order by 
			{
				var o = GetRandomOrder(orders);
				if (o != null)
				{
					var maxValue = Convert.ToInt32((TimeSpan.FromHours(19) - DateTime.Now.TimeOfDay).TotalMinutes + 15);
					if (maxValue > 0)
					{
						var newTime = DateTime.Now.Add(TimeSpan.FromMinutes(Random.Next(0, maxValue)).RoundToNearest15Minutes());

						o.OrderCoordinator.Order.StartTime = newTime.TimeOfDay;
						o.UpdateProcedureTime();

						//if (_invoker != null)
						//    _invoker.UpdateProcedureTimeAsync(o.Id, newTime, DateTime.Now.AddMinutes(-1), OrderCallback);

						Report(string.Format("Update Order [{0}] start time to {1}", o.Id, newTime.ToString("HH:mm")));
					}
				}
			}

			if (RollDice(60))	// 1-in-60 add note
			{
				var o = GetRandomOrder(orders);
				if (o != null)
				{
					var note = string.Format("Note {0}", DateTime.Now.ToString("HH:mm"));
					o.ConversationNotesViewModel.NoteText = note;
					o.ConversationNotesViewModel.AddNewNoteCommand.Execute(null);
					//if (_invoker != null)
					//    _invoker.AddNoteAsync(o.Id, note, OrderCallback);

					Report(string.Format("Added Note '{0}' to Order [{1}] ", note, o.Id));
				}
			}
		 

		}

		private OrderViewModel GetRandomOrder(List<OrderViewModel> orders)
		{
			if (orders.Any())
			return orders[Random.Next(0, orders.Count-1)];
			return null;
		}
	}
}

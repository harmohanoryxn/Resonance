using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Cloudmaster.WCS.DataServicesInvoker;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using WCS.Core.Composition;
using WCS.Shared.Orders;
using WCS.Shared.Schedule;
using WCS.Shared.Ward.Schedule;


namespace Cloudmaster.WCS.Drone
{
	public class WardDroneEngineViewModel : BaseDroneEngineViewModel
	{
		private WardViewModel _wvm;

		public WardDroneEngineViewModel(IWcsAsyncInvoker invoker, DeviceConfigurationInstance config, PollingTimeouts timeouts, string deviceName)
			: base(invoker)
		{
			_wvm = new WardViewModel(false, DefaultDeviceIdentity.AppVersion, deviceName, DefaultDeviceIdentity.ServerName);
			_wvm.InitialiseConfiguration(config, timeouts);
		}
		 
		protected override void DoRandomShit(long tick)
		{
			if (_wvm == null || _wvm.ScheduleViewModel.ScheduleItems == null || !_wvm.ScheduleViewModel.ScheduleItems.UnfilteredCollection.Any()) return;

			var orders = _wvm.ScheduleViewModel.ScheduleItems.UnfilteredCollection.SelectMany(patient=>patient.ScheduleItems.UnfilteredCollection).ToList();

			var ordersWithNotifications = orders.Where(o => o.Notifications.Any(n => n.NotificationType == NotificationType.Prep && !n.IsAcknowledged)).ToList();

			// 1-in-100 to acknowledge notification
			if (RollDice(100))
			{
				var target = GetRandomOrder(ordersWithNotifications);
				target.AcknowledgeOrderAndPrepNotificationsCommand.Execute(null);
				Report(string.Format("Acknowledges Prep Notifications and Order for Order [{0}]", target.Id));
			}

			if (RollDice(60))	// 1-in-60 add note
			{
				var o = GetRandomOrder(orders);

					var note = string.Format("Note {0}", DateTime.Now.ToString("HH:mm"));
				o.ConversationNotesViewModel.NoteText = note;
				o.ConversationNotesViewModel.AddNewNoteCommand.Execute(null);

				Report(string.Format("Added Note '{0}' to Order [{1}] ", note, o.Id));
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

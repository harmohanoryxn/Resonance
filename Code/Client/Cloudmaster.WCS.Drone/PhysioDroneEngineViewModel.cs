using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cloudmaster.WCS.DataServicesInvoker;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using WCS.Core.Composition;
using WCS.Shared.Orders;
using WCS.Shared.Schedule;
using WCS.Shared.Physio.Schedule;

namespace Cloudmaster.WCS.Drone
{
	class PhysioDroneEngineViewModel: BaseDroneEngineViewModel
	{
		private PhysioViewModel _pvm;

		public PhysioDroneEngineViewModel(IWcsAsyncInvoker invoker, DeviceConfigurationInstance config, PollingTimeouts timeouts, string deviceName)
			: base(invoker)
		{
			_pvm = new PhysioViewModel(false, DefaultDeviceIdentity.AppVersion, deviceName, DefaultDeviceIdentity.ServerName);
			_pvm.InitialiseConfiguration(config, timeouts);
		}
		 
		protected override void DoRandomShit(long tick)
		{
			if (_pvm == null || _pvm.ScheduleViewModel.ScheduleItems == null || !_pvm.ScheduleViewModel.ScheduleItems.UnfilteredCollection.Any()) return;

			var orders = _pvm.ScheduleViewModel.ScheduleItems.UnfilteredCollection.SelectMany(patient=>patient.ScheduleItems.UnfilteredCollection).ToList();

			var ordersWithNotifications = orders.Where(o => o.Notifications.Any(n => n.NotificationType == NotificationType.Prep && !n.IsAcknowledged)).ToList();

			// 1-in-100 to acknowledge notification
			if (RollDice(100))
			{
				var target = GetRandomOrder(ordersWithNotifications);
				target.AcknowledgeOrderAndPrepNotificationsCommand.Execute(null);
				Report(string.Format("Acknowledges Physio Notifications for Order [{0}]", target.Id));
			}

			if (RollDice(60))	// 1-in-60 add note
			{
				var o = GetRandomOrder(orders);

				var note = string.Format("Note {0}", DateTime.Now.ToString("HH:mm"));
				o.ConversationNotesViewModel.NoteText = note;
				o.ConversationNotesViewModel.AddNewNoteCommand.Execute(null); 

				Report(string.Format("Added Note '{0}' to Order [{1}] ", note, o.Id));
			}

			if (RollDice(20))
			{
				RandomlyMovePatient();

				Report(string.Format("Randomly moved patient"));
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

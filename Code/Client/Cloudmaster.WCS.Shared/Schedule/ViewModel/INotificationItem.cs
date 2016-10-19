using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;

namespace WCS.Shared.Schedule
{
	public interface INotificationItem: IOrderItem
	{
		event Action<Order> OrderUpdateAvailable;

		NotificationType NotificationType { get; }
	
		DateTime? AcknowledgedTime { get; }

		bool IsAcknowledged { get; set; }

		void AbsorbNewOrderProcedureTime(DateTime newProcedureTime);
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WCS.Services.DataServices;

namespace WCS.Services.DataServices.Data
{
	public static class NotificationExtensions
	{
		public static Notification Convert(this WCS.Data.EF.Notification n)
		{
			var notification = new Notification();
			notification.NotificationId = n.notificationId;
			notification.OrderId = n.orderId;
			notification.Description = n.description;
			notification.NotificationType = (NotificationType)Enum.Parse(typeof(NotificationType), n.NotificationType.name);
			notification.Duration = TimeSpan.FromMinutes(n.durationMinutes);
			notification.PriorToProcedureTime = TimeSpan.FromMinutes(n.priorToProcedureTime);
			notification.RequiresAcknowledgement = n.isAcknowledgmentRequired;
			notification.Acknowledged = n.acknowledged;
			notification.AcknowledgedTime = n.acknowledgedTime;
			notification.AcknowledgedBy = n.acknowledgedBy;
			return notification;
		}
	}
} 
 
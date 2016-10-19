using System;
using System.Collections.Generic;
using Cloudmaster.WCS.DataServicesInvoker;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using WCS.Core;
using WCS.Shared.Orders;

namespace WCS.Shared.Schedule
{
	public interface IOrderItem : IDisposable, IEquatable<IOrderItem>, IIdentifable
	{
		TimeSpan? StartTime { get; set; }

		OrderScheduleItemType AppointmentType { get; }
		TimeSpan? PriorTime { get; }
		TimeSpan Duration { get; }

		void Synchronise(Order order);

		void Synchronise(Notification notification);

		void Synchronise(IList<Detection> detections);

		void HandleMinuteTimerTick();
	}
}

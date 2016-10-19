using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using WCS.Core;
using WCS.Shared.Orders;
using WCS.Shared.Schedule;
using WCS.Shared.Timeline;

namespace WCS.Shared.Converters
{
	public class AllOrdersToAllPatientTimlineConverter : IMultiValueConverter
	{
		  
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (targetType != typeof(IEnumerable))
				throw new InvalidOperationException("Wrong return type");

			if (values.Count() != 2)
				throw new ArgumentException("Wrong Argument amount");
			if (values[0] == null || values[1] == null)
				return Enumerable.Empty<ITimelineItem>();
			if (!(values[0] is IList) || !(values[1] is string))
				return Enumerable.Empty<ITimelineItem>();

			var orders = (IList)values[0];
				var patientId = (string)values[1];

			try
			{
				var timelineItems = new List<ITimelineItem>();
				foreach (OrderViewModel order in orders)
				{
					if (order.OrderCoordinator.Order.Patient.IPeopleNumber == patientId)
					{
						timelineItems.AddRange(order.OrderHeaderTimeline.TimelineItems.Where(ti => ti.TimelineType == TimelineItemType.PatientOccupied).ToList());
					}
				}

				var mergedTimes = TimeDefinition.MergeTimes((timelineItems).Cast<ITimeDefinition>().ToList());
				return mergedTimes.Select(mt => new TimelineVariableDurationEventViewModel(1, mt.BeginTime, mt.Duration, "Patient is Occupied", TimelineItemType.PatientOccupied, null)).ToList();
			}
			catch
			{
				return Enumerable.Empty<ITimelineItem>();
			}
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}



using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using WCS.Shared.Schedule;

namespace WCS.Shared.Converters
{
	/// <summary>
	/// Calculates the left offset of the notification by binding to order start, it's start and duration
	/// </summary>
	class NotificationOffsetConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (targetType != typeof(Thickness))
				throw new InvalidOperationException("Wrong return type");

			if (values.Count() != 4)
				throw new ArgumentException("Wrong Argument amount");
			if (values[2] == null || values[1] == null || values[0] == null || values[3] == null)
				return new Thickness(0, 0, 0, 0);
			if (values[0].GetType() != typeof(double) || values[1].GetType() != typeof(TimeSpan) || values[2].GetType() != typeof(TimeSpan) || values[3].GetType() != typeof(TimeSpan))
				return new Thickness(0, 0, 0, 0);
		
			var maxWidth = (double)values[0];
			var notificationStartTime = (TimeSpan)values[1];
			var duration = (TimeSpan)values[2];
			var priorDuration = (TimeSpan)values[3];

			if (maxWidth==0.0)
				return new Thickness(0, 0, 0, 0);
		
		//	var notificationStartTime = orderStartTime - priorDuration;
			notificationStartTime = notificationStartTime.Subtract(new TimeSpan(4, 0, 0));

			var oneHourWidth = ScheduleBackgroundBase.GetOneHourWidth(maxWidth, true);

			var leftWidth = Math.Floor(notificationStartTime.Hours * oneHourWidth) + ((notificationStartTime.Minutes * oneHourWidth) / 60.0);
			var notificationWidth = Math.Floor(duration.Hours * oneHourWidth) + ((duration.Minutes * oneHourWidth) / 60.0);
	//		var orderStart = Math.Floor(orderStartTime.Hours * oneHourWidth) + ((orderStartTime.Minutes * oneHourWidth) / 60.0);
			var maxAvailableWidth = maxWidth - notificationWidth;

			var left = Math.Floor(Math.Max(Math.Min(maxAvailableWidth, leftWidth), 0.0));
			return new Thickness(left, 0, 0, 0);
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}

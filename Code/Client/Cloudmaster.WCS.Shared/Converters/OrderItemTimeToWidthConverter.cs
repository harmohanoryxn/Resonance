using System;
using System.Linq;
using System.Windows.Data;
using WCS.Shared.Schedule;

namespace WCS.Shared.Converters
{
	/// <summary>
	/// Used to create a binding for a width (defined by time) rendered in the appointment items control
	/// </summary>
	class OrderItemTimeToWidthConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (targetType != typeof(double))
				throw new InvalidOperationException("Wrong return type");

			if (values.Count() != 4)
				throw new ArgumentException("Wrong Argument amount");
			if (values[0] == null || values[1] == null || values[2] == null)
				return 0.0;
			if (values[0].GetType() != typeof(double) || values[1].GetType() != typeof(TimeSpan) || values[2].GetType() != typeof(TimeSpan))
				return 0.0;

			var maxWidth = System.Convert.ToDouble(values[0]);
			var duration = (TimeSpan)values[1];
			var starttime = (TimeSpan)values[2];
			var orderid = (int)values[3];

			if (maxWidth == 0.0)
				return 0.0;

			var minStartTime = new TimeSpan(4, 0, 0);
			var thresholdTime = new TimeSpan(5, 0, 0);

			var oneHourWidth = ScheduleBackgroundBase.GetOneHourWidth(maxWidth, true);
			var realStartTime = starttime;//.Subtract(minStartTime);

			if (realStartTime <= thresholdTime)
				return oneHourWidth;
			//else if (realStartTime < thresholdTime)
			//    return Math.Min(ConvertTimeToWidth(minStartTime + duration, oneHourWidth), ConvertTimeToWidth(thresholdTime, oneHourWidth));
			else
				return Math.Round(Math.Max(ConvertTimeToWidth(duration, oneHourWidth), oneHourWidth));
		}

		private static double ConvertTimeToWidth(TimeSpan realStartTime, double oneHourWidth)
		{
			return Math.Floor(realStartTime.Hours * oneHourWidth) + ((realStartTime.Minutes * oneHourWidth) / 60.0);
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}

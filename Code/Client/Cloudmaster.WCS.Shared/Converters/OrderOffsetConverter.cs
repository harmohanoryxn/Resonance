using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using WCS.Shared.Schedule;

namespace WCS.Shared.Converters
{
	/// <summary>
	/// Used to create a binding for a width (defined by pixels) rendered in the appointment items control
	/// </summary>
	class OrderOffsetConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (targetType != typeof(Thickness))
				throw new InvalidOperationException("Wrong return type");

			if (values.Count() != 4)
				throw new ArgumentException("Wrong Argument amount");
			if (values[0] == null || values[1] == null || values[2] == null)
				return new Thickness(0, 0, 0, 0);
			if (values[0].GetType() != typeof(double) || values[1].GetType() != typeof(TimeSpan) || values[2].GetType() != typeof(TimeSpan))
				return new Thickness(0, 0, 0, 0);
		
			var maxWidth = System.Convert.ToDouble(values[0]);
			var duration = (TimeSpan)values[1];
			var starttime = (TimeSpan)values[2];
			var orderid = (int)values[3];

			if (maxWidth == 0.0)
				return new Thickness(0, 0, 0, 0);
		
			var oneHourWidth = ScheduleBackgroundBase.GetOneHourWidth(maxWidth, true);
			var startts = starttime.Subtract(new TimeSpan(4, 0, 0));

			var leftOffset = Math.Floor(startts.Hours * oneHourWidth) + ((startts.Minutes * oneHourWidth) / 60.0);
			var orderWidth = Math.Floor(duration.Hours * oneHourWidth) + ((duration.Minutes * oneHourWidth) / 60.0);
			var minLeftOffset = maxWidth - orderWidth;

			//if (orderid == 2654)
			//    Console.WriteLine("{0} {1} {2}", maxWidth, starttime, durationts);


			var left = Math.Floor(Math.Max(Math.Min(minLeftOffset, leftOffset), 0.0));
			return new Thickness(left, 0, 0, 0);
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}

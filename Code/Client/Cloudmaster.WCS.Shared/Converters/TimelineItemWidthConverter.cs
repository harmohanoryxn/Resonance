using System;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using WCS.Shared.Schedule;

namespace WCS.Shared.Converters
{
	/// <summary>
	/// Calculates the width of a non-standard time item
	/// </summary>
	class TimelineItemWidthConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (targetType != typeof(double))
				throw new InvalidOperationException("Wrong return type");

			if (values.Count() != 3)
				throw new ArgumentException("Wrong Argument amount");
			if (values[0].GetType() != typeof(double))
				return 0.0;
			if (values[1].GetType() != typeof(TimeSpan))
				return 0.0;
			if (values[2].GetType() != typeof(bool))
				return 0.0;

			var maxWidth = System.Convert.ToDouble(values[0]);
			var durationts = (TimeSpan)values[1];
			var hasTba = (bool)values[2];

			var oneHourWidth = ScheduleBackgroundBase.GetOneHourWidth(maxWidth, hasTba);

			return Math.Max(Math.Floor(durationts.Hours * oneHourWidth) + ((durationts.Minutes * oneHourWidth) / 60.0), 0.0);
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}

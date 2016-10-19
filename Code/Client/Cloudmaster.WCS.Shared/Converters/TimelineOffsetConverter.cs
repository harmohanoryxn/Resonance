using System;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using WCS.Shared.Schedule;

namespace WCS.Shared.Converters
{
	/// <summary>
	/// Calculates the left offset of the timeline item by binding to order start, it's start and duration
	/// </summary>
	class TimelineOffsetConverter : IMultiValueConverter
    {
		
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
			if (targetType != typeof(Thickness))
				throw new InvalidOperationException("Wrong return type");

			if (values.Count() != 4)
				throw new ArgumentException("Wrong Argument amount");
			if (values[0].GetType() != typeof(double))
				return new Thickness(0, 0, 0, 0);
			if (values[1].GetType() != typeof(TimeSpan))
				return new Thickness(0, 0, 0, 0);
			if (values[2].GetType() != typeof(bool))
				return new Thickness(0, 0, 0, 0);
			if (values[3].GetType() != typeof(double))
				return new Thickness(0, 0, 0, 0);
		
        	var maxWidth = System.Convert.ToDouble(values[0]);
			var startts = (TimeSpan)values[1];
			var hasTba = (bool)values[2];
			var shiftWidth = System.Convert.ToDouble(values[3]);

			if (maxWidth == 0.0)
				return new Thickness(0, 0, 0, 0);
		
			var oneHourWidth = ScheduleBackgroundBase.GetOneHourWidth(maxWidth, hasTba);
			var startTimespan = startts.Subtract(new TimeSpan((hasTba?4:5), 0, 0));
			var leftWidth = Math.Floor(startTimespan.Hours * oneHourWidth) + ((startTimespan.Minutes * oneHourWidth) / 60.0);
		
        	var leftOffset = Math.Floor(Math.Max(Math.Min(maxWidth, leftWidth), 0.0));
			var leftOffsetMinusHalfWithOfIcon = leftOffset - shiftWidth;
        	return new Thickness(leftOffsetMinusHalfWithOfIcon, 0, 0, 0);
			
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

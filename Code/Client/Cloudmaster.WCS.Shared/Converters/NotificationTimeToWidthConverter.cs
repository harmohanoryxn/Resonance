using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using WCS.Shared.Schedule;

namespace WCS.Shared.Converters
{
    /// <summary>
	/// Used to create a binding for a width (defined by time) rendered in the appointment items control
    /// </summary>
	class NotificationTimeToWidthConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
			if (targetType != typeof(double))
                throw new InvalidOperationException("Wrong return type");

            if (values.Count() != 4)
                throw new ArgumentException("Wrong Argument amount");
			if (values[2] == null || values[1] == null || values[0] == null || values[3] == null)
				return 0.0;
			if (values[2].GetType() != typeof(double) || values[1].GetType() != typeof(TimeSpan) || values[0].GetType() != typeof(TimeSpan) || values[3].GetType() != typeof(TimeSpan))
				return 0.0;

			var oneHourWidth = ScheduleBackgroundBase.GetOneHourWidth(System.Convert.ToDouble(values[2]), true);
			var prior = (TimeSpan)values[0];
			var duration = (TimeSpan)values[1];
			var start = (TimeSpan)values[3];

        	var fouram = TimeSpan.FromHours(4);
        	var fiveam = TimeSpan.FromHours(5);
			var oneHour = TimeSpan.FromHours(1);

			//if (start < default(TimeSpan))
			//    start = default(TimeSpan);
			if (start < fouram)
				duration = duration - (fouram - start);
			//else if (start < fiveam)
			//    duration = duration - (fiveam - start);
			//else if (start  < prior)
			//    duration = start;

			//if (duration < oneHour)
			//    duration = oneHour;
        	var width = Math.Round(Math.Floor(duration.Hours*oneHourWidth) + ((duration.Minutes*oneHourWidth)/60.0));
			if (width >= 0.0)
				return width;
        	return 0.0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

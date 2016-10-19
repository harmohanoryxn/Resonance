using System;
using System.Linq;
using System.Windows.Data;
using WCS.Shared.Schedule;

namespace WCS.Shared.Converters
{
    /// <summary>
	/// Used to create a binding for the offset of the dropdpwn notes popup so that it can track notes across the screen
    /// </summary>
	public class NotePopupOffsetConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
			if (targetType != typeof(double))
                throw new InvalidOperationException("Wrong return type");

            if (values.Count() != 3)
				throw new ArgumentException("Wrong Argument amount");
			if (values[0] == null || values[1] == null || values[2] == null)
				return 0.0;
			if (values[0].GetType() != typeof(double) || values[1].GetType() != typeof(double) || values[2].GetType() != typeof(TimeSpan))
				return 0.0;

			var maxWidth = System.Convert.ToDouble(values[0]);
			var popupWidth = System.Convert.ToDouble(values[1]);
			var oneHourWidth = ScheduleBackgroundBase.GetOneHourWidth(maxWidth, true);
			var starttime = (TimeSpan)values[2];
			var startts = starttime.Subtract(new TimeSpan(4, 0, 0));
			var leftOffet = Math.Floor(startts.Hours * oneHourWidth) + ((startts.Minutes * oneHourWidth) / 60.0);

		//	return 850.0;
			return leftOffet - (popupWidth / 2) + 8;//+ 240; 
			//var durationts = (int)values[2];
			//var orderWidth = Math.Floor(durationts.Hours * oneHourWidth) + ((durationts.Minutes * oneHourWidth) / 60.0);
			//var maxAvailableWidth = maxWidth - orderWidth;

			//// Convert.ToInt32((note.StartTime.Value.TotalHours - 4) * 70);

			//return Math.Floor(Math.Max(Math.Min(maxAvailableWidth, leftOffet), 0.0));
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

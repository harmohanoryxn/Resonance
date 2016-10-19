using System;
using System.Windows.Data;
using System.Globalization;
using WCS.Core;

namespace WCS.Shared.Converters
{
	public class SliderItemLabelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
			if (value == null) return "?";

			if (value.GetType() != typeof(DateTime))
				return "Overdue";

			var dischargeTime = (DateTime)value;
			if (dischargeTime < DateTime.Now.RoundDownToNearestHour())
				return "Late";
			if (dischargeTime < DateTime.Now.RoundUpToNearestHour())
				return "Due";

			var hours = (dischargeTime - DateTime.Now.RoundDownToNearestHour()).TotalHours;
			if (hours >= 72)
				return "3 Days";
			else if (hours >= 48)
				return "2 Days";
			else if (hours >= 24)
				return "1 Day";
			else
				return dischargeTime.TimeOfDay.ToOrderTimeFormat();
        
		}

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return parameter;
        }
    }
}

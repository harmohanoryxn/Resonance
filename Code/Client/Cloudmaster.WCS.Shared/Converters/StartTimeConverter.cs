using System;
using System.Windows.Data;
using System.Globalization;
using WCS.Core;

namespace WCS.Shared.Converters
{
	public class StartTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
			if (value == null) return "TBA";

			if (value.GetType() != typeof(TimeSpan))
				return "TBA";

			var startTime = (TimeSpan)value;
			if(startTime == new TimeSpan())
				return "TBA";

			if (startTime < new TimeSpan(5, 0, 0))
				return "TBA";

			return startTime.ToOrderTimeFormat();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return parameter;
        }
    }
}

using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using WCS.Shared.Schedule;

namespace WCS.Shared.Converters
{
	class MinutesToAngleConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value.GetType() != typeof(int))
				return 0.0;

		    var totalAngle = 270;

			var minutes = (int) value;

            if (minutes > 15)
                return totalAngle;
            else
            {
                var percentage = (double) minutes/15;

                return percentage * totalAngle;
            }
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new System.NotImplementedException();
		}
	}
}

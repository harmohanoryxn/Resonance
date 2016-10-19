using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using WCS.Shared.Schedule;

namespace WCS.Shared.Converters
{
	public class ScheduleMarginConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (targetType != typeof(Thickness))
				throw new InvalidOperationException("Wrong return type");
			if (value == null)
				return new Thickness(5, 5, 5, 0);
			if (value.GetType() != typeof(bool))
				return new Thickness(5, 5, 5, 0);

			return new Thickness(5, 5, 5, 0);
		//	return ((bool)value) ? new Thickness(5, 5, 5, 100) : new Thickness(5, 5, 5, 0);

		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}

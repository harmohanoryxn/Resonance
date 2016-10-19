using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WCS.Shared.Converters
{
	public class NotificationStartTimeToOpacityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
			{
				return 0.0;
			}
			if (!(value is TimeSpan))
			{
				return 0.0;
			}
			return (TimeSpan)value <= TimeSpan.FromHours(5) ? 0.5 : 1.0;
		}

		public object ConvertBack(object value, Type targetType, object parameter,
			CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}

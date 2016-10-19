using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace WCS.Shared.Converters
{
	public class NotificationReequiredToVisibilityConverter : IMultiValueConverter
	{ 
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (targetType != typeof(Visibility))
				throw new InvalidOperationException("Wrong return type");

			if (values.Count() != 3)
				throw new ArgumentException("Wrong Argument amount");
			if (values[0].GetType() != typeof(bool) || values[1].GetType() != typeof(bool))
				return Visibility.Collapsed;
			if (values[2] == null || values[2].GetType() != typeof(TimeSpan) )
				return Visibility.Collapsed;

			return (bool) values[0] && !(bool)values[1] && (TimeSpan)values[2] < DateTime.Now.TimeOfDay;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}

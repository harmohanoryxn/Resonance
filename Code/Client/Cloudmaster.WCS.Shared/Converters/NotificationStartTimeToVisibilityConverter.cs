using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace WCS.Shared.Converters
{
	public class NotificationStartTimeToVisibilityConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (targetType != typeof(Visibility))
				throw new InvalidOperationException("Wrong return type");

			if (values.Count() != 2)
				throw new ArgumentException("Wrong Argument amount");
			if ( !(values[0] is TimeSpan) || !(values[1] is bool))
				return Visibility.Collapsed;

			var notStartTime = (TimeSpan)values[0];
			var isAcked = (bool)values[1];

			if (isAcked || notStartTime > DateTime.Now.TimeOfDay || notStartTime < TimeSpan.FromHours(5))
				return Visibility.Collapsed;

			return Visibility.Visible;

		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
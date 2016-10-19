using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using WCS.Core;

namespace WCS.Shared.Converters
{
	public class TimeToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (targetType != typeof(string) && targetType != typeof(object))
				throw new InvalidOperationException("Wrong return type");
			if (value.GetType() != typeof(TimeSpan))
				return "";

			return ((TimeSpan)value).ToWcsTimeFormat();
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}

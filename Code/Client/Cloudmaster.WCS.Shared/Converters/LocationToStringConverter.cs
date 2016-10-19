using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace WCS.Shared.Converters
{
	public class LocationToStringConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values.Count() == 2)
			{
				return string.Format("{0} | {1}", values);
			}
			if (values.Count() == 3)
			{
				if (string.IsNullOrEmpty((string) values[1]))
					return (string) values[0];
				return string.Format("{0} | {1} | {2}", values);
			}
			return "";
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

}

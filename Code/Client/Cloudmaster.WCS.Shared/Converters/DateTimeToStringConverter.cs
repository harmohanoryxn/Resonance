using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using WCS.Core;

namespace WCS.Shared.Converters
{

	public class DateTimeToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (targetType != typeof(string))
				throw new InvalidOperationException("Wrong return type");
			if (value.GetType() != typeof(DateTime))
				return "";

			var dt = (DateTime) value;
			if (dt.Date == DateTime.Today)
				return string.Format("Today {0}", dt.ToWcsTimeFormat());
			else if (dt.Date.AddDays(+1) == DateTime.Today)
				return string.Format("Yesterday {0}", dt.ToWcsTimeFormat());
			else
				return string.Format("{0} {1}", dt.DayOfWeek, dt.ToWcsTimeFormat());
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}

using System;
using System.Windows.Data;
using System.Globalization;

namespace WCS.Shared.Converters
{
    [ValueConversion(typeof(string), typeof(bool))]
	public class EmptyStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
        	return !string.IsNullOrEmpty((string)value);
               }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return parameter;
        }
    }
}

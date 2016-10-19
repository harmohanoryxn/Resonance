using System;
using System.Globalization;
using System.Windows.Data;

namespace WCS.Shared.Converters
{
    public class NullableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool param = bool.Parse(parameter.ToString());

            if (value == null)
            {
                return param;
            }
            else
            {
                return !(param);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw (new NotImplementedException());
        }
    }
}

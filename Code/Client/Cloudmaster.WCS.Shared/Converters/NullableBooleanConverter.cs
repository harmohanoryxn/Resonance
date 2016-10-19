using System;
using System.Globalization;
using System.Windows.Data;

namespace WCS.Shared.Converters
{
    [ValueConversion(typeof(bool?), typeof(bool))]
    public class NullableBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool param = bool.Parse(parameter.ToString());
            
            if (value == null)
            {
                return false;
            }
            else
            {
                return !((bool)value ^ param);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool param = bool.Parse(parameter.ToString());
            return !((bool)value ^ param);
        }
    }
    
}

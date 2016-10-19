using System;
using System.Windows.Data;
using System.Globalization;

namespace WCS.Shared.Converters
{
    [ValueConversion(typeof(object), typeof(bool))]
    public class BooleanToOppositeConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool currentValue = (bool) value;

            bool oppositeValue = !currentValue;

            return oppositeValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}

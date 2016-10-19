using System;
using System.Windows.Data;
using System.Globalization;

namespace WCS.Shared.Converters
{
    [ValueConversion(typeof(object), typeof(bool))]
    public class BooleanToPassFailConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string result;

            bool isPassed = (bool) value;

            result = isPassed ? "Pass" : "Fail";

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}

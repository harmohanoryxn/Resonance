using System;
using System.Windows.Data;
using System.Globalization;

namespace WCS.Shared.Converters
{
    [ValueConversion(typeof(object), typeof(bool))]
    public class RadioButtonConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool result = false;

            string expectedValue = (string) parameter;

            string valueToCompare = value.ToString();

            if (expectedValue.CompareTo(valueToCompare) == 0)
            {
                result = true;
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return parameter;
        }
    }
}

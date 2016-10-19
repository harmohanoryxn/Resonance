using System;
using System.Windows.Data;
using System.Globalization;

namespace WCS.Shared.Converters
{
    [ValueConversion(typeof(object), typeof(string))]
    public class WardCodeConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
			if (value == null) return "";

            string result = value.ToString();

            if (String.CompareOrdinal(result, "ACC1") == 0)
            {
                result = "MT";
            }
            else if (String.CompareOrdinal(result, "ACC2") == 0)
            {
                result = "JP";
            }
            else if (String.CompareOrdinal(result, "ACCG") == 0)
            {
                result = "OM";
            }
            else if (String.CompareOrdinal(result, "ACCLG") == 0)
            {
                result = "FR";
            }
            else if (String.CompareOrdinal(result, "ACC3") == 0)
            {
                result = "OLOK";
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return parameter;
        }
    }
}

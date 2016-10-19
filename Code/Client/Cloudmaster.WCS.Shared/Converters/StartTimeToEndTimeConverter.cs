using System;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Media;

using WCS.Core;

namespace WCS.Shared.Converters
{
    public class StartTimeToEndTimeConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Count() != 2)
                throw new ArgumentException("Wrong Argument amount");
            if (!(values[0] is TimeSpan) || !(values[1] is TimeSpan))
                return "";

            return (((TimeSpan)values[0]).Add((TimeSpan)values[1])).ToWcsHoursMinutesFormat();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

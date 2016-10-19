using System;
using System.Windows.Data;
using System.Globalization;
using System.Windows;
using WCS.Shared.Schedule;
using System.Windows.Media;

namespace WCS.Shared.Converters
{
    [ValueConversion(typeof(double), typeof(ScreenSelectionType))]
    public class BooleanToSelectedMargin : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return new Thickness(0, 0, 0, 0);

            switch ((ScreenSelectionType)value)
            {
                case ScreenSelectionType.Selected:
                    return new Thickness(0, 0, 0, 1);
                case ScreenSelectionType.DeSelected:
                    return new Thickness(0, 0, 0, 0);
                default:
                    return new Thickness(0, 0, 0, 0);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}

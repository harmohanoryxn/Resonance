using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WCS.Shared.Converters
{
    [ValueConversion(typeof(int), typeof(Visibility))]
    public class ViewModeToVisibilityConverter : IValueConverter
    {
        public object Convert(
            object value
            , Type targetType
            , object parameter
            , CultureInfo culture
            )
        {
            Visibility result = Visibility.Collapsed;

            int displayOnViewIndex = int.Parse((string)parameter);

            int currentViewIndex = (int)value;

            if (currentViewIndex == displayOnViewIndex)
            {
                result = Visibility.Visible;
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}

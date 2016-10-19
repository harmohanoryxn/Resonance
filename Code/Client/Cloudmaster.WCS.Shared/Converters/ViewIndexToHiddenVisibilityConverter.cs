using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Cloudmaster.WCS.Controls.Converters;

namespace WCS.Shared.Converters
{
    [ValueConversion(typeof(int), typeof(Visibility))]
    public class ViewIndexToHiddenVisibilityConverter : IValueConverter
    {
        public object Convert(
            object value
            , Type targetType
            , object parameter
            , CultureInfo culture
            )
        {
            ConverterHelper.ValidateArguments(
                value
                , false
                , typeof(int)
                , targetType
                , typeof(Visibility)
                , parameter
                , typeof(String)
                );

            Visibility result = Visibility.Hidden;

            int displayOnViewIndex = int.Parse ((string) parameter);

            int currentViewIndex = (int) value;

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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using System.Globalization;

namespace Cloudmaster.WCS.Controls.Converters
{
    [ValueConversion(typeof(object), typeof(Color))]
    public class PercentageToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Brush result;

            double percentage = int.Parse(value.ToString());

            double minThreshold = 50;

            percentage = Math.Max(percentage - minThreshold, 0) * 2;

            Color startColor = Colors.Red;
            Color endColor = Colors.Lime;

            int rMax = endColor.R;
            int rMin = startColor.R;

            int bMax = endColor.B;
            int bMin = startColor.B;

            int gMax = endColor.G;
            int gMin = startColor.G;

            var rAverage = rMin + (int)((rMax - rMin) * percentage / 100);
            var gAverage = gMin + (int)((gMax - gMin) * percentage / 100);
            var bAverage = bMin + (int)((bMax - bMin) * percentage / 100);

            Color color = Color.FromRgb((byte) rAverage, (byte) gAverage, (byte) bAverage);

            result = new SolidColorBrush(color);

            return result;            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}

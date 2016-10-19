using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Cloudmaster.WCS.Classes.Helpers
{
    public class ColorHelper
    {
        public static Color GetColorBasedOnPercentageOfLinearGradient(double percentage, string startValue, string endValue)
        {
            Color startColor = (Color) ColorConverter.ConvertFromString(startValue);
            Color endColor = (Color) ColorConverter.ConvertFromString(endValue);

            return GetColorBasedOnPercentageOfLinearGradient(percentage, startColor, endColor);
        }

        public static Brush GetBrushBasedOnPercentageOfLinearGradient(double percentage, SolidColorBrush startValue, SolidColorBrush endValue)
        {
            Color startColor = startValue.Color;
            Color endColor = endValue.Color;

            return GetBrushBasedOnPercentageOfLinearGradient(percentage, startColor, endColor);
        }

        public static Brush GetBrushBasedOnPercentageOfLinearGradient(double percentage, Color startColor, Color endColor)
        {
            Brush result;

            Color color = GetColorBasedOnPercentageOfLinearGradient(percentage, startColor, endColor);

            result = new SolidColorBrush(color);

            return result; 
        }

        public static Color GetColorBasedOnPercentageOfLinearGradient(double percentage, Color startColor, Color endColor)
        {
            Color result;
             
            int rMax = endColor.R;
            int rMin = startColor.R;

            int bMax = endColor.B;
            int bMin = startColor.B;

            int gMax = endColor.G;
            int gMin = startColor.G;

            var rAverage = rMin + (int)((rMax - rMin) * percentage / 100);
            var gAverage = gMin + (int)((gMax - gMin) * percentage / 100);
            var bAverage = bMin + (int)((bMax - bMin) * percentage / 100);

            result = Color.FromRgb((byte)rAverage, (byte)gAverage, (byte)bAverage);

            return result;
        }
    }
}

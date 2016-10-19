using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Globalization;
using Cloudmaster.WCS.Classes.Helpers;

namespace Cloudmaster.WCS.Controls
{
    public class AlertTimer : Control
    {
        private static Pen LinePen;

        private static Brush OuterBrush;

        private static Brush InnerBrush;

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(AlertTimer), new UIPropertyMetadata(string.Empty));

        public Brush TimerBrush
        {
            get { return (Brush)GetValue(TimerBrushProperty); }
            set { SetValue(TimerBrushProperty, value); }
        }

        public static readonly DependencyProperty TimerBrushProperty =
            DependencyProperty.Register("TimerBrush", typeof(Brush), typeof(AlertTimer), new UIPropertyMetadata(Brushes.DarkRed));

        public double Degrees
        {
            get { return (double)GetValue(DegreesProperty); }
            set { SetValue(DegreesProperty, value); }
        }

        public static readonly DependencyProperty DegreesProperty =
            DependencyProperty.Register("Degrees", typeof(double), typeof(AlertTimer), new UIPropertyMetadata(0.0));

        static AlertTimer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AlertTimer), new FrameworkPropertyMetadata(typeof(AlertTimer)));

            LinePen = new Pen(Brushes.Black, 2.0);

            LinePen.StartLineCap = PenLineCap.Round;
            LinePen.EndLineCap = PenLineCap.Round;

            LinePen.Freeze();

            Color startColor = (Color) ColorConverter.ConvertFromString("#40AD83");
            Color endColor = (Color) ColorConverter.ConvertFromString("#255F47");

            OuterBrush = new LinearGradientBrush(startColor, endColor, 90.0);
            OuterBrush.Freeze();

            InnerBrush = Brushes.WhiteSmoke;
            InnerBrush.Freeze();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            double percentage = (Degrees / 360) * 100;

            Color timerStartColor = ColorHelper.GetColorBasedOnPercentageOfLinearGradient(percentage, "#D3D3AB", "#D63535");
            Color timerEndColor = ColorHelper.GetColorBasedOnPercentageOfLinearGradient(percentage, "#BC9E66", "DarkRed");

            TimerBrush = new LinearGradientBrush(timerStartColor, timerEndColor, 90.0);

            DrawAlert(drawingContext, Width, Height, TimerBrush, Degrees);

            DrawText(drawingContext, Text);
        }

        public static void DrawAlert(DrawingContext context, double width, double height, Brush TimerBrush, double degrees)
        {
            Point centrePoint = new Point(0, 0);

            double outerBorderHeight = 140;

            double radiusX = width / 2;
            double radiusY = height / 2;

            double innerRadiusX = radiusX - outerBorderHeight;
            double innerRadiusY = radiusY - outerBorderHeight;

            context.DrawEllipse(OuterBrush, LinePen, centrePoint, radiusX, radiusY);
            context.DrawEllipse(InnerBrush, LinePen, centrePoint, innerRadiusX, innerRadiusY);

            DrawCurve(context, width, height, TimerBrush, degrees);
        }

        private static Typeface TextTypeFace = new Typeface("Segoe UI");

        public void DrawText(DrawingContext context, string text)
        {
            FormattedText displayNameText = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, TextTypeFace, 162, Brushes.Black);

            double halfDisplayNameTextWidth = displayNameText.Width / 2;
            double halfDisplayNameTextHeight = displayNameText.Height / 2;

            context.DrawText(displayNameText, new Point(-halfDisplayNameTextWidth, -halfDisplayNameTextHeight-20));
        }

        private static void DrawCurve(DrawingContext context, double width, double height, Brush TimerBrush, double degrees)
        {
            int isLarge = (degrees > 180) ? 1 : 0;

            double radians = degrees * 0.0174532925;

            double timerBorderExtraHeight = 0;

            double timerBorderHeight = 140;

            double radiusX = (width / 2) + timerBorderExtraHeight;
            double radiusY = (height / 2) + timerBorderExtraHeight;

            double radiusX2 = radiusX - timerBorderHeight;
            double radiusY2 = radiusY - timerBorderHeight;

            double y0 = -radiusY + timerBorderHeight;
            double y1 = -radiusY;

            double x2 = ((radiusX) * Math.Sin(radians));
            double y2 = - ((radiusY) * Math.Cos(radians));

            double x3 = ((radiusX2) * Math.Sin(radians));
            double y3 = - ((radiusY2) * Math.Cos(radians));

            string curvedPath = string.Format("M 0 {11} L 0 {12} A {0} {1} {2} {8} 1 {3} {4} L {6} {7} A {9} {10} {2} {8} 0 0 {11} Z", radiusX, radiusY, degrees, x2, y2, timerBorderHeight, x3, y3, isLarge, radiusX2, radiusY2, y0, y1);


            Geometry curvedGeometry = Geometry.Parse(curvedPath);

            curvedGeometry.Freeze();

            context.DrawGeometry(TimerBrush, LinePen, curvedGeometry);
        }
    }
}

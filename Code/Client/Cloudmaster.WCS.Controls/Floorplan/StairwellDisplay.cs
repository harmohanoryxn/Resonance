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

namespace Cloudmaster.WCS.Controls
{
    public class StairwellDisplay : Control
    {
        private static Pen OutlinePen;

        private static Brush OddBrush;

        private static Brush EvenBrush;

        private static Brush CentreBrush;

        static StairwellDisplay()
        {
            OutlinePen = new Pen(Brushes.Black, 1.5);
            OutlinePen.Freeze();

            OddBrush = Brushes.Silver;
            OddBrush.Freeze();

            EvenBrush = new SolidColorBrush(Color.FromRgb((byte)100, (byte)100, (byte)100));
            EvenBrush.Freeze();

            CentreBrush = Brushes.Silver;
            CentreBrush.Freeze();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            Draw(drawingContext, Width, Height, string.Empty);
        }

        public static void Draw(DrawingContext context, double width, double height, string style)
        {
            switch (style)
            {
                case "Round":

                    DrawRound(context, width, height);

                    break;

                default:

                    DrawRectangularStairwell(context, width, height);

                    break;
            }
        }

        public static void DrawRound(DrawingContext context, double width, double height)
        {
            Brush brushToUse;

            double increment = 12;

            bool isOdd = false;

            for (double i = 0; i < 270; i += increment)
            {
                double radians1 = i * 0.0174532925;
                double radians2 = (i + increment) * 0.0174532925;

                double centerX = width / 2;
                double centerY = height / 2;

                double x1 = centerX + ((centerX) * Math.Cos(radians1));
                double y1 = centerY + ((centerY) * Math.Sin(radians1));

                double x2 = centerX + ((centerX) * Math.Cos(radians2));
                double y2 = centerY + ((centerY) * Math.Sin(radians2));

                brushToUse = (isOdd) ? OddBrush : EvenBrush;

                string path = string.Format("M {0} {1} L {2} {3} A {0} {1} {4} 0 1 {5} {6} Z", centerX, centerY, x1, y1, increment, x2, y2);

                Geometry geometry = Geometry.Parse(path);

                context.DrawGeometry(brushToUse, OutlinePen, geometry);

                isOdd = !isOdd;

                context.DrawEllipse(CentreBrush, OutlinePen, new Point(centerX, centerY), centerX / 3, centerY / 3);
            }
        }

        private static void DrawRectangularStairwell(DrawingContext context, double width, double height)
        {
            Brush brushToUse;

            double current = 0;

            bool isOdd = false;

            while (current < width - 100)
            {
                current += 50;

                Rect rect = new Rect(current, 0, 50, height);

                brushToUse = (isOdd) ? OddBrush : EvenBrush;

                context.DrawRectangle(brushToUse, OutlinePen, rect);

                isOdd = !isOdd;
            }

            current += 50;

            string path = string.Format("M {0} {2} L {1} {2} L {0} 0", current, current + 100, height);

            Geometry geometry = Geometry.Parse(path);

            geometry.Freeze();

            brushToUse = (isOdd) ? OddBrush : EvenBrush;

            context.DrawGeometry(brushToUse, OutlinePen, geometry);

            isOdd = !isOdd;

            path = string.Format("M {0} {3} L {1} {3} L {0} {2}", current + 50, current + 100, height / 2, height);

            geometry = Geometry.Parse(path);

            geometry.Freeze();

            brushToUse = (isOdd) ? OddBrush : EvenBrush;

            context.DrawGeometry(brushToUse, OutlinePen, geometry);
        }

    }
}

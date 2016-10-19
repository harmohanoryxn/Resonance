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
    public class TableDisplay : Control
    {
        private static Pen OutlinePen;

        private static Brush TableTopBrush;

        private static Brush CounterTopBrush;

        static TableDisplay()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TableDisplay), new FrameworkPropertyMetadata(typeof(TableDisplay)));

            OutlinePen = new Pen(Brushes.Black, 1.5);
            OutlinePen.Freeze();

            TableTopBrush = new BrushConverter().ConvertFromString("#A9895A") as SolidColorBrush;
            TableTopBrush.Freeze();

            CounterTopBrush = Brushes.Silver;
            CounterTopBrush.Freeze();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            DrawTable(drawingContext, Width, Height, string.Empty);
        }

        public static void DrawTable(DrawingContext context, double width, double height, string style)
        {
            switch (style)
            {
                case "Round":

                    context.DrawEllipse(TableTopBrush, OutlinePen, new Point(0,0), width, height);

                    break;

                case "Curved":

                    double degrees = width;
                    int isLarge = (degrees > 180) ? 1 : 0;

                    double radians = degrees * 0.0174532925;

                    double radiusX = 500;
                    double radiusY = 500;

                    double radiusX2 = radiusX - height;
                    double radiusY2 = radiusY - height;

                    double x2 = ((radiusX) * Math.Sin(radians));
                    double y2 = radiusY - ((radiusY) * Math.Cos(radians));

                    double x3 = ((radiusX2) * Math.Sin(radians));
                    double y3 = radiusY - ((radiusY2) * Math.Cos(radians));

                    string curvedPath = string.Format("M 0 {5} L 0 0 A {0} {1} {2} {8} 1 {3} {4} L {6} {7} A {9} {10} {2} {8} 0 0 {5} Z", radiusX, radiusY, degrees, x2, y2, height, x3, y3, isLarge, radiusX2, radiusY2);


                    Geometry curvedGeometry = Geometry.Parse(curvedPath);

                    curvedGeometry.Freeze();

                    context.DrawGeometry(TableTopBrush, OutlinePen, curvedGeometry);

                    break;

                case "L":

                    double depth = 120;

                    double widthMinusDepth = width - depth;
                    double heightMinusDepth = height - depth;

                    double widthMinusTwiceDepth = width - (depth * 2);
                    double heightMinusTwiceDepth = height - (depth * 2);

                    // 0 - width
                    // 1 - height
                    // 2 - widthMinusDepth
                    // 3 - heightMinusDepth
                    // 4 - widthMinusTwiceDepth
                    // 5 - heightMinusTwiceDepth

                    string path = string.Format("M 0 {3} L {4} {3} Q {2} {3} {2} {5} L {2} 0 L {0} 0 L {0} {5} Q {0} {1} {4} {1} L 0 {1} Z", width, height, widthMinusDepth, heightMinusDepth, widthMinusTwiceDepth, heightMinusTwiceDepth);

                    Geometry geometry = Geometry.Parse(path);

                    geometry.Freeze();

                    context.DrawGeometry(CounterTopBrush, OutlinePen, geometry);

                    break;

                default:

                    Rect tableRect = new Rect(0, 0, width, height);

                    context.DrawRectangle(TableTopBrush, OutlinePen, tableRect);

                    break;
            }
        }

    }
}

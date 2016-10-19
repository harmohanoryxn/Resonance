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
    public class WallDisplay : Control
    {
        private static Pen outerPen;
        private static Pen innerPen;

        private static Brush CounterTopBrush;

        static WallDisplay()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WallDisplay), new FrameworkPropertyMetadata(typeof(WallDisplay)));

            Brush outerPenBrush = new SolidColorBrush(Color.FromRgb((byte)62, (byte)62, (byte)62));
            outerPenBrush.Freeze();

            outerPen = new Pen(outerPenBrush, 32.0);

            outerPen.StartLineCap = PenLineCap.Round;
            outerPen.EndLineCap = PenLineCap.Round;

            outerPen.Freeze();

            Brush innerPenBrush = Brushes.WhiteSmoke;
            innerPenBrush.Freeze();

            innerPen = new Pen(innerPenBrush, 24.0);

            innerPen.StartLineCap = PenLineCap.Round;
            innerPen.EndLineCap = PenLineCap.Round;

            innerPen.Freeze();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            DrawWall(drawingContext, Height);
        }

        public static void DrawWall(DrawingContext context, double height)
        {
            context.DrawLine (outerPen, new Point (0,0), new Point(0, height));
            context.DrawLine (innerPen, new Point (0,0), new Point(0, height));
        }
    }
}

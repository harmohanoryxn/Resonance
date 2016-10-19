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
    public class EntranceDisplay : Control
    {
        private static Pen OutlinePen;

        private static Brush EntanceBrush;

        static EntranceDisplay()
        {
            OutlinePen = new Pen(Brushes.Black, 1.5);
            OutlinePen.Freeze();

            EntanceBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#88FFFFFF"));
            EntanceBrush.Freeze();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            Draw(drawingContext, Width, Height);
        }

        public static void Draw(DrawingContext context, double width, double height)
        {
            context.DrawEllipse(EntanceBrush, OutlinePen, new Point(0, 0), width, height);
        }

    }
}

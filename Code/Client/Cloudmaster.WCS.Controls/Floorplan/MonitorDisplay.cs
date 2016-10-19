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
    public class MonitorDisplay : Control 
    {
        private static Pen OutlinePen;

        private static Brush ScreenBrush;

        private static Brush BackBrush;

        private static Brush BaseBrush;

        private static Brush StandBrush;

        static MonitorDisplay()
        {
            OutlinePen = new Pen(Brushes.Black, 1.5);
            OutlinePen.Freeze();

            ScreenBrush = Brushes.Silver;
            ScreenBrush.Freeze();

            BackBrush = new SolidColorBrush(Color.FromRgb((byte)100, (byte)100, (byte)100));
            BackBrush.Freeze();

            BaseBrush = new SolidColorBrush(Color.FromRgb((byte)68, (byte)68, (byte)68));
            BaseBrush.Freeze();

            StandBrush = new SolidColorBrush(Color.FromRgb((byte)90, (byte)90, (byte)90));
            StandBrush.Freeze();
        }

        protected override void OnRender(DrawingContext dc)
        {
            DrawMonitor(dc);
        }

        public static void  DrawMonitor(DrawingContext context)
        {
            Rect backRect = new Rect (-5, -46, 20, 92);
            Rect screenRect = new Rect (5,-50, 10, 100);
            Rect standRect = new Rect (-10,-25, 5, 50);

            context.DrawEllipse(BaseBrush, OutlinePen, new Point (0,0), 30, 30);
            context.DrawRoundedRectangle(BackBrush, OutlinePen, backRect, 4, 4);
            context.DrawRectangle(ScreenBrush, OutlinePen, screenRect);
            context.DrawRectangle(StandBrush, OutlinePen, standRect);
        }
    }
}

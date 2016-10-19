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
    public class ToiletDisplay : Control 
    {
        private static Pen OutlinePen;

        private static Brush SeatBrush;

        private static Brush BackRestBrush;

        static ToiletDisplay()
        {
            OutlinePen = new Pen(Brushes.Black, 1.5);
            OutlinePen.Freeze();

            SeatBrush = Brushes.GhostWhite;
            SeatBrush.Freeze();

            BackRestBrush = Brushes.GhostWhite;
            BackRestBrush.Freeze();
        }

        protected override void OnRender(DrawingContext dc)
        {
            Draw(dc);
        }

        public static void  Draw(DrawingContext context)
        {
            Rect seatRect = new Rect(-60, -60, 120, 120);
            Rect backRestRect = new Rect(-60, -60, 30, 120);

            context.DrawEllipse(SeatBrush, OutlinePen, new Point(0,0), 60, 50);
            context.DrawEllipse(SeatBrush, OutlinePen, new Point(0, 0), 40, 30);
            context.DrawRoundedRectangle(BackRestBrush, OutlinePen, backRestRect, 20, 20);
        }
    }
}

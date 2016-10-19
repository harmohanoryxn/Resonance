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
    public class WoodenSeatDisplay : Control 
    {
        private static Pen OutlinePen;

        private static Brush SeatBrush;

        private static Brush BackRestBrush;

        static WoodenSeatDisplay()
        {
            OutlinePen = new Pen(Brushes.Black, 1.5);
            OutlinePen.Freeze();

            SeatBrush = Brushes.DarkGoldenrod;
            SeatBrush.Freeze();

            BackRestBrush = Brushes.Goldenrod;
            BackRestBrush.Freeze();
        }

        protected override void OnRender(DrawingContext dc)
        {
            DrawSeat(dc);
        }

        public static void  DrawSeat (DrawingContext context)
        {
            Rect seatRect = new Rect(-40, -40, 80, 80);
            Rect backRestRect = new Rect(-40, -40, 20, 80);

            context.DrawRectangle(SeatBrush, OutlinePen, seatRect);
            context.DrawRoundedRectangle(BackRestBrush, OutlinePen, backRestRect, 4, 4);
        }
    }
}

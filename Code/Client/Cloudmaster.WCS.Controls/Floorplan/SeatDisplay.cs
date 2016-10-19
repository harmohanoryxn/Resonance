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
    public class SeatDisplay : Control 
    {
        private static Pen OutlinePen;

        private static Brush SeatBrush;

        private static Brush BackRestBrush;

        static SeatDisplay()
        {
            OutlinePen = new Pen(Brushes.Black, 1.5);
            OutlinePen.Freeze();

            SeatBrush = Brushes.LightSteelBlue;
            SeatBrush.Freeze();

            BackRestBrush = Brushes.SlateGray;
            BackRestBrush.Freeze();
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            DrawSeat(dc);
        }

        public static void  DrawSeat (DrawingContext context)
        {
            Rect seatRect = new Rect(-60, -60, 120, 120);
            Rect backRestRect = new Rect(-60, -60, 30, 120);

            context.DrawRoundedRectangle(SeatBrush, OutlinePen, seatRect, 10, 10);
            context.DrawRoundedRectangle(BackRestBrush, OutlinePen, backRestRect, 10, 10);
        }
    }
}

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
    public class CouchDisplay : Control 
    {
        private static Pen OutlinePen;

        private static Brush SeatBrush;

        private static Brush BackRestBrush;

        static CouchDisplay()
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
            Draw(dc);
        }

        public static void Draw (DrawingContext context)
        {
            Rect seatRect1 = new Rect(-40, -180, 100, 120);
            Rect seatRect2 = new Rect(-40, -60, 100, 120);
            Rect seatRect3 = new Rect(-40, 60, 100, 120);
            Rect backRestRect = new Rect(-60, -200, 120, 400);

            context.DrawRoundedRectangle(BackRestBrush, OutlinePen, backRestRect, 10, 10);

            context.DrawRectangle(SeatBrush, OutlinePen, seatRect1);
            context.DrawRectangle(SeatBrush, OutlinePen, seatRect2);
            context.DrawRectangle(SeatBrush, OutlinePen, seatRect3);
        }
    }
}

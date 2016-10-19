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
    public class BedDisplay : Control 
    {
        private static Pen OutlinePen;

        private static Brush QuiltBrush;

        private static Brush SheetBrush;

        private static Brush PillowBrush;

        private static Brush BedBrush;

        static BedDisplay()
        {
            OutlinePen = new Pen(Brushes.Black, 1.5);
            OutlinePen.Freeze();

            QuiltBrush = Brushes.LightSteelBlue;
            QuiltBrush.Freeze();

            SheetBrush = Brushes.WhiteSmoke;
            SheetBrush.Freeze();

            PillowBrush = Brushes.Lavender;
            PillowBrush.Freeze();

            BedBrush = Brushes.WhiteSmoke;
            BedBrush.Freeze();
        }

        protected override void OnRender(DrawingContext dc)
        {
            DrawBed(dc);
        }

        public static void DrawBed(DrawingContext context)
        {
            Rect bedRect = new Rect (-130,-70, 260, 140);
            Rect quiltRect = new Rect (-50,-70, 180, 140);
            Rect sheetRect = new Rect(-50, -70, 40, 140);
            Rect pillowRect = new Rect (-115,-50, 50, 100);

            context.DrawRoundedRectangle(BedBrush, OutlinePen, bedRect, 8, 8);
            context.DrawRoundedRectangle(QuiltBrush, OutlinePen, quiltRect, 8, 8);
            context.DrawRectangle(SheetBrush, OutlinePen, sheetRect);
            context.DrawRoundedRectangle(PillowBrush, OutlinePen, pillowRect, 6, 6);
        }
    }
}

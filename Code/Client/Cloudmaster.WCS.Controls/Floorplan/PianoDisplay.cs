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
    public class PianoDisplay : Control 
    {
        private static Pen OutlinePen;

        private static Brush TopBrush;

        private static Brush KeysBrush;

        static PianoDisplay()
        {
            OutlinePen = new Pen(Brushes.Black, 1.5);
            OutlinePen.Freeze();

            TopBrush = new SolidColorBrush(Color.FromRgb((byte)32, (byte)32, (byte)32));
            TopBrush.Freeze();

            KeysBrush = Brushes.GhostWhite;
            KeysBrush.Freeze();
        }

        protected override void OnRender(DrawingContext dc)
        {
            Draw(dc);
        }

        public static void  Draw(DrawingContext context)
        {
            string path = "M 0 300 L 0 150 A 125 125 180 0 1 250 150 A 25 25 90 0 0 275 175 A 25 25 90 0 1 300 200 L 300 300 Z";

            Geometry geometry = Geometry.Parse(path);

            geometry.Freeze();

            context.DrawGeometry(TopBrush, OutlinePen, geometry);

            Rect keysRect = new Rect(20, 250, 260, 50);

            context.DrawRectangle(KeysBrush, OutlinePen, keysRect);
        }
    }
}

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
    public class DoorDisplay : Control 
    {
        public static Pen DoorPen;

        public static Brush DoorPenBrush;

        public static Brush DoorBrush;

         static DoorDisplay()
        {
            DoorBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#88FFFFFF"));
            DoorBrush.Freeze();

            DoorPenBrush = new SolidColorBrush(Color.FromRgb((byte)32, (byte)32, (byte)32));
            DoorPenBrush.Freeze();

            DoorPen = new Pen(DoorPenBrush, 2.0);
            DoorPen.Freeze();
        }

        protected override void OnRender(DrawingContext dc)
        {
            Draw(dc);
        }

        private static double DoorWidth = 140;

        public static void  Draw (DrawingContext context)
        {
            string path = string.Format("M 0 0 Q {0} 0 {0} {0} L 0 {0}", DoorWidth);

            context.DrawGeometry(DoorBrush, DoorPen, Geometry.Parse(path));
        }
    }
}

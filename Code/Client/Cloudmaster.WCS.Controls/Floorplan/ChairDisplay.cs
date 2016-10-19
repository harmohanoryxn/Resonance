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
    public class ChairDisplay : Control 
    {
        private static Pen OutlinePen;

        private static Brush CushionBrush;

        private static Brush ArmrestsBrush;

        private static Brush BackRestBrush;

        private static Geometry CushionGeometry;

        private static Rect TopArmrestRectangle;

        private static Rect BottomArmrestRectangle;

        private static Geometry BackRestGeometry;

        static ChairDisplay()
        {
            //DefaultStyleKeyProperty.OverrideMetadata(typeof(ChairDisplay), new FrameworkPropertyMetadata(typeof(ChairDisplay)));

            OutlinePen = new Pen(Brushes.Black, 1);
            OutlinePen.Freeze();

            CushionBrush = new SolidColorBrush(Color.FromRgb((byte)100, (byte)100, (byte)100));
            CushionBrush.Freeze();

            ArmrestsBrush = Brushes.Gray;
            ArmrestsBrush.Freeze();

            BackRestBrush = Brushes.DimGray;
            BackRestBrush.Freeze();

            string path = string.Format("M 60 0 L 100 30 L 100 70 L 60 100 L 20 100 Q 0 100 0 80 L 0 20 Q 0 0 20 0 Z");

            CushionGeometry = Geometry.Parse (path);

            CushionGeometry.Freeze ();

            TopArmrestRectangle = new Rect(25, -8, 60, 16);
            BottomArmrestRectangle = new Rect(25, 92, 60, 16);

            path = string.Format("M 85 15 Q 82 5 105 15 Q 120 50 105 85 Q 82 95 85 85 Q 95 50 85 15 Z");

            BackRestGeometry = Geometry.Parse(path);

            BackRestGeometry.Freeze();
        }

        protected override void OnRender(DrawingContext dc)
        {
            DrawChair(dc);
        }

        public static void  DrawChair (DrawingContext context)
        {
            context.DrawGeometry(CushionBrush, OutlinePen, CushionGeometry);

            context.DrawGeometry(CushionBrush, OutlinePen, CushionGeometry);

            context.DrawRoundedRectangle(ArmrestsBrush, OutlinePen, TopArmrestRectangle, 8, 8);

            context.DrawRoundedRectangle(ArmrestsBrush, OutlinePen, BottomArmrestRectangle, 8, 8);

            context.DrawGeometry(ArmrestsBrush, OutlinePen, BackRestGeometry);
        }
    }
}

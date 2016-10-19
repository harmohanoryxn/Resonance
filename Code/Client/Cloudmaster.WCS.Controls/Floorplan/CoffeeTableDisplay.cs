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
    public class CoffeeTableDisplay : Control
    {
        private static Pen OutlinePen;

        private static Brush TableTopBrush;

        static CoffeeTableDisplay()
        {
            OutlinePen = new Pen(Brushes.Black, 1.5);
            OutlinePen.Freeze();

            TableTopBrush = Brushes.Goldenrod;
            TableTopBrush.Freeze();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            Draw (drawingContext);
        }

        public static void Draw(DrawingContext context)
        {
            context.PushTransform(new TranslateTransform(-100, 0));
            context.PushTransform(new RotateTransform(0));

            WoodenSeatDisplay.DrawSeat(context);

            context.Pop();
            context.Pop();

            context.PushTransform(new TranslateTransform(100, 0));
            context.PushTransform(new RotateTransform(180));

            WoodenSeatDisplay.DrawSeat(context);

            context.Pop();
            context.Pop();

            context.PushTransform(new TranslateTransform(0, -100));
            context.PushTransform(new RotateTransform(90));

            WoodenSeatDisplay.DrawSeat(context);

            context.Pop();
            context.Pop();

            context.PushTransform(new TranslateTransform(0, 100));
            context.PushTransform(new RotateTransform(270));

            WoodenSeatDisplay.DrawSeat(context);

            context.Pop();
            context.Pop();

            TableDisplay.DrawTable(context, 100, 100, "Round");
        }

    }
}

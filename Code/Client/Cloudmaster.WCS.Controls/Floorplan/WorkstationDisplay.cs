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
    public class WorkstationDisplay : Control
    {
        private static Pen OutlinePen;

        private static Brush TableTopBrush;

        static WorkstationDisplay()
        {
            
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            Draw(drawingContext);
        }

        public static void Draw(DrawingContext context)
        {
            context.PushTransform(new TranslateTransform(-100, -100));
            context.PushTransform(new RotateTransform(0));

            TableDisplay.DrawTable(context, 100, 200, string.Empty);

            context.Pop();
            context.Pop();

            context.PushTransform(new TranslateTransform(10, -50));
            context.PushTransform(new RotateTransform(0));

            ChairDisplay.DrawChair(context);

            context.Pop();
            context.Pop();

            context.PushTransform(new TranslateTransform(-50, 0));
            context.PushTransform(new RotateTransform(0));

            MonitorDisplay.DrawMonitor(context);

            context.Pop();
            context.Pop();
        }

    }
}

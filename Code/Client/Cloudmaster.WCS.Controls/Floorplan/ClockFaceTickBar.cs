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
using System.Windows.Controls.Primitives;

namespace Cloudmaster.WCS.Controls
{
    public class ClockFaceTickBar : TickBar
    {
        static ClockFaceTickBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ClockFaceTickBar), new FrameworkPropertyMetadata(typeof(ClockFaceTickBar)));
        }

        Pen ticksPen;

        public ClockFaceTickBar()
            : base()
        {
            ticksPen = new Pen(Brushes.WhiteSmoke, 1.5);

            ticksPen.Freeze();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            DrawTicks(drawingContext);
        }

        private void DrawTicks(DrawingContext drawingContext)
        {
            for (double i = 0; i < 360; i += 6)
            {
                double radians = i * 0.0174532925;

                double centerX = this.Width / 2;
                double centerY = this.Height / 2;

                double x1 = centerX + ((centerX - 15) * Math.Cos(radians));
                double y1 = centerY + ((centerY - 15) * Math.Sin(radians));

                double x2 = centerX + ((centerX - 5) * Math.Cos(radians));
                double y2 = centerY + ((centerY - 5) * Math.Sin(radians));

                double x3 = centerX + ((centerX - 3.75) * Math.Cos(radians));
                double y3 = centerY + ((centerY - 3.75) * Math.Sin(radians));

                if (i % 30 == 0)
                {
                    drawingContext.DrawLine(ticksPen, new Point(x1, y1), new Point(x3, y3));
                }
                else
                {
                    drawingContext.DrawEllipse(Brushes.WhiteSmoke, null, new Point(x2, y2), 1.25, 1.25);
                }
            }
        }
    }
}

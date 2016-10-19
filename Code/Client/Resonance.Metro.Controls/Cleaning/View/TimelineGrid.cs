using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Resonance.Metro.Controls.Cleaning.View
{
    public class TimelineGrid :TimelineControl
    {
        static TimelineGrid()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (TimelineGrid),
                                                     new FrameworkPropertyMetadata(typeof (TimelineGrid)));
        }

        public Pen VerticalMajorPen
        {
            get { return (Pen) GetValue(VerticalMajorPenProperty); }
            set { SetValue(VerticalMajorPenProperty, value); }
        }

        public static readonly DependencyProperty VerticalMajorPenProperty =
            DependencyProperty.Register("VerticalMajorPen", typeof (Pen), typeof (TimelineGrid),
                                        new UIPropertyMetadata(null));

        public Pen VerticalMinorPen
        {
            get { return (Pen) GetValue(VerticalMinorPenProperty); }
            set { SetValue(VerticalMinorPenProperty, value); }
        }

        public static readonly DependencyProperty VerticalMinorPenProperty =
            DependencyProperty.Register("VerticalMinorPen", typeof (Pen), typeof (TimelineGrid),
                                        new UIPropertyMetadata(null));

        protected override void OnRender(DrawingContext context)
        {
            if (ActualWidth > 0)
                DrawGrid(context);
        }

        protected void DrawGrid(DrawingContext context)
        {
            double timelineWidth = ActualWidth;

            double oneHourWidth = TimelineControl.GetOneHourWidth(timelineWidth, StartHour, EndHour);

            DrawGridLines(context, timelineWidth, oneHourWidth);
        }

        private void DrawGridLines(DrawingContext context, double gridWidth, double oneHourWidth)
        {
            double halfHourWidth = oneHourWidth/2;
            double quarterHourWidth = oneHourWidth/4;

            double currentX = 0;

            for (int hour = StartHour; hour < EndHour; hour++)
            {
                if (VerticalMajorPen != null)
                {
                    context.DrawLine(VerticalMajorPen, new Point(currentX, 0), new Point(currentX, ActualHeight));
                }

                if (VerticalMinorPen != null)
                {
                    context.DrawLine(VerticalMajorPen, new Point(currentX + halfHourWidth, 0),
                                     new Point(currentX + halfHourWidth, ActualHeight));
                }
            }
        }
    }
}

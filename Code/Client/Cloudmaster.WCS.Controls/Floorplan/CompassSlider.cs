using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace Cloudmaster.WCS.Controls
{
    public class CompassSlider : Slider
    {
        private const double degreesPerRadian = (180.0 / Math.PI);

        private const double snapThresholdInDegrees = 3;

        private const double snapToIntervalInDegrees = 45;

        private Boolean isDragging = false;

        private double startingDragAngleInDegrees;

        private double originalValueInDegrees;

        static CompassSlider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CompassSlider), new FrameworkPropertyMetadata(typeof(CompassSlider)));
        }

        public CompassSlider()
        {
            Minimum = 0;
            Maximum = 360;
            Value = 0;
        }

        Thumb draggableThumb;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            draggableThumb = base.GetTemplateChild("PART_Thumb") as Thumb;

            if (draggableThumb != null)
            {
                draggableThumb.DragDelta += new DragDeltaEventHandler(draggableThumb_DragDelta);
                draggableThumb.DragStarted += new DragStartedEventHandler(draggableThumb_DragStarted);
                draggableThumb.DragCompleted += new DragCompletedEventHandler(draggableThumb_DragCompleted);
            }
        }

        Point dragStartingOffset;

        void draggableThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            dragStartingOffset = new Point(e.HorizontalOffset, e.VerticalOffset);

            startingDragAngleInDegrees = CalculateAngleBetweenCentrePointAndDragOffset(dragStartingOffset);

            originalValueInDegrees = this.Value;

            isDragging = true;
        }

        void draggableThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Point currentDragOffset = new Point(dragStartingOffset.X + e.HorizontalChange, dragStartingOffset.Y + e.VerticalChange);

            double currentAngleInDegrees = CalculateAngleBetweenCentrePointAndDragOffset(currentDragOffset);

            double differenceInDegrees = currentAngleInDegrees - startingDragAngleInDegrees;

            double degrees = originalValueInDegrees - differenceInDegrees;

            if (degrees < 0)
            {
                degrees = 360 + degrees;
            }

            double snapToDegrees = SnapDegrees(degrees);

            this.Value = snapToDegrees % 360;
        }

        void draggableThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            isDragging = false;
        }

        private static double SnapDegrees(double degrees)
        {
            double remainder = degrees % snapToIntervalInDegrees;

            if (remainder < snapThresholdInDegrees)
            {
                degrees -= remainder;
            }
            else if (remainder > snapToIntervalInDegrees - snapThresholdInDegrees)
            {
                degrees += snapToIntervalInDegrees - remainder;
            }

            return Math.Round(degrees);
        }

        private double CalculateAngleBetweenCentrePointAndDragOffset(Point dragOffset)
        {
            Point centrePoint = GetCentrePoint();

            double x = dragOffset.X - centrePoint.X;
            double y = dragOffset.Y - centrePoint.Y;

            double angleInRadians = Math.Atan2(y, x);
            double angleInDegrees = angleInRadians * (180 / Math.PI);

            if (angleInDegrees < 0)
            {
                angleInDegrees = 360 + angleInDegrees;
            }

            double clockwiseAngleInDegrees = 360 - angleInDegrees;

            double rotatedAngleInDegrees = (90 + clockwiseAngleInDegrees) % 360;

            return rotatedAngleInDegrees;
        }

        private Point GetCentrePoint()
        {
            Point result;

            double centerX = this.Width / 2;
            double centerY = this.Height / 2;

            result = new Point(centerX, centerY);

            return result;
        }
    }
}

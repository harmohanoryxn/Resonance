using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Cloudmaster.WCS.Classes
{
    public static class PointExtensions
    {
        private const double snapToInterval = 100;

        private const double snapThreshold = 50;

        private static double SnapTo(double value)
        {
            double remainder = value % snapToInterval;

            // If the remainder is within the snap threshold on either side of the snap interval add or subtract it to the degrees 
            // so that degrees becomes equal to the snap interval.

            if (remainder < snapToInterval)
            {
                value -= remainder;
            }
            else if (remainder > snapToInterval - snapThreshold)
            {
                value += snapToInterval - remainder;
            }

            // Round degrees to the nearest integer

            return Math.Round(value);
        }


        public static Point SnapTo(this Point point)
        {
            double x = SnapTo(point.X);
            double y = SnapTo(point.Y);

            Point result = new Point(x, y);

            return result;
        }
    }
}

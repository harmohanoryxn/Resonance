using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Resonance.Metro.Controls.Cleaning.View
{
    public class TimelineControl : Control
    {
        static TimelineControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TimelineControl),
                                                     new FrameworkPropertyMetadata(typeof(TimelineControl)));
        }

        public int StartHour
        {
            get { return (int)GetValue(StartHourProperty); }
            set { SetValue(StartHourProperty, value); }
        }

        public static readonly DependencyProperty StartHourProperty =
            DependencyProperty.Register("StartHour", typeof(int), typeof(TimelineControl), new UIPropertyMetadata(0));

        public int EndHour
        {
            get { return (int)GetValue(EndHourProperty); }
            set { SetValue(EndHourProperty, value); }
        }

        public static readonly DependencyProperty EndHourProperty =
            DependencyProperty.Register("EndHour", typeof(int), typeof(TimelineControl), new UIPropertyMetadata(24));

        public static double GetOneHourWidth(double width, int startHourIn24HrsFormat, int endHourIn24HrsFormat)
        {
            if (width <= 0)
            {
                return 0;
            }
            else if (endHourIn24HrsFormat <= startHourIn24HrsFormat)
            {
                var message = string.Format("End hours {0} cannot be less than of equal to start hours {1}",
                                            endHourIn24HrsFormat, startHourIn24HrsFormat);
                throw new ArgumentException(message);
            }
            else if ((startHourIn24HrsFormat < 0) || (startHourIn24HrsFormat > 23))
            {
                var message = string.Format("Start hours {0} must be between 0 and 23 hours",
                            startHourIn24HrsFormat);

                throw new ArgumentException(message);
            }
            else if ((endHourIn24HrsFormat < 1) || (endHourIn24HrsFormat > 24))
            {
                var message = string.Format("Start hours {0}must be between 1 and 24 hours",
                            endHourIn24HrsFormat);

                throw new ArgumentException(message);
            }
            else if ((endHourIn24HrsFormat - startHourIn24HrsFormat) < 1)
            {
                var message = string.Format("End hour {0} must be greate than start hour {1}",
                            endHourIn24HrsFormat, startHourIn24HrsFormat);

                throw new ArgumentException(message);
            }

            double totalHoursInGrid = (endHourIn24HrsFormat - startHourIn24HrsFormat);

            double result = width / totalHoursInGrid;

            return result;
        }

    }
}

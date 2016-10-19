using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Cloudmaster.WCS.Classes.Helpers
{
    public class FrequencyHelper
    {
        public static SolidColorBrush ExpiredBrush;

        public static SolidColorBrush PendingBrush;

        public static SolidColorBrush ValidBrush;

        static FrequencyHelper()
        {
            ValidBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#84B1A2"));
            ValidBrush.Freeze();

            PendingBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C6A567"));
            PendingBrush.Freeze();

            ExpiredBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CA5A5A"));
            ExpiredBrush.Freeze();
        }

        public static Brush GetColorBasedOnFrquency (DateTime dateTimeToCompare, string frequency)
        {
            Brush result;

            if (FrequencyHelper.IsComplete(dateTimeToCompare, frequency))
            {
                result = ValidBrush;
            }
            else if (FrequencyHelper.IsExpired(dateTimeToCompare, frequency))
            {
                result = ExpiredBrush;
            }
            else
            {
                double percentage = FrequencyHelper.GetPercentageExpired(frequency);

                result = ColorHelper.GetBrushBasedOnPercentageOfLinearGradient(percentage, PendingBrush, ExpiredBrush);
            }

            return result;
        }


        public static double GetPercentageExpired(string frequency)
        {
            double result = 0;

            DateTime now = DateTime.Now;

            DateTime startDate = GetStartDate(frequency);

            switch (frequency)
            {
                case "Daily":

                    double currentMintue = ((now.Hour * 60) + now.Minute);

                    double totalMinutesInDay =  (24 * 60);

                    result = (currentMintue / totalMinutesInDay) * 100;

                    break;

                case "Monthly":

                    int totalDaysInMonth = DateTime.DaysInMonth(now.Year, now.Month);

                    int currentDay = now.Day;

                    result = (currentDay / totalDaysInMonth) * 100;

                    break;

                default:

                    throw new Exception("Invalid Frequency");
            }

            return result;
        }

        public static DateTime GetStartDate(string frequency)
        {
            DateTime result;

            DateTime now = DateTime.Now;

            switch (frequency)
            {
                case "Daily":

                    result = new DateTime(now.Year, now.Month, now.Day);

                    break;

                case "Monthly":

                    result = new DateTime(now.Year, now.Month, 1);

                    break;

                default:

                    throw new Exception ("Invalid Frequency");
            }

            return result;
        }

        public static DateTime GetEndDate(string frequency)
        {
            DateTime result;

            DateTime now = DateTime.Now;

            switch (frequency)
            {
                case "Daily":

                    result = new DateTime(now.Year, now.Month, now.Day).AddDays(1);

                    break;

                case "Monthly":

                    result = new DateTime(now.Year, now.Month, 0).AddMonths(1);

                    break;

                default:

                    throw new Exception("Invalid Frequency");
            }

            return result;
        }

        public static bool IsComplete(DateTime lastCleaned, string frequency)
        {
            bool result = false;

            DateTime startDateDue = FrequencyHelper.GetStartDate(frequency);

            result = (lastCleaned > startDateDue);

            return result;
        }

        public static bool IsExpired(DateTime lastCleaned, string frequency)
        {
            bool result = false;

            DateTime expiredDateDue = FrequencyHelper.GetStartDate(frequency).AddDays(-1);

            result = (lastCleaned < expiredDateDue);

            return result;
        }

        public static Brush GetBrushExpiredWithin6Hours(DateTime expirationDateTime)
        {
            Brush result = PendingBrush;

            double percentage = GetPercentageExpiredWithin6Hours(expirationDateTime);

            if (percentage > 0)
            {
                result = ColorHelper.GetBrushBasedOnPercentageOfLinearGradient(percentage, PendingBrush, ExpiredBrush);
            }

            return result;
        }

        public static double GetPercentageExpiredWithin6Hours(DateTime expirationDateTime)
        {
            double result = 0;

            DateTime now = DateTime.Now;

            if (now > expirationDateTime)
            {
                result = 100;
            }
            else
            {
                TimeSpan timeLeft = expirationDateTime - now;

                int minutesInFourHours = 6 * 60;

                double totalMinutesLeft = timeLeft.TotalMinutes;

                if (totalMinutesLeft > minutesInFourHours)
                {
                    result = 0;
                }
                else
                {
                    result = 100 - ((totalMinutesLeft / minutesInFourHours) * 100);
                }
            }

            return result;
        }
    }
}

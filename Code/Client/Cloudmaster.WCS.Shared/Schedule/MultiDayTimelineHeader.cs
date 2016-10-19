using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using WCS.Core;

namespace WCS.Shared.Schedule
{
	internal class MultiDayTimelineHeader : ScheduleBackgroundBase
	{
		//protected new double HeaderHeight = 20.0;

		static MultiDayTimelineHeader()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(MultiDayTimelineHeader), new FrameworkPropertyMetadata(typeof(MultiDayTimelineHeader)));
		}

		public static readonly DependencyProperty NumberExtraDaysProperty = DependencyProperty.RegisterAttached("NumberExtraDays", typeof(int), typeof(MultiDayTimelineHeader), new PropertyMetadata(3));
		public int NumberExtraDays
		{
			get { return (int)this.GetValue(NumberExtraDaysProperty); }
			set { this.SetValue(NumberExtraDaysProperty, value); }
		}

		public static readonly DependencyProperty NumberHoursDaysProperty = DependencyProperty.RegisterAttached("NumberHoursDays", typeof(int), typeof(MultiDayTimelineHeader), new PropertyMetadata(12));
		public int NumberHoursDays
		{
			get { return (int)this.GetValue(NumberHoursDaysProperty); }
			set { this.SetValue(NumberHoursDaysProperty, value); }
		}

		protected override void DrawGrid(DrawingContext context)
		{
			double actualWidth = ActualWidth - (HasScrollbar ? ScrollbarWidth : 0);

			double gridWidth = actualWidth - (HasInfobar ? PatientDataRailWidth : 0.0);

			double hourWidth = gridWidth / (NumberHoursDays + NumberExtraDays + (HasTbaHour ? 1 : 0));

			double halfHeaderHeight = HeaderHeight / 1.5;

			double tbaWidth = (HasTbaHour ? hourWidth : 0.0);

			// Draw Hours

			double currentX = (HasInfobar ? PatientDataRailWidth : 0.0) + tbaWidth;

			//	context.DrawLine(HeaderBorderPen, new Point((HasInfobar ? PatientDataRailWidth : 0.0), 0), new Point((HasInfobar ? PatientDataRailWidth : 0.0), HeaderHeight));

			for (int hour = 0; hour < NumberHoursDays; hour++)
			{
				var headerText = new FormattedText((DateTime.Now.AddHours(hour).RoundDownToNearestHour()).TimeOfDay.ToWcsHoursMinutesFormat(), CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, TextTypeFace, LargeFontSize, Brushes.RoyalBlue);
				context.DrawText(headerText, new Point(currentX + 5, 22));

				if (hour == 0)
				{
					headerText = new FormattedText(DateTime.Today.ToHeadlineTimeFormat(), CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, TextTypeFace, LargeFontSize, Brushes.RoyalBlue);
					context.DrawText(headerText, new Point(currentX + 5, 5));

					context.DrawLine(HeaderBorderPen, new Point(currentX, 5), new Point(currentX, HeaderHeight));
				}
				else
				{
					context.DrawLine(HeaderBorderPen, new Point(currentX, halfHeaderHeight), new Point(currentX, HeaderHeight));
				}

				currentX += hourWidth;
			} 
			
			for (int day = 1; day <= NumberExtraDays; day++)
			{
				var headerText = new FormattedText((DateTime.Now.AddDays(day)).ToHeadlineDateFormat(), CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, TextTypeFace, LargeFontSize, Brushes.RoyalBlue);
				context.DrawText(headerText, new Point(currentX + 5, 22));

				headerText = new FormattedText(DateTime.Today.AddDays(day).ToDayOfWeek(), CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, TextTypeFace, LargeFontSize, Brushes.RoyalBlue);
					context.DrawText(headerText, new Point(currentX + 5, 5));

					context.DrawLine(HeaderBorderPen, new Point(currentX, 5), new Point(currentX, HeaderHeight));
					//	context.DrawLine(HeaderBorderPen, new Point(currentX + halfHourWidth, halfHeaderHeight), new Point(currentX + halfHourWidth, HeaderHeight));
				 

				currentX += hourWidth;
			}

			context.DrawLine(HeaderBorderPen, new Point(currentX, 0), new Point(currentX, HeaderHeight));

		}

	}
}
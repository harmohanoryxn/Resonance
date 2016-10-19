using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using WCS.Core;

namespace WCS.Shared.Schedule
{
	internal class SingleDayTimelineHeader : ScheduleBackgroundBase
	{
		//protected new double HeaderHeight = 20.0;

		static SingleDayTimelineHeader()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(SingleDayTimelineHeader), new FrameworkPropertyMetadata(typeof(SingleDayTimelineHeader)));
		}
  
	
		protected override void DrawGrid(DrawingContext context)
		{
			double actualWidth = ActualWidth - (HasScrollbar ? ScrollbarWidth : 0);

			double gridWidth = actualWidth - (HasInfobar ? PatientDataRailWidth : 0.0);

			double hourWidth = gridWidth / (EndHour - StartHour + (HasTbaHour ? 1 : 0));

			double halfHeaderHeight = HeaderHeight / 1.5;

			double tbaWidth = (HasTbaHour ? hourWidth : 0.0);

			// Draw Hours

			double currentX = (HasInfobar ? PatientDataRailWidth : 0.0) + tbaWidth;

		//	context.DrawLine(HeaderBorderPen, new Point((HasInfobar ? PatientDataRailWidth : 0.0), 0), new Point((HasInfobar ? PatientDataRailWidth : 0.0), HeaderHeight));

			for (int hour = StartHour; hour < EndHour; hour++)
			{

				// Time Labels
				int hourToUse = (hour <= 12) ? hour : hour - 12;

				var headerText = new FormattedText(hourToUse.ToString(), CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, TextTypeFace, LargeFontSize, Brushes.RoyalBlue);
				context.DrawText(headerText, new Point(currentX + 5, 22));

				string ampm = "";
				if (hour == 5)
					ampm = DateTime.Today.ToHeadlineTimeFormat();
				else if (hour == 12)
					ampm = "PM";
				if (!string.IsNullOrEmpty(ampm))
				{
					headerText = new FormattedText(ampm, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, TextTypeFace, LargeFontSize, Brushes.RoyalBlue);
					context.DrawText(headerText, new Point(currentX + 5, 5));
				
					context.DrawLine(HeaderBorderPen, new Point(currentX, 5), new Point(currentX, HeaderHeight));
				//	context.DrawLine(HeaderBorderPen, new Point(currentX + halfHourWidth, halfHeaderHeight), new Point(currentX + halfHourWidth, HeaderHeight));
			
				}
				else
				{
					context.DrawLine(HeaderBorderPen, new Point(currentX, halfHeaderHeight), new Point(currentX, HeaderHeight));
				//	context.DrawLine(HeaderBorderPen, new Point(currentX + halfHourWidth, halfHeaderHeight), new Point(currentX + halfHourWidth, HeaderHeight));
				}

					currentX += hourWidth;
			}

			context.DrawLine(HeaderBorderPen, new Point(currentX, 0), new Point(currentX, HeaderHeight));

		}

	}
}
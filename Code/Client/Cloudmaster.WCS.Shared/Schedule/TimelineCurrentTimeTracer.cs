using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace WCS.Shared.Schedule
{
	public class TimelineCurrentTimeTracer : ScheduleBackgroundBase
	{
		static TimelineCurrentTimeTracer()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof (TimelineCurrentTimeTracer), new FrameworkPropertyMetadata(typeof (TimelineCurrentTimeTracer)));
		}

		public TimelineCurrentTimeTracer()
		{
			var timer = new DispatcherTimer();

			timer.Tick += UpdateTimeline_Tick;
			timer.Interval = TimeSpan.FromSeconds(60);
			timer.Start();
		}

		private void UpdateTimeline_Tick(object sender, EventArgs e)
		{
			InvalidateVisual();
		}


		protected override void DrawGrid(DrawingContext context)
		{
			double gridWidth = ActualWidth - (HasInfobar ? PatientDataRailWidth : 0.0) - (HasScrollbar ? ScrollbarWidth : 0);

			double hourWidth = GetOneHourWidth(gridWidth, HasTbaHour);

			double tbaWidth = hourWidth * (HasTbaHour ? 1 : 0);

			double dayPlusWidth = hourWidth*DayPlusOneHours;

			double timelineWidth = gridWidth - tbaWidth - dayPlusWidth;


			double timelineStartX = (HasInfobar ? PatientDataRailWidth : 0.0) + tbaWidth;

			DrawCurrentTime(context, timelineWidth, timelineStartX);
		}
 

		private void DrawCurrentTime(DrawingContext context, double timelineWidth, double startX)
		{
			if (DateTime.Now.TimeOfDay > TimeSpan.FromHours(4) && DateTime.Now.TimeOfDay < TimeSpan.FromMinutes(1439))
			{
				double currentMinutesX = GetXPositionForDateTime(timelineWidth, DateTime.Now);

				context.DrawRectangle(TryFindResource("CurrentTimeBrush") as Brush, null,
				                      new Rect(new Point(startX + currentMinutesX, 0),
				                               new Point(startX + currentMinutesX + 4, ActualHeight)));
			}
		}
	}
}

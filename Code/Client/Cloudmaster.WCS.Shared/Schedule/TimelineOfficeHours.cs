using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace WCS.Shared.Schedule
{
	public class TimelineOfficeHours : ScheduleBackgroundBase
	{
		static TimelineOfficeHours()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(TimelineOfficeHours), new FrameworkPropertyMetadata(typeof(TimelineOfficeHours)));
		}

		public static readonly DependencyProperty HideTBAProperty = DependencyProperty.RegisterAttached("HideTBA", typeof(bool), typeof(TimelineOfficeHours), new PropertyMetadata(false));
		public bool HideTBA
		{
			get { return (bool)this.GetValue(HideTBAProperty); }
			set { this.SetValue(HideTBAProperty, value); }
		}

		protected override void DrawGrid(DrawingContext context)
		{
			double gridWidth = ActualWidth - (HasInfobar ? PatientDataRailWidth : 0.0) - (HasScrollbar ? ScrollbarWidth : 0);

			double hourWidth = GetOneHourWidth(gridWidth, HasTbaHour);

			double tbaWidth = hourWidth * (HasTbaHour ? 1 : 0);


			double timelineStartX = (HasInfobar ? PatientDataRailWidth : 0.0) + tbaWidth;

			double startX = (HasInfobar ? PatientDataRailWidth : 0.0);

			DrawOutOfOfficeHours(context, timelineStartX, hourWidth);

			if (HasTbaHour && !HideTBA)
				DrawTBA(context, startX, hourWidth);
		}

		private void DrawOutOfOfficeHours(DrawingContext context, double startX, double hourWidth)
		{
			double startOutOfOfficeHours = StartOfficeHours - StartHour;

			if (startOutOfOfficeHours > 0)
			{
				var startOutOfOfficeHoursRect = new Rect(new Point(startX, 0), new Point(startX + (startOutOfOfficeHours * hourWidth), ActualHeight));

				context.DrawRectangle(TryFindResource("ScheduleOutsideOfficeHoursOverlayBrush") as Brush, null, startOutOfOfficeHoursRect);
			}

			double endOutOfOfficeHours = EndHour - EndOfficeHours;

			if (endOutOfOfficeHours > 0)
			{
				var firstX = startX + ((EndOfficeHours - StartHour) * hourWidth);
				var secondX = startX + ((EndHour - StartHour) * hourWidth);
				var endOutOfOfficeHoursRect = new Rect(new Point(firstX, 0), new Point(secondX, ActualHeight));

				context.DrawRectangle(TryFindResource("ScheduleOutsideOfficeHoursOverlayBrush") as Brush, null, endOutOfOfficeHoursRect);
			}
		}

		private void DrawTBA(DrawingContext context, double startX, double hourWidth)
		{
			var tbaRect = new Rect(new Point(startX, 0), new Point(startX + hourWidth, ActualHeight));

			context.DrawRectangle(TryFindResource("RequiresAttentionBrush") as Brush, null, tbaRect);
		}
	}
}

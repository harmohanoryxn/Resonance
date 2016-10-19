using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace WCS.Shared.Schedule
{
	internal class SingleDayTimelineGridlines : ScheduleBackgroundBase
	{
		static SingleDayTimelineGridlines()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(SingleDayTimelineGridlines), new FrameworkPropertyMetadata(typeof(SingleDayTimelineGridlines)));
		}

		public static readonly DependencyProperty ShowMinorTicksProperty = DependencyProperty.RegisterAttached("ShowMinorTicks", typeof(bool), typeof(SingleDayTimelineGridlines), new PropertyMetadata(true));
		public bool ShowMinorTicks
		{
			get { return (bool)this.GetValue(ShowMinorTicksProperty); }
			set { this.SetValue(ShowMinorTicksProperty, value); }
		}

		public SingleDayTimelineGridlines()
		{
			HasInfobar = false;

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
			double gridWidth = ActualWidth - (HasInfobar ? PatientDataRailWidth : 0.0);

			double hourWidth = GetOneHourWidth(gridWidth, HasTbaHour);

			double tbaWidth = hourWidth * (HasTbaHour ? 1 : 0);

			double dayPlusWidth = hourWidth*DayPlusOneHours;

			double timelineWidth = gridWidth - tbaWidth - dayPlusWidth;


			double timelineStartX = (HasInfobar ? PatientDataRailWidth : 0.0) + tbaWidth;


			DrawGridLines(context, timelineWidth, timelineStartX, hourWidth);
		}

		private void DrawGridLines(DrawingContext context, double gridWidth, double startX, double hourWidth)
		{

			double halfHourWidth = hourWidth/2;
			double quarterHourWidth = hourWidth/4;

			double currentX = startX;

			// Draw TBA heavy line
			//	context.DrawLine(heavyGridPen, new Point(currentX + hourWidth, 0), new Point(currentX + hourWidth, ActualHeight));

			for (int hour = StartHour; hour < EndHour; hour++)
			{
				// Grid Vertical Lines

				context.DrawLine(HeavyGridPen, new Point(currentX, 0), new Point(currentX, ActualHeight));
				if (ShowMinorTicks) context.DrawLine(HeavyGridPen, new Point(currentX + quarterHourWidth, ActualHeight / 2), new Point(currentX + quarterHourWidth, ActualHeight));
				context.DrawLine(HeavyGridPen, new Point(currentX + halfHourWidth, 0), new Point(currentX + halfHourWidth, ActualHeight));
				if (ShowMinorTicks) context.DrawLine(HeavyGridPen, new Point(currentX + halfHourWidth + quarterHourWidth, ActualHeight / 2), new Point(currentX + halfHourWidth + quarterHourWidth, ActualHeight));

				currentX += hourWidth;
			}


			//	context.DrawLine(heavyGridPen, new Point(currentX, 0), new Point(currentX, ActualHeight));

			// Draw TBA heavy line
			context.DrawLine(HeavyBorderPen, new Point(startX, 0), new Point(startX, ActualHeight));

			// Borders
			//	context.DrawLine(heavyBorderPen, new Point(PatientDataRailWidth, 0), new Point(PatientDataRailWidth, ActualHeight));
			//     context.DrawLine(heavyBorderPen, new Point(ActualWidth, 0), new Point(ActualWidth, ActualHeight));
		}

	}
}
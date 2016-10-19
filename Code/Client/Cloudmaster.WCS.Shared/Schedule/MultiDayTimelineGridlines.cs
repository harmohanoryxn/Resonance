using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace WCS.Shared.Schedule
{
	internal class MultiDayTimelineGridlines : ScheduleBackgroundBase
	{
		static MultiDayTimelineGridlines()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(MultiDayTimelineGridlines), new FrameworkPropertyMetadata(typeof(MultiDayTimelineGridlines)));
		}

		public static readonly DependencyProperty ShowMinorTicksProperty = DependencyProperty.RegisterAttached("ShowMinorTicks", typeof(bool), typeof(MultiDayTimelineGridlines), new PropertyMetadata(true));
		public bool ShowMinorTicks
		{
			get { return (bool)this.GetValue(ShowMinorTicksProperty); }
			set { this.SetValue(ShowMinorTicksProperty, value); }
		}

		public static readonly DependencyProperty NumberExtraDaysProperty = DependencyProperty.RegisterAttached("NumberExtraDays", typeof(int), typeof(MultiDayTimelineGridlines), new PropertyMetadata(3));
		public int NumberExtraDays
		{
			get { return (int)this.GetValue(NumberExtraDaysProperty); }
			set { this.SetValue(NumberExtraDaysProperty, value); }
		}

		public static readonly DependencyProperty NumberHoursDaysProperty = DependencyProperty.RegisterAttached("NumberHoursDays", typeof(int), typeof(MultiDayTimelineGridlines), new PropertyMetadata(12));
		public int NumberHoursDays
		{
			get { return (int)this.GetValue(NumberHoursDaysProperty); }
			set { this.SetValue(NumberHoursDaysProperty, value); }
		}

		public MultiDayTimelineGridlines()
		{
			HasInfobar = false;
		}

		protected override void DrawGrid(DrawingContext context)
		{
			double gridWidth = ActualWidth - (HasInfobar ? PatientDataRailWidth : 0.0);

			double hourWidth = gridWidth / (NumberHoursDays + NumberExtraDays + (HasTbaHour ? 1 : 0));

			double tbaWidth = hourWidth * (HasTbaHour ? 1 : 0);



			double timelineStartX = (HasInfobar ? PatientDataRailWidth : 0.0) + tbaWidth;


			DrawGridLines(context, timelineStartX, hourWidth);
		}

		private void DrawGridLines(DrawingContext context, double startX, double hourWidth)
		{

			double halfHourWidth = hourWidth/2;
			double quarterHourWidth = hourWidth/4;

			double currentX = startX;

			// Draw TBA heavy line
			//	context.DrawLine(heavyGridPen, new Point(currentX + hourWidth, 0), new Point(currentX + hourWidth, ActualHeight));

			for (int line = 0; line < NumberHoursDays+NumberExtraDays; line++)
			{
				// Grid Vertical Lines

				context.DrawLine(HeavyGridPen, new Point(currentX, 0), new Point(currentX, ActualHeight));
				if (ShowMinorTicks)
				{
					context.DrawLine(HeavyGridPen, new Point(currentX + quarterHourWidth, ActualHeight / 2), new Point(currentX + quarterHourWidth, ActualHeight));
					context.DrawLine(HeavyGridPen, new Point(currentX + halfHourWidth, 0), new Point(currentX + halfHourWidth, ActualHeight));
					context.DrawLine(HeavyGridPen, new Point(currentX + halfHourWidth + quarterHourWidth, ActualHeight / 2), new Point(currentX + halfHourWidth + quarterHourWidth, ActualHeight));
				}

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
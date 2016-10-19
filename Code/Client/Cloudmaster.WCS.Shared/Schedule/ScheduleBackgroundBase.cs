using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Globalization;

namespace WCS.Shared.Schedule
{
	public abstract class ScheduleBackgroundBase : Canvas
	{
		static ScheduleBackgroundBase()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ScheduleBackgroundBase), new FrameworkPropertyMetadata(typeof(ScheduleBackgroundBase)));
		}

		public static int StartHour = 5;
		public static int EndHour = 24;


		//protected int TBAHours = 1;

		public static readonly DependencyProperty HasTbaHourProperty = DependencyProperty.RegisterAttached("HasTbaHour", typeof(bool), typeof(ScheduleBackgroundBase), new PropertyMetadata(true));
		public bool HasTbaHour
		{
			get { return (bool)this.GetValue(HasTbaHourProperty); }
			set { this.SetValue(HasTbaHourProperty, value); }
		}

		public static readonly DependencyProperty HasScrollbarProperty = DependencyProperty.RegisterAttached("HasScrollbar", typeof(bool), typeof(ScheduleBackgroundBase), new PropertyMetadata(false));
		public bool HasScrollbar
		{
			get { return (bool)this.GetValue(HasScrollbarProperty); }
			set { this.SetValue(HasScrollbarProperty, value); }
		}

		public static readonly DependencyProperty HasInfobarProperty = DependencyProperty.RegisterAttached("HasInfobar", typeof(bool), typeof(ScheduleBackgroundBase), new PropertyMetadata(true));
		public bool HasInfobar
		{
			get { return (bool)this.GetValue(HasInfobarProperty); }
			set { this.SetValue(HasInfobarProperty, value); }
		}

		//public static bool GetRadius(DependencyObject obj)
		//{
		//    return (bool)obj.GetValue(HasTbaHourProperty);
		//}
		//public static void SetRadius(DependencyObject obj, bool value)
		//{
		//    obj.SetValue(HasTbaHourProperty, value);
		//}

		protected static int DayPlusOneHours = 0;

		protected int MinutesInHour = 60;
		protected int StartOfficeHours = 8;
		protected int EndOfficeHours = 20;

		protected double XtraLargeFontSize = 18;
		protected double LargeFontSize = 12;
		protected double MediumFontSize = 10;
		protected double SmallFontSize = 9;
		protected double RowHeight = 60;
		protected double PatientDataRailWidth = 240.0;
		protected double HeaderHeight = 40.0;
		protected double AppointmentVerticalIndent = 8;
		protected double ScrollbarWidth = 20;

		protected Pen HeavyBorderPen;
		protected Pen HeaderBorderPen;
		protected Pen HeavyGridPen;
		protected Pen LightGridPen;

		protected Typeface TextTypeFace = new Typeface(new FontFamily("Segoe UI"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);

		protected ScheduleBackgroundBase()
		{
			HeavyBorderPen = TryFindResource("ScheduleBorderPen") as Pen;
			HeaderBorderPen = TryFindResource("ScheduleHeaderBorderPen") as Pen;
			HeavyGridPen = TryFindResource("ScheduleGridHeavyPen") as Pen;
			LightGridPen = TryFindResource("ScheduleGridLightPen") as Pen;
		}


		protected override void OnRender(DrawingContext context)
		{
			if (ActualWidth > 0)
				DrawGrid(context);
		}

		protected abstract void DrawGrid(DrawingContext context);



		internal static double GetOneHourWidth(double width, bool includeTba)
		{
			double result;

			double totalHoursInGrid = (EndHour - StartHour + (includeTba ? 1.0 : 0.0) + DayPlusOneHours);

			result = width / totalHoursInGrid;

			return result;
		}

		protected double ConvertTimeSpanToXPosition(double timelineWidth, TimeSpan timeFromStartOfToday)
		{
			double result = 0;

			double totalMinutesAtStartHour = StartHour * MinutesInHour;

			double totalMintuesInGrid = (EndHour - StartHour) * MinutesInHour;

			double totalMinutesFromStartOfToday = timeFromStartOfToday.TotalMinutes;

			double totalMinutesFromStartHour = totalMinutesFromStartOfToday - totalMinutesAtStartHour;

			double xPosition = timelineWidth * (totalMinutesFromStartHour / totalMintuesInGrid);

			result = xPosition; // Math.Min(Math.Max(xPosition, 0), width);

			return result;
		}

		protected double ConvertToXDeltaToMinutes(double width, double xDelta)
		{
			double totalMintuesInGrid = (EndHour - StartHour) * MinutesInHour;

			double totalDeltaMinutes = totalMintuesInGrid * (xDelta / width);

			return totalDeltaMinutes;
		}

		protected double GetXPositionForDateTime(double timelineWidth, DateTime dateTimeToConvert)
		{
			double result = 0;

			if (timelineWidth > 0)
			{
				TimeSpan timeFromStartOfToday = dateTimeToConvert.Subtract(DateTime.Today);

				return ConvertTimeSpanToXPosition(timelineWidth, timeFromStartOfToday);
			}

			return result;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using WCS.Core;
using WCS.Shared.Controls;

namespace WCS.Shared.Schedule
{
	public class TimelineAvailabilityHours : ScheduleBackgroundBase
	{
		static TimelineAvailabilityHours()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(TimelineAvailabilityHours), new FrameworkPropertyMetadata(typeof(TimelineAvailabilityHours)));
			UnavailableTimesProperty = DependencyProperty.RegisterAttached("UnavailableTimes", typeof(List<TimeDefinition>), typeof(TimelineAvailabilityHours), new PropertyMetadata(new List<TimeDefinition>(), new PropertyChangedCallback(OnUnavailableTimesChanged)));
		}

		public static readonly DependencyProperty HideTBAProperty = DependencyProperty.RegisterAttached("HideTBA", typeof(bool), typeof(TimelineAvailabilityHours), new PropertyMetadata(false));
		public bool HideTBA
		{
			get { return (bool)this.GetValue(HideTBAProperty); }
			set { this.SetValue(HideTBAProperty, value); }
		}

		#region UnavailableTimes dependency property

		public static readonly DependencyProperty UnavailableTimesProperty;
		public static List<TimeDefinition> GetUnavailableTimes(DependencyObject obj)
		{
			return (List<TimeDefinition>)obj.GetValue(UnavailableTimesProperty);
		}
		public static void SetUnavailableTimes(DependencyObject obj, List<TimeDefinition> value)
		{
			obj.SetValue(UnavailableTimesProperty, value);
		}

		private static void OnUnavailableTimesChanged(DependencyObject d, DependencyPropertyChangedEventArgs eventArgs)
		{
			TimelineAvailabilityHours crtl = (TimelineAvailabilityHours)d;
			if (eventArgs.NewValue is List<TimeDefinition>)
			{
				crtl.UnavailableTimes = TimeDefinition.MergeTimes((eventArgs.NewValue as List<TimeDefinition>).Cast<ITimeDefinition>().ToList());
				crtl.InvalidateVisual();
			}
		}

		#endregion

		private List<TimeDefinition> UnavailableTimes { get; set; }

	
		protected override void DrawGrid(DrawingContext context)
		{
			double gridWidth = ActualWidth - (HasInfobar ? PatientDataRailWidth : 0.0);

			double hourWidth = GetOneHourWidth(gridWidth, HasTbaHour);

			double tbaWidth = hourWidth * (HasTbaHour ? 1 : 0);


			double timelineStartX = (HasInfobar ? PatientDataRailWidth : 0.0) + tbaWidth;

			double startX = (HasInfobar ? PatientDataRailWidth : 0.0);

			DrawUnavailableHours(context, timelineStartX, hourWidth);

			if (HasTbaHour && !HideTBA)
				DrawTBA(context, startX, hourWidth);
		}

		private void DrawUnavailableHours(DrawingContext context, double startX, double hourWidth)
		{
			if (UnavailableTimes == null) return;

			UnavailableTimes = TimeDefinition.MergeTimes(UnavailableTimes.Cast<ITimeDefinition>().ToList());
			UnavailableTimes.ForEach(t =>
			{
				var gridStartTime = t.BeginTime - TimeSpan.FromHours(5);
				var startPx = (gridStartTime.Hours * hourWidth) + (gridStartTime.Minutes * hourWidth / 60);
				var endPx = ((gridStartTime + t.Duration).Hours * hourWidth) + ((gridStartTime + t.Duration).Minutes * hourWidth / 60);
				var unavailableHours = new Rect(new Point(startX + startPx, 0), new Point(startX + endPx, ActualHeight));
				context.DrawRectangle(TryFindResource("ScheduleUnavailableOverlayBrush") as Brush, null, unavailableHours);
			});
		}

		private void DrawTBA(DrawingContext context, double startX, double hourWidth)
		{
			var tbaRect = new Rect(new Point(startX, 0), new Point(startX + hourWidth, ActualHeight));

			context.DrawRectangle(TryFindResource("RequiresAttentionBrush") as Brush, null, tbaRect);
		}

	}
}

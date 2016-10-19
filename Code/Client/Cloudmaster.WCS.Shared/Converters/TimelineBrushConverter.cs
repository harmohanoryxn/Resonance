using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using WCS.Shared.Schedule;

namespace WCS.Shared.Converters
{
	public class TimelineBrushConverter : IMultiValueConverter
    {
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (targetType != typeof(Brush))
				throw new InvalidOperationException("Wrong return type");

			if (values.Count() != 2)
				throw new ArgumentException("Wrong Argument amount");
			if (values[0].GetType() != typeof(FlaggedTimelineItemView) || values[1].GetType() != typeof(TimelineItemType))
				return Brushes.Transparent;

			switch ((TimelineItemType)values[1])
			{
				case TimelineItemType.Notification:
					return ((FlaggedTimelineItemView)values[0]).TryFindResource("Apple2Brush") as Brush;
				case TimelineItemType.PatientArrived:
					return ((FlaggedTimelineItemView)values[0]).TryFindResource("Apple5Brush") as Brush;
				case TimelineItemType.OrderCompleted:
					return ((FlaggedTimelineItemView)values[0]).TryFindResource("Apple9Brush") as Brush;
				default:
					return ((FlaggedTimelineItemView)values[0]).TryFindResource("Apple8Brush") as Brush;
			}
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}

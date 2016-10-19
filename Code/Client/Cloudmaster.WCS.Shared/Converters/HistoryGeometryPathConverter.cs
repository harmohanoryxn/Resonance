using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using WCS.Core;
using WCS.Shared.Schedule;

namespace WCS.Shared.Converters
{
	public class HistoryGeometryPathConverter : IMultiValueConverter
    {
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values.Count() != 2)
				throw new ArgumentException("Wrong Argument amount");
			
			if (!(values[0] is FrameworkElement) || values[1].GetType() != typeof(TimelineItemType))
				return new Path();

			var ui = ((FrameworkElement)values[0]);
			switch ((TimelineItemType)values[1])
			{
				case TimelineItemType.NoteIn:
					return WpfExtensions<Path>.Clone(ui.TryFindResource("timelineNoteInPath") as Path);
				case TimelineItemType.NoteOut:
					return WpfExtensions<Path>.Clone(ui.TryFindResource("timelineNoteOutPath") as Path);
				case TimelineItemType.ProcedureTimeUpdated:
					return WpfExtensions<Path>.Clone(ui.TryFindResource("timelineProcedureTimeUpdatedPath") as Path);
				case TimelineItemType.OrderAssigned:
					return WpfExtensions<Path>.Clone(ui.TryFindResource("timelineOrderStartPath") as Path);
				case TimelineItemType.OutstandingNotification:
					return WpfExtensions<Path>.Clone(ui.TryFindResource("timelineOutstandingNotificationPath") as Path);
				case TimelineItemType.OrderCompleted:
					return WpfExtensions<Path>.Clone(ui.TryFindResource("timelineOrderEndPath") as Path);
				case TimelineItemType.NotificationAcknowlegement:
					return WpfExtensions<Path>.Clone(ui.TryFindResource("timelineNotificationAcknowledgedPath") as Path);
				case TimelineItemType.PatientOccupied:
					return WpfExtensions<Path>.Clone(ui.TryFindResource("timelinePatientOccupiedPath") as Path);
					case TimelineItemType.CleaningService:
					return WpfExtensions<Path>.Clone(ui.TryFindResource("timelineCleanBed") as Path);
				case TimelineItemType.BedMarkedAsDirty:
					return WpfExtensions<Path>.Clone(ui.TryFindResource("timelineDirtyBed") as Path);
				case TimelineItemType.FreeRoom:
					return WpfExtensions<Path>.Clone(ui.TryFindResource("timelineFreeRoom") as Path);
				case TimelineItemType.Discharge:
					return WpfExtensions<Path>.Clone(ui.TryFindResource("timelineDischarge") as Path);
				default:
						return WpfExtensions<Path>.Clone(ui.TryFindResource("timelineBlank") as Path);
			}
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}

using System;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Media;
using WCS.Core;
using WCS.Shared.Schedule;

namespace WCS.Shared.Converters
{
	public class SynchronisedBrushConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (targetType != typeof(Brush))
				throw new InvalidOperationException("Wrong return type");

			if (values.Count() != 2)
				throw new ArgumentException("Wrong Argument amount");
			if (!(values[0] is FrameworkElement) || !(values[1] is SynchronousStatus))
				return ((FrameworkElement)values[0]).TryFindResource("LoadingSyncBrush") as Brush;

			var syncedStatus = (SynchronousStatus)values[1];

			switch (syncedStatus)
			{
				case SynchronousStatus.Loading:
					return ((FrameworkElement)values[0]).TryFindResource("LoadingSyncBrush") as Brush;
				case SynchronousStatus.UpToDate:
					return ((FrameworkElement)values[0]).TryFindResource("UpToDateSyncBrush") as Brush;
				case SynchronousStatus.DelayedUpTo1Hour:
					return ((FrameworkElement)values[0]).TryFindResource("UoTo1HourSyncBrush") as Brush;
				case SynchronousStatus.DelayedUpTo1Day:
					return ((FrameworkElement)values[0]).TryFindResource("UoTo1DaySyncBrush") as Brush;
				 default:
					return ((FrameworkElement)values[0]).TryFindResource("MoreThan1DaySyncBrush") as Brush;
			}
	 			
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}

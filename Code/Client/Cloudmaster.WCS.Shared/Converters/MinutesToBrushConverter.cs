using System;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Media;

namespace WCS.Shared.Converters
{
	public class MinutesToBrushConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (targetType != typeof(Brush))
				throw new InvalidOperationException("Wrong return type");

			if (values.Count() != 3)
				throw new ArgumentException("Wrong Argument amount");
			if (!(values[0] is FrameworkElement) || !(values[1] is int) || !(values[2] is string))
				return ((FrameworkElement)values[0]).TryFindResource("stopwatchCountdownDefaultBrush") as Brush;

			var minutes = (int)values[1];
			var type = (string)values[2];
			if (type == "hour")
				return ((FrameworkElement)values[0]).TryFindResource("stopwatchCountdownCriticalBrush") as Brush;
			
			if (minutes < 0)
				return ((FrameworkElement)values[0]).TryFindResource("stopwatchCountdownDefaultBrush") as Brush;
			else if (minutes < 10)
				return ((FrameworkElement)values[0]).TryFindResource("stopwatchCountdownOkBrush") as Brush;
			else
				return ((FrameworkElement)values[0]).TryFindResource("stopwatchCountdownCriticalBrush") as Brush;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class MinutesToBackgroundBrushConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (targetType != typeof(Brush))
				throw new InvalidOperationException("Wrong return type");

			if (values.Count() != 3)
				throw new ArgumentException("Wrong Argument amount");
			if (!(values[0] is FrameworkElement) || !(values[1] is int) || !(values[2] is string))
				return ((FrameworkElement)values[0]).TryFindResource("stopwatchCountdownDefaultBrush") as Brush;

			var type = (string)values[2];
			if (type == "hour")
				return ((FrameworkElement)values[0]).TryFindResource("stopwatchCountdownBackgroundHourBrush") as Brush;

			return ((FrameworkElement)values[0]).TryFindResource("stopwatchCountdownBackgroundMinuteBrush") as Brush;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}

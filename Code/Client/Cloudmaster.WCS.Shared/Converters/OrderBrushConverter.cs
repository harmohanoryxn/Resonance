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
	public class OrderBrushConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (targetType != typeof(Brush))
				throw new InvalidOperationException("Wrong return type");

			if (values.Count() != 3)
				throw new ArgumentException("Wrong Argument amount");
			if (!(values[0] is FrameworkElement) || !(values[1] is TimeSpan) || !(values[2] is bool))
				return ((FrameworkElement)values[0]).TryFindResource("OrderWarningBackGroundBrush") as Brush;

			var orderStartTime = (TimeSpan)values[1];
			var isAcked = (bool)values[2];

			if (orderStartTime < TimeSpan.FromHours(5))
				return ((FrameworkElement)values[0]).TryFindResource("OrderWarningBackGroundBrush") as Brush;

			if (!isAcked && orderStartTime < DateTime.Now.TimeOfDay)
				return ((FrameworkElement)values[0]).TryFindResource("OrderWarningBackGroundBrush") as Brush;
		
			return ((FrameworkElement)values[0]).TryFindResource("OrderBackGroundBrush") as Brush;
			
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}

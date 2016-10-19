using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows;

namespace WCS.Shared.Converters
{
	public class BedHighlightingToBrushConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values.Count() != 2)
				throw new ArgumentException("Wrong Argument amount");
			if (!(values[0] is FrameworkElement) || values[1].GetType() != typeof(bool))
				return Brushes.Transparent;

			var ui = (FrameworkElement)values[0];
			var isHighlighted = (bool)values[1];

			if (isHighlighted)
			{
				return ui.TryFindResource("BlackBrush") as Brush;
			}
			return ui.TryFindResource("BlackBrush") as Brush;

		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}

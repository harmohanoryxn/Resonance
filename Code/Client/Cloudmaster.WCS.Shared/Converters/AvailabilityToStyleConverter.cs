using System;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Media;

namespace WCS.Shared.Converters
{
	public class AvailabilityToStyleConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (targetType != typeof(Style))
				throw new InvalidOperationException("Wrong return type");

			if (values.Count() != 2)
				throw new ArgumentException("Wrong Argument amount");
			if (!(values[0] is FrameworkElement) || !(values[1] is bool))
				return ((FrameworkElement)values[0]).TryFindResource("OrderInformationFontStyle") as Style;


			return ((FrameworkElement)values[0]).TryFindResource((bool)values[1] ? "CleaningEmphasisTextStyle" : "OrderInformationFontStyle") as Style;

					
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}

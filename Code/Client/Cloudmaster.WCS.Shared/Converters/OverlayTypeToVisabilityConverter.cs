using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using WCS.Shared.Department;

namespace WCS.Shared.Converters
{
	public class OverlayTypeToVisabilityConverter : IValueConverter
	{ 
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (targetType != typeof(Visibility))
				throw new InvalidOperationException("Wrong return type");
			if (!(value is OverlayType) || !(parameter is OverlayType))
				return Visibility.Collapsed;

			//return (OverlayType)value == (OverlayType)parameter  ? Visibility.Visible : Visibility.Collapsed;
			return (((OverlayType)value | (OverlayType)parameter ) == (OverlayType)value) ? Visibility.Visible : Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}

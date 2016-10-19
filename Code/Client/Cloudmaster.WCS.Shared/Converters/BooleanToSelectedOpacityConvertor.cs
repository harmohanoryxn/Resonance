using System;
using System.Windows.Data;
using System.Globalization;
using WCS.Shared.Schedule;

namespace WCS.Shared.Converters
{
	[ValueConversion(typeof(double), typeof(ScreenSelectionType))]
	public class BooleanToSelectedOpacityConvertor : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value==null) return 0.95;

			switch ((ScreenSelectionType)value)
			{
				case ScreenSelectionType.Selected:
					return 1.0;
				case ScreenSelectionType.DeSelected:
					return 0.1;
				default:
					return 1.0;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}

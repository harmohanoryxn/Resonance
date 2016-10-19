using System;
using System.Windows.Data;
using System.Globalization;
using WCS.Shared.Schedule;
using System.Windows.Media;

namespace WCS.Shared.Converters
{
	[ValueConversion(typeof(double), typeof(ScreenSelectionType))]
    public class BooleanToSelectedBackgroundConvertor : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
            if (value == null) return Brushes.White;

			switch ((ScreenSelectionType)value)
			{
				case ScreenSelectionType.Selected:
					return Brushes.White;
                case ScreenSelectionType.DeSelected:
                    return Brushes.Transparent;
				default:
                    return Brushes.White;
            }
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}

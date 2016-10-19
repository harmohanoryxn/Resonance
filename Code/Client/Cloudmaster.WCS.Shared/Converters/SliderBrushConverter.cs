using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using WCS.Shared.Schedule;

namespace WCS.Shared.Converters
{
	/// <summary>
	/// Used to create a binding for a margin (defined by pixels) rendered by the slider in the discharge screen
	/// </summary>
	class SliderBrushConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (targetType != typeof(Brush))
				throw new InvalidOperationException("Wrong return type");

			if (values.Count() != 3)
				throw new ArgumentException("Wrong Argument amount");
			if (!(values[0] is FrameworkElement) || !(values[1] is double) || !(values[2] is DateTime))
				return ((FrameworkElement)values[0]).TryFindResource("OrderWarningBackGroundBrush") as Brush;

			var maxWidth = System.Convert.ToDouble(values[1]);
			var dischargeTime = (DateTime)values[2];

			var hours = (dischargeTime - DateTime.Now).Hours;
			return hours < 0 ? ((FrameworkElement)values[0]).TryFindResource("OrderWarningBackGroundBrush") as Brush : ((FrameworkElement)values[0]).TryFindResource("Notification1BackGroundBrush") as Brush;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}

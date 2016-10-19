using System;
using System.Linq;
using System.Windows.Data;
using WCS.Shared.Schedule;

namespace WCS.Shared.Converters
{
	/// <summary>
	/// Used to create a binding for a width (defined by time) rendered by the slider in the discharge screen
	/// </summary>
	class SliderWidthConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (targetType != typeof(double))
				throw new InvalidOperationException("Wrong return type");

			if (values.Count() != 2)
				throw new ArgumentException("Wrong Argument amount");
			if (values[0] == null)
				return 0.0;
			if (values[0].GetType() != typeof(double))
				return 0.0;
		
			var maxWidth = System.Convert.ToDouble(values[0]);
	 
			if (maxWidth == 0.0)
				return 0.0;

			var oneHourWidth = maxWidth / 16;
			return oneHourWidth; 
		}

		private static double ConvertTimeToWidth(TimeSpan realStartTime, double oneHourWidth)
		{
			return Math.Floor(realStartTime.Hours * oneHourWidth) + ((realStartTime.Minutes * oneHourWidth) / 60.0);
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}

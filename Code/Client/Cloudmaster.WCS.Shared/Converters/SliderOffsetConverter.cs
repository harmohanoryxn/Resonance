using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using WCS.Core;
using WCS.Shared.Schedule;

namespace WCS.Shared.Converters
{
	/// <summary>
	/// Used to create a binding for a margin (defined by pixels) rendered by the slider in the discharge screen
	/// </summary>
	class SliderOffsetConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (targetType != typeof(Thickness))
				throw new InvalidOperationException("Wrong return type");

			if (values.Count() != 2)
				throw new ArgumentException("Wrong Argument amount");
			if (values[0] == null || values[1] == null)
				return new Thickness(0, 0, 0, 0);
			if (values[0].GetType() != typeof(double) || values[1].GetType() != typeof(DateTime))
				return new Thickness(0, 0, 0, 0);

			var maxWidth = System.Convert.ToDouble(values[0]);
			var dischargeTime = (DateTime)values[1];
			if (maxWidth == 0.0)
				return new Thickness(0, 0, 0, 0);

			var oneHourWidth = maxWidth / 16;

			double left = 0.0;
			var hours = (dischargeTime - DateTime.Now.RoundDownToNearestHour()).TotalHours;
		//	Debug.WriteLine(hours);
			if (hours >= 72)
				left = 15 * oneHourWidth;
			else if (hours >= 48)
				left = 14 * oneHourWidth;
			else if (hours >=24)
				left = 13 * oneHourWidth;
			else
				left = hours < 0 ? 0.0 : (hours + 1) * oneHourWidth;

			return new Thickness(left, 0, 0, 0);
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}

using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using WCS.Shared.Schedule;

namespace WCS.Shared.Converters
{
	public class RightArrowMarginConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (targetType != typeof(Thickness))
				throw new InvalidOperationException("Wrong return type");

			if (values.Count() != 4)
				throw new ArgumentException("Wrong Argument amount");
			if (values[0] == null || values[1] == null || values[2] == null || values[3] == null)
				return new Thickness(0, 0, 0, 0);
			if (values[0].GetType() != typeof(double) || values[1].GetType() != typeof(double) || values[2].GetType() != typeof(double) || values[3].GetType() != typeof(double))
				return new Thickness(0, 0, 0, 0);

			var maxWidth = System.Convert.ToDouble(values[0]);
			var maxHeidth = System.Convert.ToDouble(values[1]);
			var offset = System.Convert.ToDouble(values[2]);
			var additionalOffset = System.Convert.ToDouble(values[3]);

			if (maxWidth == 0.0 || maxHeidth == 0.0)
				return new Thickness(0, 0, 0, 0);

			return new Thickness(0, additionalOffset + offset, (maxWidth - 1) * -1, 0);
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}

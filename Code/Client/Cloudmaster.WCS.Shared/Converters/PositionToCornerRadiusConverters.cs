using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace WCS.Shared.Converters
{
	public class IsOnlyPositionToCornerRadiusConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values.Count() != 2)
				throw new ArgumentException("Wrong Argument amount");
			if (!(values[0] is int) || !(values[1] is int))
				return new CornerRadius(0, 0, 0, 0);

			var count = (int)values[0];
			var index = (int)values[1];

			return count == 1;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
	public class IsTopPositionToCornerRadiusConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values.Count() != 2)
				throw new ArgumentException("Wrong Argument amount");
			if (!(values[0] is int) || !(values[1] is int))
				return new CornerRadius(0, 0, 0, 0);

			var count = (int)values[0];
			var index = (int)values[1];

			return index == 0;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
	public class IsBottomPositionToCornerRadiusConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values.Count() != 2)
				throw new ArgumentException("Wrong Argument amount");
			if (!(values[0] is int) || !(values[1] is int))
				return new CornerRadius(0, 0, 0, 0);

			var count = (int)values[0];
			var index = (int)values[1];

			return index == count-1;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}

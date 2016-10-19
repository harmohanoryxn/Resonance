using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Cloudmaster.WCS.Controls.Converters;

namespace WCS.Shared.Converters
{
	[ValueConversion(typeof(Boolean), typeof(Visibility))]
	public class ArgBooleanToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			ConverterHelper.ValidateArguments(value, false, typeof(Boolean), targetType, typeof(Visibility), parameter, typeof(String));

			Visibility result;
			string parameterText = (string)parameter;

			bool isValueTrue = (bool)value;

			switch (parameterText)
			{
				case "VisibleIfTrue":
					result = isValueTrue ? Visibility.Visible : Visibility.Hidden;
					break;

				case "HiddenIfTrue":
					result = isValueTrue ? Visibility.Hidden : Visibility.Visible;
					break;

				case "CollapsedIfTrue":
					result = isValueTrue ? Visibility.Collapsed : Visibility.Visible;
					break;

				case "CollapsedIfFalse":
					result = isValueTrue ? Visibility.Visible : Visibility.Collapsed;
					break;

				default:
					throw new InvalidOperationException(String.Format("Invalid parameter value: {0}.", parameterText));
			}

			return result;
		}

		public object ConvertBack(object value, Type targetType, object parameter,			CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}



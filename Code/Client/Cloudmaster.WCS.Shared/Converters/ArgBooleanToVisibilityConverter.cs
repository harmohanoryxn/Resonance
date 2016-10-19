using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Cloudmaster.WCS.Controls.Converters;

namespace WCS.Shared.Converters
{
	public class ArgNullableToBooleanConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			string parameterText = (string)parameter;

			switch (parameterText)
			{
				case "trueIfNull":
					return (value == null);

				case "falseIfNull":
					return (value != null);

				default:
					throw new InvalidOperationException(String.Format("Invalid parameter value: {0}.", parameterText));
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter,
			CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}



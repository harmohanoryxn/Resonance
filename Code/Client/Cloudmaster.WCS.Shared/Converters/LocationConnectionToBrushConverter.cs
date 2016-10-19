using System;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Media;
using WCS.Core;

namespace WCS.Shared.Converters
{
	public class LocationConnectionToBrushConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (targetType != typeof(Brush))
				throw new InvalidOperationException("Wrong return type");

			if (values.Count() != 5)
				throw new ArgumentException("Wrong Argument amount");
			if (!(values[0] is FrameworkElement) || (values[1] != null && !(values[1] is DateTime)) || !(values[2] is int) || !(values[3] is int))
				return Brushes.LightGray;

			if (values[1] != null)
			{
				TimeSpan ts = DateTime.Now - (DateTime)values[1];

				if (ts.TotalMinutes > 2)
					return ((FrameworkElement)values[0]).TryFindResource("CritialWardBrush") as Brush;
			}

			int shortTerm = (int)values[2];
			int longTerm = (int)values[3];

			if (longTerm > 0)
				return ((FrameworkElement)values[0]).TryFindResource("AlertSeriousBrush") as Brush;
			else if (shortTerm > 0)
				return ((FrameworkElement)values[0]).TryFindResource("WarningWardBrush") as Brush;


			if (values[1] != null)
			{
				return ((FrameworkElement) values[0]).TryFindResource("OkBrush") as Brush;
			}
			else
			{
				return Brushes.LightGray;
			}
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}

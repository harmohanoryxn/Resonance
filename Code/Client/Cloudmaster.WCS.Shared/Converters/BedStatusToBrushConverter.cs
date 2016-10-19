using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using System.Windows;
using WCS.Shared.Schedule;

namespace WCS.Shared.Converters
{
	public class BedStatusToBrushConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values.Count() != 2)
				throw new ArgumentException("Wrong Argument amount");
			if (!(values[0] is FrameworkElement) || values[1].GetType() != typeof(BedStatus))
				return Brushes.White;

			var ui = (FrameworkElement)values[0];
			var status = (BedStatus)values[1];
 
			switch (status)
			{
				case BedStatus.Clean:
					return ui.TryFindResource("BedCleanStatusBrush") as Brush;
				case BedStatus.Dirty:
					return ui.TryFindResource("BedDirtyStatusBrush") as Brush;
				case BedStatus.RequiresDeepClean:
					return ui.TryFindResource("BedDeepCleanRequiredBrush") as Brush;
		
				default:
					{
						string parameterText = (string)parameter;
						throw new InvalidOperationException(String.Format("Invalid parameter value: {0}.", parameterText));
					}
			}
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}

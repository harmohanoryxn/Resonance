using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using WCS.Core;
using WCS.Shared.Controls;
using WCS.Shared.Schedule;

namespace WCS.Shared.Converters
{
	public class BedMapStatusToGeometryPathConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values.Count() != 2)
				throw new ArgumentException("Wrong Argument amount");

			if (!(values[0] is FrameworkElement) || values[1].GetType() != typeof(Cloudmaster.WCS.DataServicesInvoker.DataServices.BedStatus))
				return new Path();

			var ui = ((FrameworkElement)values[0]);


			var border = new Border();


			switch ((Cloudmaster.WCS.DataServicesInvoker.DataServices.BedStatus)values[1])
			{
				case Cloudmaster.WCS.DataServicesInvoker.DataServices.BedStatus.RequiresDeepClean:
					{
						border.Background = ui.TryFindResource("alternativeExclaimBrush") as Brush;
						break;
					}
				case Cloudmaster.WCS.DataServicesInvoker.DataServices.BedStatus.Dirty:
					border.Background = ui.TryFindResource("exclaimBrush") as Brush;
					break;
			}
			return border;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}

using System;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Media;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using WCS.Core;

namespace WCS.Shared.Converters
{
	public class ConfigurationToThumbnailBrushConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (targetType != typeof(Brush))
				throw new InvalidOperationException("Wrong return type");

			if (values.Count() != 2)
				throw new ArgumentException("Wrong Argument amount");
			if (!(values[0] is FrameworkElement) || (values[1] == null))
				return Brushes.Transparent;

			switch ((DeviceConfigurationType)values[1])
			{
			    case DeviceConfigurationType.Department:
			            return ((FrameworkElement)values[0]).TryFindResource("departmentThumbnailBrush") as Brush;
			    case DeviceConfigurationType.Cleaning:
			    case DeviceConfigurationType.Discharge:
			    case DeviceConfigurationType.Admissions:
			            return ((FrameworkElement)values[0]).TryFindResource("cleaningThumbnailBrush") as Brush;
			    case DeviceConfigurationType.Ward:
			            return ((FrameworkElement)values[0]).TryFindResource("wardThumbnailBrush") as Brush;
				case DeviceConfigurationType.Physio:
			            return ((FrameworkElement)values[0]).TryFindResource("physioThumbnailBrush") as Brush;
			    default:
			    return Brushes.Transparent;
			}
			 
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}

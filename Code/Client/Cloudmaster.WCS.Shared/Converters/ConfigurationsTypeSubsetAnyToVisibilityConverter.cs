using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using WCS.Shared.Schedule;

namespace WCS.Shared.Converters
{
	public class ConfigurationsTypeSubsetAnyToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
				return Visibility.Collapsed;
			if (!(value is ListCollectionView))
				return Visibility.Collapsed;
		
			var configInstnaces = ((ListCollectionView)value).OfType<DeviceConfigurationInstance>();

			if (parameter is DeviceConfigurationType)
			{
				return configInstnaces.Any(configInstnace => configInstnace.Type == (DeviceConfigurationType)parameter) ? Visibility.Visible : Visibility.Collapsed;
			}
			return Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}



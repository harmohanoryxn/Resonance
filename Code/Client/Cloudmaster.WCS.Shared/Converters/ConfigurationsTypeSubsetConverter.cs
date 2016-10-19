using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using WCS.Shared.Schedule;

namespace WCS.Shared.Converters
{
	public class ConfigurationsTypeSubsetConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			//	if (!(value is IList))
			if (!(targetType.Name == "IEnumerable"))
				throw new InvalidOperationException("Wrong return type");

			if (value == null)
				return Enumerable.Empty<DeviceConfigurationInstance>();
			if (!(value is ListCollectionView))
				return Enumerable.Empty<DeviceConfigurationInstance>();

			var configInstnaces = ((ListCollectionView)value).OfType<DeviceConfigurationInstance>();

			if (parameter is DeviceConfigurationType)
			{
				return configInstnaces.Where(configInstnace => configInstnace.Type == (DeviceConfigurationType)parameter);
			}
			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}



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
	public class ConfigurationsVisibleLocationsDefaultLocationNameConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{

            if (value == null)
                return String.Empty;
            if (!(value is IList))
                return String.Empty;

			var locations = (IEnumerable<LocationSummary>)value;

            var firstLocation = locations.FirstOrDefault();

            return firstLocation != null ? firstLocation.Name : String.Empty;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}



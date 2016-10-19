using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using WCS.Shared.Orders;
using WCS.Shared.Schedule;

namespace WCS.Shared.Converters
{
	class PatientGroupingConverter : IValueConverter
	{
		#region IValueConverter Members

		public object NullValue { get; set; }

		public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return (value as OrderViewModel).OrderCoordinator.Order.Patient.SearchString;
		}

		public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new System.Exception("The method or operation is not implemented.");
		}

		#endregion
	}
}
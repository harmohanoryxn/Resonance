using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using WCS.Shared.Orders;
using WCS.Shared.Schedule;

namespace WCS.Shared.Converters
{
	public class DoesPatientHaveMultipleOrdersToVisabilityConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (targetType != typeof(Visibility))
				throw new InvalidOperationException("Wrong return type");

			if (values.Count() != 2)
				throw new ArgumentException("Wrong Argument amount");
			if (values[0] == null || values[1] == null)
				return Visibility.Collapsed;
			if (!(values[0] is IList) || !(values[1] is string))
				return Visibility.Collapsed;

			var orders = (IList)values[0];
			var patientId = (string)values[1];

			try
			{
				var depts = new List<string>();
				foreach (OrderViewModel order in orders)
				{
					if (order.OrderCoordinator.Order.Patient.IPeopleNumber == patientId)
					{
						if (!depts.Contains(order.OrderDepartmentCode)) depts.Add(order.OrderDepartmentCode);
						if (depts.Count > 1) return Visibility.Visible;
					}
				}
				return Visibility.Collapsed;
			}
			catch
			{
				return Visibility.Collapsed;
			}
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}



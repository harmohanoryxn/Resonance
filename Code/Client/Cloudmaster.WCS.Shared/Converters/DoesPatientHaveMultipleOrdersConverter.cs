using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using WCS.Shared.Orders;
using WCS.Shared.Schedule;

namespace WCS.Shared.Converters
{
	public class DoesPatientHaveMultipleOrdersConverter : IMultiValueConverter
	{

		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (targetType != typeof(bool))
				throw new InvalidOperationException("Wrong return type");

			if (values.Count() != 2)
				throw new ArgumentException("Wrong Argument amount");
			if (values[0] == null || values[1] == null)
				return false;
			if (!(values[0] is IList) || !(values[1] is string))
				return false;

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
						if (depts.Count > 1) return true;
					}
				}
				return false;
			}
			catch
			{
				return false;
			}
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}



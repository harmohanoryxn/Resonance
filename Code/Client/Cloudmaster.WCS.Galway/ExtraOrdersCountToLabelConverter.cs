using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using WCS.Shared.Orders;
using WCS.Shared.Schedule;

namespace Cloudmaster.WCS.Galway
{
	class ExtraOrdersCountToLabelConverter : IMultiValueConverter
	{

		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values.Count() != 3)
				throw new ArgumentException("Wrong Argument amount");
			if (values[0] == null || values[1] == null)
				return "";
			if (!(values[0] is IList) || !(values[1] is string) || !(values[2] is bool))
				return "";

			var orders = (IList)values[0];
			var dept = (string)values[1];
			var show = (bool)values[2];

			try
			{
				var otherDeptCount = 0;
				foreach (OrderViewModel order in orders)
				{
					if (order.OrderDepartmentCode != dept) otherDeptCount++;
				}
				return string.Format("{0} {1} Related Order{2}", show ? "Hide" : "Show", otherDeptCount, otherDeptCount>1?"s":"");
			}
			catch
			{
				return "";
			}

			return "";
		 
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}



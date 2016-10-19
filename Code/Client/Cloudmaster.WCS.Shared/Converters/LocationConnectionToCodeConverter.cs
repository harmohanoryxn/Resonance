using System;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Media;
using WCS.Core;
using WCS.Shared.Schedule;

namespace WCS.Shared.Converters
{
	public class LocationConnectionToCodeConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (targetType != typeof(string))
				throw new InvalidOperationException("Wrong return type");

			if (values.Count() != 5)
				throw new ArgumentException("Wrong Argument amount");
			if (!(values[0] is DateTime) || !(values[1] is int) || !(values[2] is int) || !(values[3] is DeviceInfoViewModel.Mode))
				return "Disconnected";

			var ts = DateTime.Now - (DateTime)values[0];
			var shortTerm = (int)values[1];
			var longTerm = (int)values[2];
			var type = (DeviceInfoViewModel.Mode)values[3];

			if (ts.TotalMinutes > 2)
				return "Disconnected";

			switch (type)
			{
				case (DeviceInfoViewModel.Mode.Department):
					if (longTerm > 0)
						return string.Format("{0} Unscheduled", longTerm);
					break;
				case (DeviceInfoViewModel.Mode.Ward):
					if (longTerm > 0)
						return string.Format("{0} Overdue", longTerm);
					else if (shortTerm > 0)
						return string.Format("{0} Requires Attention", longTerm);
					break;
			}

			return "Connected";
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}

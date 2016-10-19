using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using WCS.Shared.Schedule;

namespace WCS.Shared.Converters
{
	class BedStatusToNotBoolConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var status = (BedStatus)value;
			
			string parameterText = (string)parameter;

			switch (parameterText)
			{
				case "Clean":
					return (status != BedStatus.Clean);
				case "Dirty":
					return (status != BedStatus.Dirty);
				case "RequiresDeepClean":
					return (status != BedStatus.RequiresDeepClean);
				//case "NotReady":
				//    return (status != BedStatus.NR);
				//case "OutOfOrder":
				//    return (status != BedStatus.OO);
				//case "Reserved":
				//    return (status != BedStatus.RB);
				//case "Ready":
				//    return (status != BedStatus.RE);

				default:
					throw new InvalidOperationException(String.Format("Invalid parameter value: {0}.", parameterText));
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter,CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
 
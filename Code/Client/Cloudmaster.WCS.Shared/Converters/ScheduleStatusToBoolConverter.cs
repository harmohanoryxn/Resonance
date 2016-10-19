using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using Cloudmaster.WCS.DataServicesInvoker.Types;
using WCS.Shared.Schedule;

namespace WCS.Shared.Converters
{
	class ScheduleStatusToBoolConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var status = (MultiSelectAdmissionStatusFlag)value;
			
			string parameterText = (string)parameter;

            switch (parameterText)
			{
				case "Admitted":
					return status.IsSet(MultiSelectAdmissionStatusFlag.Admitted);
				case "Registered":
					return (status == MultiSelectAdmissionStatusFlag.Registered);
				case "Discharged":
					return (status == MultiSelectAdmissionStatusFlag.Discharged);
				default:
					return (status == MultiSelectAdmissionStatusFlag.Unknown);
				//case "Admitted":
				//    return (status == MultiSelectAdmissionStatusFlag.Admitted);
				//case "Registered":
				//    return (status == MultiSelectAdmissionStatusFlag.Registered);
				//case "Discharged":
				//    return (status == MultiSelectAdmissionStatusFlag.Discharged);
				//default:
				//    return (status == MultiSelectAdmissionStatusFlag.Unknown);
            }
		}

		public object ConvertBack(object value, Type targetType, object parameter,CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}

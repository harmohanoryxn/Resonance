using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using WCS.Shared.Location;

namespace WCS.Shared.Converters
{
	public class DetectionToFloorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null || value.GetType() != typeof(DetectionViewModel))
				return "";

			switch ((value as DetectionViewModel).Location)
			{
				case "O Malley":
					return "Basement";
				case "John Paul II":
				case "MRI":
				case "CT Scan":
				case "Ultrasound":
				case "X-Ray":
				case "Nuclear Medicine":
				case "Fluoroscopy":
				case "Cardiology":
				case "NEURO MOD":
					return "Ground Floor";
				case "Freyer":
					return "1st Floor";
				case "Mother Teresa":
					return "2nd Floor";
				case "Our Lady Of Knock":
					return "3rd Floor";
				case "Florence Nightingale":
					return "4th Floor";
				default:
					return "";
			}

		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}

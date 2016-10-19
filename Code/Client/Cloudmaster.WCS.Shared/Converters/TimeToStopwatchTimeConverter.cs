using System;
using System.Windows.Data;
using GalaSoft.MvvmLight;
using WCS.Core;

namespace WCS.Shared.Converters
{
	/// <summary>
	/// 
	/// </summary>
	public class TimeToStopwatchTimeConverter : IValueConverter
	{

		public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (targetType != typeof(string) && targetType != typeof(object))
				throw new InvalidOperationException("Wrong return type");
            if (value.GetType() != typeof(TimeSpan))
				return "";

            return ((TimeSpan)value).ToStopwatchTimeFormat();
		}

		public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new System.NotImplementedException();
		}
	}
}
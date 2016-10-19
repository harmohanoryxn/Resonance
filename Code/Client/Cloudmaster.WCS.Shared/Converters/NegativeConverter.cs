using System.Windows.Data;

namespace WCS.Shared.Converters
{
	public class NegativeConverter : IValueConverter
	{
		#region IValueConverter Members

		public object NullValue { get; set; }

		public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value == null)
				return NullValue;
			if (value is double)
				return -1.0 * (double)value;
			else if (value is short)
				return -1.0 * (short)value;
			else if (value is long)
				return -1.0 * (long)value;
			else if (value is bool)
				return !(bool)value;

			else
				throw new System.ArgumentException();
		}

		public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new System.Exception("The method or operation is not implemented.");
		}

		#endregion
	} 
}
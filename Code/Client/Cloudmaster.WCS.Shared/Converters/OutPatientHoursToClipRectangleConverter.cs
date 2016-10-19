using System;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using WCS.Shared.Orders;
using WCS.Shared.Schedule;

namespace WCS.Shared.Converters
{
	/// <summary>
	/// Used to create a binding that returns a rectangle that clips an overlay into the shap of an Order
	/// </summary>
	/// <remarks>
	/// Although the StartTime is an input parameter to this convert, it is never actually used. The reason it is an input is that
	/// it was needed because the converter needs to be recalled when ever the start time is changed
	/// </remarks>
	class OutPatientHoursToClipRectangleConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (targetType != typeof(Rect))
				throw new InvalidOperationException("Wrong return type");

			if (values.Count() != 3)
				throw new ArgumentException("Wrong Argument amount");
			if (values[0] == null || values[1] == null)
				return new Rect(0, 0, 0, 0);
			if (values[0].GetType() != typeof(double) || !(values[1] is OrderItemViewModel))
				return new Rect(0, 0, 0, 0);

			var start = 0d;
			var width = 0d;

			var maxWidth = System.Convert.ToDouble(values[0])-240;
			var order = values[1] as OrderItemViewModel;
			var oneHourWidth = ScheduleBackgroundBase.GetOneHourWidth(maxWidth, true);
			var starttime = order.StartTime.HasValue?order.StartTime.Value:TimeSpan.FromHours(4);
			var startts = starttime.Subtract(new TimeSpan(4, 0, 0));

			var leftWidth = Math.Floor(startts.Hours * oneHourWidth) + ((startts.Minutes * oneHourWidth) / 60.0);
			var durationts = order.Duration;
			var orderWidth = Math.Floor(durationts.Hours * oneHourWidth) + ((durationts.Minutes * oneHourWidth) / 60.0);
			var maxAvailableWidth = maxWidth - orderWidth;

			//var oneHourWidth = TimelineCurrentTimeTracer.GetOneHourWidth(System.Convert.ToDouble(values[1]));

			width = Math.Floor(durationts.Hours * oneHourWidth) + ((durationts.Minutes * oneHourWidth) / 60.0);
			width = (width < oneHourWidth) ? oneHourWidth : width;

			start = Math.Floor(Math.Max(Math.Min(maxAvailableWidth, leftWidth), 0.0))+240;

			return new Rect(start, 2d, width, 42d);
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}

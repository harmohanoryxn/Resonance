using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Converters;
using System.Windows.Shapes;
using Cloudmaster.WCS.Galway.Core;
using WCS.Shared.Beds;
using WCS.Shared.Cleaning.Schedule;
using WCS.Shared.Controls;
using WCS.Shared.Discharge.Schedule;

namespace WCS.Shared.Converters
{
	public class RfidBackgroundBrushConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value.GetType() != typeof(string))
				return Brushes.Transparent;

			var location = (string)value;

			var brush = RfidMapResource.GetLocationBrush(location);
			if (brush == null)
				throw new ArgumentException(string.Concat(new[] { "Cannot find geometry for '", "", "'" }));

			return brush;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class RfidLocationToWidthConverter : IValueConverter
	{

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var canvasName = "";
			if (parameter.ToString() == "total")
				canvasName = "Document";
			if (parameter.ToString() == "floorplan")
				canvasName = (string)value;
			else if (parameter.ToString() == "annotation")
				return RfidMapResource.GetLabelsWidth();

			return RfidMapResource.GetCanvasWidth(canvasName);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class RfidLocationToHeightConverter : IValueConverter
	{

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var canvasName = "";
			if (parameter.ToString() == "total")
				canvasName = "Document";
			if (parameter.ToString() == "floorplan")
				canvasName = (string)value;
			else if (parameter.ToString() == "annotation")
				return RfidMapResource.GetLabelsHeight();

			return RfidMapResource.GetCanvasHeight(canvasName);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class RfidLocationToOffsetConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var canvasName = "";
			if (parameter.ToString() == "total")
				canvasName = "Document";
			if (parameter.ToString() == "floorplan")
				canvasName = (string)value;
			else if (parameter.ToString() == "annotation")
				return RfidMapResource.GetLabelsOffet();

			return RfidMapResource.GetCanvasOffset(canvasName);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class RfidLocationGeometry : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null) return new Thickness();

			var loc = (string)value;
			return RfidMapResource.GetLocationGeometry(loc);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class RfidCurrentLocationToOffsetConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null) return new Thickness();

			var loc = (string)value;
			var width = RfidMapResource.GetCanvasWidth(loc);
			var height = RfidMapResource.GetCanvasHeight(loc);
			var offset = RfidMapResource.GetCanvasOffset(loc);
			return new Thickness((offset.Left + (width / 2) - 200), (offset.Top + (height / 2) - 200), 0, 0);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

}

using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using Cloudmaster.WCS.Galway.Core;
using WCS.Shared.Controls;

namespace WCS.Shared.Converters
{
	/// <summary>
	/// Converts a ward name to a brush to paint the background floorplan
	/// </summary>
	public class LocationToFloorplanBrushConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values.Count() != 2)
				throw new ArgumentException("Wrong Argument amount");
			if (!(values[0] is FrameworkElement) || values[1].GetType() != typeof(string))
				return Brushes.Transparent;

			var ui = (FrameworkElement)values[0];
			var wardCode = (string)values[1];

			var brush = LocationToMapResource.GetLocationBrush(wardCode);
			if (brush == null)
				throw new ArgumentException(string.Concat(new[] { "Cannot find geometry for '", wardCode, "'" }));

			return brush;// WpfExtensions<Geometry>.Clone(geometry);
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class LocationToWidthConverter : IValueConverter
	{

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var canvasName = (string)value;
			if (parameter.ToString() == "total")
				canvasName = "Document";
			if (parameter.ToString() == "floorplan")
				canvasName = (string)value;
			else if (parameter.ToString() == "annotation")
				canvasName = string.Format("{0}_Labels", canvasName);

			return LocationToMapResource.GetCanvasWidth(canvasName);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class LocationToHeightConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var canvasName = (string)value;
			if (parameter.ToString() == "total")
				canvasName = "Document";
			if (parameter.ToString() == "floorplan")
				canvasName = (string)value;
			else if (parameter.ToString() == "annotation")
				canvasName = string.Format("{0}_Labels", canvasName);

			return LocationToMapResource.GetCanvasHeight(canvasName);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class LocationToOffsetConverter : IValueConverter
	{

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var canvasName = (string) value;
			if (parameter.ToString() == "total")
				canvasName = "Document";
			if (parameter.ToString() == "floorplan")
				canvasName = (string) value;
			else if (parameter.ToString() == "annotation")
				canvasName = string.Format("{0}_Labels", canvasName);

			return LocationToMapResource.GetCanvasOffset(canvasName);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
	 
	/// <summary>
	/// Converts a ward name to a brush to paint the background floorplan's annotations
	/// </summary>
	public class LocationToFloorplanAnnotationBrushConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values.Count() != 2)
				throw new ArgumentException("Wrong Argument amount");
			if (!(values[0] is FrameworkElement) || values[1].GetType() != typeof(string))
				return Brushes.Transparent;

			var ui = (FrameworkElement)values[0];
			var wardCode = (string)values[1];

			var brush = LocationToMapResource.GetLocationAnnotationBrush(wardCode);
			if (brush == null)
				throw new ArgumentException(string.Concat(new[] { "Cannot find geometry for '", wardCode, "'" }));

			return brush;// WpfExtensions<Geometry>.Clone(geometry);
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	/// <summary>
	/// Converts a bed name to a geometry
	/// </summary>
	public class BedMapToGeometryConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values.Count() != 2)
				throw new ArgumentException("Wrong Argument amount");
			if (!(values[0] is FrameworkElement))
				return Brushes.Transparent;

			var ui = (FrameworkElement)values[0];
			dynamic bed = values[1];

			Geometry geometry = LocationToMapResource.GetBedGeometry(bed.LocationCode, bed.Room, bed.Bed);
			if (geometry == null)
			{
				var message = string.Concat(new[] { "Cannot find geometry for 'geo", bed.LocationCode, bed.Room, bed.Bed, "'" });
				//throw new ArgumentException(message);
			}

			return geometry;// WpfExtensions<Geometry>.Clone(geometry);
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	/// <summary>
	/// Checks if a geometry exists and then returns an appropriate visibility for it
	/// </summary>
	public class BedHasGeometryToVisibilityConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values.Count() != 2)
				throw new ArgumentException("Wrong Argument amount");
			if (!(values[0] is FrameworkElement))
				return Visibility.Collapsed;

			var ui = (FrameworkElement)values[0];
			dynamic bed = values[1];

			Geometry geometry = LocationToMapResource.GetBedGeometry(bed.LocationCode, bed.Room, bed.Bed);
			return (geometry == null) ? Visibility.Collapsed : Visibility.Visible;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
	 

	/// <summary>
	/// Converts a bed name to a path
	/// </summary>
	public class BedMapToPathConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values.Count() != 2)
				throw new ArgumentException("Wrong Argument amount");
			if (!(values[0] is FrameworkElement))
				return WpfExtensions<Path>.Clone(((FrameworkElement)values[0]).TryFindResource("timelineUnknownPath") as Path);

			var ui = (FrameworkElement)values[0];
			dynamic bed = values[1];

			var path = LocationToMapResource.GetBedPath(bed.LocationCode, bed.Room, bed.Bed);
			if (path == null)
			{
				var message = string.Concat(new[] { "Cannot find path for 'path", bed.LocationCode, bed.Room, bed.Bed, "'" });
				return null;
				//throw new ArgumentException(message);
			}

			var colourHighlightedBinding = new MultiBinding();
			colourHighlightedBinding.Bindings.Add(new Binding() { Source = ui });
			colourHighlightedBinding.Bindings.Add(new Binding("IsHighlighted") { Source = bed });
			colourHighlightedBinding.Converter = new BedHighlightingToBrushConverter();

			var availableBinding = new Binding("IsAvailableNowForCleaning");
			availableBinding.Converter = new BooleanToVisibilityConverter();

			var iconBinding = new MultiBinding();
			iconBinding.Bindings.Add(new Binding() { Source = ui });
			iconBinding.Bindings.Add(new Binding("Status") { Source = bed });
			iconBinding.Converter = new BedMapStatusToGeometryPathConverter();

			path.SetBinding(Shape.StrokeProperty, colourHighlightedBinding);
			path.StrokeThickness = 2.5;
			path.Margin = new Thickness(path.Data.Bounds.Left - 0.5, path.Data.Bounds.Top - 0.5, 0, 0);

			//var brush = ((FrameworkElement)values[0]).TryFindResource("Notification1BackGroundBrush") as Brush;
			var isAvailableNow = new FlashingControl();
			isAvailableNow.SetBinding(UIElement.VisibilityProperty, availableBinding);

			var icon = new ContentControl();
			icon.SetBinding(ContentControl.ContentProperty, iconBinding);
			icon.Width = 15;
			icon.Height = 15;
			//icon.Margin = new Thickness(path.Data.Bounds.Left + (path.Data.Bounds.Width/2)-5,path.Data.Bounds.Top + (path.Data.Bounds.Height/2)-5,0,0);
			//icon.Margin = new Thickness(path.Data.Bounds.X + 5, path.Data.Bounds.Y + 5, 0, 0);
			icon.Margin = new Thickness(path.Data.Bounds.X, path.Data.Bounds.Y, 0, 0);
			icon.SetBinding(Border.VisibilityProperty, availableBinding);


			var g = new Grid();
			g.Children.Add(isAvailableNow);
			g.Children.Add(path);
			g.Children.Add(icon);
			return g;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}

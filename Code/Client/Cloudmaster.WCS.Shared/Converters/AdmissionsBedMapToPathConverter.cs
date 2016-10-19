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
	/// Converts a bed name to a path for the admissions screen
	/// </summary>
	public class AdmissionsBedMapToPathConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values.Count() != 2)
				throw new ArgumentException("Wrong Argument amount");
			if (!(values[0] is FrameworkElement))
				return WpfExtensions<Path>.Clone(((FrameworkElement) values[0]).TryFindResource("timelineUnknownPath") as Path);

			var ui = (FrameworkElement) values[0];
			dynamic bed = values[1];

			var path = LocationToMapResource.GetBedPath(bed.LocationCode, bed.Room, bed.Bed);
			if (path == null)
			{
				var message = string.Concat(new[] {"Cannot find path for 'path", bed.LocationCode, bed.Room, bed.Bed, "'"});
				return null;
			}

			var colourHighlightedBinding = new MultiBinding();
			colourHighlightedBinding.Bindings.Add(new Binding() {Source = ui});
			colourHighlightedBinding.Bindings.Add(new Binding("IsHighlighted") {Source = bed});
			colourHighlightedBinding.Converter = new BedHighlightingToBrushConverter();

			var availableBinding = new Binding("IsAvailableNowForCleaning");
			availableBinding.Converter = new BooleanToVisibilityConverter();

			path.SetBinding(Shape.StrokeProperty, colourHighlightedBinding);
			path.StrokeThickness = 2.5;
			path.Margin = new Thickness(path.Data.Bounds.Left - 0.5, path.Data.Bounds.Top - 0.5, 0, 0);

			//var isAvailableNow = new FlashingControl();
			//isAvailableNow.SetBinding(UIElement.VisibilityProperty, availableBinding);

			var g = new Grid();
			//g.Children.Add(isAvailableNow);
			g.Children.Add(path);
			return g;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	/// <summary>
	/// Determines the colour of the bed on the map depending 
	/// </summary>
	public class AdmissionsBedStatusToBrushConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			return Brushes.Tomato;
			
			if (values.Count() != 2)
				throw new ArgumentException("Wrong Argument amount");
			if (!(values[0] is FrameworkElement) || values[1].GetType() != typeof (BedStatus))
				return Brushes.White;

			var ui = (FrameworkElement) values[0];

			
			//var status = (BedStatus)values[1];

			//switch (status)
			//{
			//    case BedStatus.Clean:
			//        return ui.TryFindResource("BedCleanStatusBrush") as Brush;
			//    case BedStatus.Dirty:
			//        return ui.TryFindResource("BedDirtyStatusBrush") as Brush;
			//    case BedStatus.RequiresDeepClean:
			//        return ui.TryFindResource("BedDeepCleanRequiredBrush") as Brush;

			//    default:
			//        {
			//            string parameterText = (string)parameter;
			//            throw new InvalidOperationException(String.Format("Invalid parameter value: {0}.", parameterText));
			//        }
			//}
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}

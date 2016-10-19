using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Cloudmaster.WCS.Galway.Core;
using WCS.Shared.Controls;
namespace WCS.Shared.Location
{
	public class RfidLocationTemplateSelector : DataTemplateSelector
	{
		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			var element = container as FrameworkElement;

			if (element != null && item != null && item is DetectionViewModel)
			{
				var det = item as DetectionViewModel;

				var loc = DetectionToRfidLocationMapper.Translate(det.Location);

				// create datatemplate locally
				var geo = RfidMapResource.GetLocationGeometry(loc);
				var dt = new DataTemplate();
				dt.DataType = typeof(DetectionViewModel); 

				var totalWidth = RfidMapResource.GetCanvasWidth("Document");
				var totalHeigth = RfidMapResource.GetCanvasHeight("Document");
				var offset = RfidMapResource.GetCanvasOffset(loc);
				var width = RfidMapResource.GetCanvasWidth(loc);
				var height = RfidMapResource.GetCanvasHeight(loc);
				

				var spFactory = new FrameworkElementFactory(typeof(Grid));
                spFactory.SetValue(Grid.BackgroundProperty, Brushes.Transparent);
                spFactory.SetValue(FrameworkElement.WidthProperty, totalWidth);
				spFactory.SetValue(FrameworkElement.HeightProperty, totalHeigth);
				spFactory.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Stretch);
				spFactory.SetValue(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);

				FrameworkElementFactory path = new FrameworkElementFactory(typeof(Path));
				path.SetValue(FrameworkElement.WidthProperty, totalWidth);
				path.SetValue(FrameworkElement.HeightProperty, totalHeigth);
				path.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Stretch);
				path.SetValue(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
				path.SetValue(Path.DataProperty, geo);
				path.SetValue(Shape.FillProperty, new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFB1EAFF")));
				path.SetValue(Shape.StrokeProperty, Brushes.Black);
				path.SetValue(Shape.StrokeThicknessProperty, 1.0);
				spFactory.AppendChild(path);

                FrameworkElementFactory beacon = new FrameworkElementFactory(typeof(BeaconControl));

                var beaconOffset = new Thickness((offset.Left + (width / 2) - 200), (offset.Top + (height / 2) - 200), 0, 0);

                beacon.SetValue(FrameworkElement.MarginProperty, beaconOffset);
				beacon.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Top);
				beacon.SetValue(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Left);
				beacon.SetValue(FrameworkElement.WidthProperty, 200.0);
				beacon.SetValue(FrameworkElement.HeightProperty, 200.0);
				spFactory.AppendChild(beacon);

				dt.VisualTree = spFactory;


				return dt;
			}

			return null;
		}
	}
}

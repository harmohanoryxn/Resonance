using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WCS.Shared.Schedule;

namespace WCS.Shared.Ward.Schedule
{
	public partial class WardPatientTimelineView : UserControl
	{
		public WardPatientTimelineView()
		{
			InitializeComponent();

			//var visibilising = new ObjectAnimationUsingKeyFrames();
			//visibilising.KeyFrames.Add(new DiscreteObjectKeyFrame(Visibility.Visible, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(500))));
			//var visabiliseStoryboard = new Storyboard();
			//visabiliseStoryboard.Children.Add(visibilising);
			//Storyboard.SetTarget(visibilising, wptv.card);
			//Storyboard.SetTargetProperty(visibilising, new PropertyPath("Visibility"));

			//var selectionTrigger = new DataTrigger
			//{
			//    Binding = new Binding("SelectionType"),
			//    Value = ScreenSelectionType.Selected
			//};
			//var bse = new BeginStoryboard { Storyboard = visabiliseStoryboard, Name = "visibleoStoryboard" };
			//var sse = new StopStoryboard { BeginStoryboardName = "visibleoStoryboard" };
			//selectionTrigger.EnterActions.Add(bse);
			//selectionTrigger.ExitActions.Add(sse);


			//var style = new Style(typeof(OrderCardView));
			//style.Setters.Add(new Setter(VisibilityProperty, Visibility.Collapsed));
			//style.RegisterName(bse.Name, bse);
			//style.Triggers.Add(selectionTrigger);
			//wptv.card.Style = style;
		}

		//public static readonly DependencyProperty DesiredGrowHeightProperty = DependencyProperty.RegisterAttached("DesiredGrowHeight", typeof(double), typeof(WardPatientTimelineView), new PropertyMetadata((obj, args) =>
		//{
		//    var pptv = (WardPatientTimelineView)obj;

		//    var expanding = new DoubleAnimation(Convert.ToDouble(args.NewValue), new Duration(TimeSpan.FromMilliseconds(500)));
		//    var expandingStoryboard = new Storyboard();
		//    expandingStoryboard.Children.Add(expanding);
		//    Storyboard.SetTarget(expanding, pptv.expandingGrowBorder);
		//    Storyboard.SetTargetProperty(expanding, new PropertyPath("Height"));

		//    var collasping = new DoubleAnimation(0.0, new Duration(TimeSpan.FromMilliseconds(500)));
		//    var collaspingStoryboard = new Storyboard();
		//    collaspingStoryboard.Children.Add(collasping);
		//    Storyboard.SetTarget(collasping, pptv.expandingGrowBorder);
		//    Storyboard.SetTargetProperty(collasping, new PropertyPath("Height"));

		//    var selectionTrigger = new DataTrigger
		//    {
		//        Binding = new Binding("SelectionType"),
		//        Value = ScreenSelectionType.Selected
		//    };
		//    var bse = new BeginStoryboard { Storyboard = expandingStoryboard, Name = "expandoStoryboard" };
		//    selectionTrigger.EnterActions.Add(bse);

		//    var bsc = new BeginStoryboard { Storyboard = collaspingStoryboard, Name = "collaspoStoryboard" };
		//    selectionTrigger.ExitActions.Add(bsc);

		//    var style = new Style(typeof(Border));
		//    style.Setters.Add(new Setter(HeightProperty, 1.0));
		//    style.RegisterName(bse.Name, bse);
		//    style.RegisterName(bsc.Name, bsc);
		//    style.Triggers.Add(selectionTrigger);
		//    pptv.expandingGrowBorder.Style = style;
		//}));

		//public double DesiredGrowHeight
		//{
		//    get { return (double)this.GetValue(DesiredGrowHeightProperty); }
		//    set { this.SetValue(DesiredGrowHeightProperty, value); }
		//}
	}
}

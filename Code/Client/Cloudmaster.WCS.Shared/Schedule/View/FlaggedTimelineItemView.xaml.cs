using System.Windows;
using System.Windows.Controls;

namespace WCS.Shared.Schedule
{
	public partial class FlaggedTimelineItemView : UserControl
	{
		public FlaggedTimelineItemView()
		{
			InitializeComponent();
		}

		public static readonly DependencyProperty HasTbaHourProperty = DependencyProperty.RegisterAttached("HasTbaHour", typeof(bool), typeof(FlaggedTimelineItemView), new PropertyMetadata(true));
		public bool HasTbaHour
		{
			get { return (bool)this.GetValue(HasTbaHourProperty); }
			set { this.SetValue(HasTbaHourProperty, value); }
		}

		public static readonly DependencyProperty ShiftDistanceProperty = DependencyProperty.RegisterAttached("ShiftDistance", typeof(double), typeof(FlaggedTimelineItemView), new PropertyMetadata(5.0));
		public double ShiftDistance
		{
			get { return (double)this.GetValue(ShiftDistanceProperty); }
			set { this.SetValue(ShiftDistanceProperty, value); }
		}
	}
}

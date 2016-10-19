using System.Windows;
using System.Windows.Controls;

namespace WCS.Shared.Schedule
{
	public partial class TimelineItemStretchedView : UserControl
	{
		public TimelineItemStretchedView()
		{
			InitializeComponent();
		}

		public static readonly DependencyProperty HasTbaHourProperty = DependencyProperty.RegisterAttached("HasTbaHour", typeof(bool), typeof(TimelineItemStretchedView), new PropertyMetadata(true));
		public bool HasTbaHour
		{
			get { return (bool)this.GetValue(HasTbaHourProperty); }
			set { this.SetValue(HasTbaHourProperty, value); }
		}

		public static readonly DependencyProperty ShiftDistanceProperty = DependencyProperty.RegisterAttached("ShiftDistance", typeof(double), typeof(TimelineItemStretchedView), new PropertyMetadata(0.0));
		public double ShiftDistance
		{
			get { return (double)this.GetValue(ShiftDistanceProperty); }
			set { this.SetValue(ShiftDistanceProperty, value); }
		}
	}
}

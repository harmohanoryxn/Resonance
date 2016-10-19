using System.Windows;
using System.Windows.Controls;
using WCS.Shared.Schedule;

namespace WCS.Shared.Location
{
	public partial class TrackingLocationView : UserControl
	{
		public TrackingLocationView()
		{
			InitializeComponent();
		}

		public static readonly DependencyProperty HasTbaHourProperty = DependencyProperty.RegisterAttached("HasTbaHour", typeof(bool), typeof(TrackingLocationView), new PropertyMetadata(true));
		public bool HasTbaHour
		{
			get { return (bool)this.GetValue(HasTbaHourProperty); }
			set { this.SetValue(HasTbaHourProperty, value); }
		}

		public static readonly DependencyProperty ShiftDistanceProperty = DependencyProperty.RegisterAttached("ShiftDistance", typeof(double), typeof(TrackingLocationView), new PropertyMetadata(0.0));
		public double ShiftDistance
		{
			get { return (double)this.GetValue(ShiftDistanceProperty); }
			set { this.SetValue(ShiftDistanceProperty, value); }
		}
	}
}

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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WCS.Shared.Schedule.View
{
	public partial class TimelineAvailableBedView : UserControl
	{
		public TimelineAvailableBedView()
		{
			InitializeComponent();
		}

		public static readonly DependencyProperty HasTbaHourProperty = DependencyProperty.RegisterAttached("HasTbaHour", typeof(bool), typeof(TimelineAvailableBedView), new PropertyMetadata(true));
		public bool HasTbaHour
		{
			get { return (bool)this.GetValue(HasTbaHourProperty); }
			set { this.SetValue(HasTbaHourProperty, value); }
		}

		public static readonly DependencyProperty ShiftDistanceProperty = DependencyProperty.RegisterAttached("ShiftDistance", typeof(double), typeof(TimelineAvailableBedView), new PropertyMetadata(0.0));
		public double ShiftDistance
		{
			get { return (double)this.GetValue(ShiftDistanceProperty); }
			set { this.SetValue(ShiftDistanceProperty, value); }
		}
	}
}

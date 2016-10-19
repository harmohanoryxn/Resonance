using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Cloudmaster.WCS.Classes;

namespace Cloudmaster.WCS.Model
{
    public class Labels : DependencyObject
    {
        public string CleaningLabel
        {
            get { return (string)GetValue(CleaningLabelProperty); }
            set { SetValue(CleaningLabelProperty, value); }
        }

        public static readonly DependencyProperty CleaningLabelProperty =
            DependencyProperty.Register("CleaningLabel", typeof(string), typeof(Labels), new UIPropertyMetadata("Cleaning"));

        public string AdmissionsLabel
        {
            get { return (string)GetValue(AdmissionsLabelProperty); }
            set { SetValue(AdmissionsLabelProperty, value); }
        }

        public static readonly DependencyProperty AdmissionsLabelProperty =
            DependencyProperty.Register("AdmissionsLabel", typeof(string), typeof(Labels), new UIPropertyMetadata("Appointments"));

        public string EnvironmentLabel
        {
            get { return (string)GetValue(EnvironmentLabelProperty); }
            set { SetValue(EnvironmentLabelProperty, value); }
        }

        public static readonly DependencyProperty EnvironmentLabelProperty =
            DependencyProperty.Register("EnvironmentLabel", typeof(string), typeof(Labels), new UIPropertyMetadata("Environment"));

        public string FireSafetyLabel
        {
            get { return (string)GetValue(FireSafetyLabelProperty); }
            set { SetValue(FireSafetyLabelProperty, value); }
        }

        public static readonly DependencyProperty FireSafetyLabelProperty =
            DependencyProperty.Register("FireSafetyLabel", typeof(string), typeof(Labels), new UIPropertyMetadata("FireSafety"));

        public string WorkRequestsLabel
        {
            get { return (string)GetValue(WorkRequestsLabelProperty); }
            set { SetValue(WorkRequestsLabelProperty, value); }
        }

        public static readonly DependencyProperty WorkRequestsLabelProperty =
            DependencyProperty.Register("WorkRequestsLabel", typeof(string), typeof(Labels), new UIPropertyMetadata("Work Requests (0)"));

       
    }
}

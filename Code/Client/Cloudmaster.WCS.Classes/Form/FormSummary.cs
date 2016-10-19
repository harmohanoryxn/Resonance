using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Cloudmaster.WCS.Classes
{
    public class FormSummary : DependencyObject
    {
        public Guid Id
        {
            get { return (Guid)GetValue(IdProperty); }
            set { SetValue(IdProperty, value); }
        }

        public static readonly DependencyProperty IdProperty =
            DependencyProperty.Register("Id", typeof(Guid), typeof(FormSummary), new UIPropertyMetadata(null));

        public string FormDefinitionName
        {
            get { return (string)GetValue(FormDefinitionNameProperty); }
            set { SetValue(FormDefinitionNameProperty, value); }
        }

        public static readonly DependencyProperty FormDefinitionNameProperty =
            DependencyProperty.Register("FormDefinitionName", typeof(string), typeof(FormSummary), new UIPropertyMetadata(string.Empty));

        public string FormDefinitionDescription
        {
            get { return (string)GetValue(FormDefinitionDescriptionProperty); }
            set { SetValue(FormDefinitionDescriptionProperty, value); }
        }

        public static readonly DependencyProperty FormDefinitionDescriptionProperty =
            DependencyProperty.Register("FormDefinitionDescription", typeof(string), typeof(FormSummary), new UIPropertyMetadata(string.Empty));

        public Guid FormDefinitionId
        {
            get { return (Guid)GetValue(FormDefinitionIdProperty); }
            set { SetValue(FormDefinitionIdProperty, value); }
        }

        public static readonly DependencyProperty FormDefinitionIdProperty =
            DependencyProperty.Register("FormDefinitionId", typeof(Guid), typeof(FormSummary), new UIPropertyMetadata(null));

        public string FormDefinitionFrequency
        {
            get { return (string)GetValue(FormDefinitionFrequencyProperty); }
            set { SetValue(FormDefinitionFrequencyProperty, value); }
        }

        public static readonly DependencyProperty FormDefinitionFrequencyProperty =
            DependencyProperty.Register("FormDefinitionFrequency", typeof(string), typeof(FormSummary), new UIPropertyMetadata(string.Empty));

        public string DueDateDescription
        {
            get { return (string)GetValue(DueDateDescriptionProperty); }
            set { SetValue(DueDateDescriptionProperty, value); }
        }

        public static readonly DependencyProperty DueDateDescriptionProperty =
            DependencyProperty.Register("DueDateDescription", typeof(string), typeof(FormSummary), new UIPropertyMetadata(string.Empty));

        public DateTime DateLastCompleted
        {
            get { return (DateTime)GetValue(DateLastCompletedProperty); }
            set { SetValue(DateLastCompletedProperty, value); }
        }

        public static readonly DependencyProperty DateLastCompletedProperty =
            DependencyProperty.Register("DateLastCompleted", typeof(DateTime), typeof(FormSummary), new UIPropertyMetadata(null));

        public int Priority
        {
            get { return (int)GetValue(PriorityProperty); }
            set { SetValue(PriorityProperty, value); }
        }

        public static readonly DependencyProperty PriorityProperty =
            DependencyProperty.Register("Priority", typeof(int), typeof(FormSummary), new UIPropertyMetadata(0));
    }
}

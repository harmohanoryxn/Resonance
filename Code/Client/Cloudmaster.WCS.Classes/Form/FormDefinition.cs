using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;

namespace Cloudmaster.WCS.Classes
{
    public class FormDefinition : DependencyObject
    {
        internal FormDefinition() 
        {
            Sections = new ObservableCollection<Section>();
        }

        public FormDefinition(Guid id)
        {
            Id = id;

            Sections = new ObservableCollection<Section>();
        }

        public Guid Id
        {
            get { return (Guid)GetValue(IdProperty); }
            set { SetValue(IdProperty, value); }
        }

        public static readonly DependencyProperty IdProperty =
            DependencyProperty.Register("Id", typeof(Guid), typeof(FormDefinition), new UIPropertyMetadata(null));

        public string Name
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        public static readonly DependencyProperty NameProperty =
            DependencyProperty.Register("Name", typeof(string), typeof(FormDefinition), new UIPropertyMetadata(string.Empty));

        public string EntityType
        {
            get { return (string)GetValue(EntityTypeProperty); }
            set { SetValue(EntityTypeProperty, value); }
        }

        public static readonly DependencyProperty EntityTypeProperty =
            DependencyProperty.Register("EntityType", typeof(string), typeof(FormDefinition), new UIPropertyMetadata(string.Empty));

        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(FormDefinition), new UIPropertyMetadata(string.Empty));

        public string Frequency
        {
            get { return (string)GetValue(FrequencyProperty); }
            set { SetValue(FrequencyProperty, value); }
        }

        public static readonly DependencyProperty FrequencyProperty =
            DependencyProperty.Register("Frequency", typeof(string), typeof(FormDefinition), new UIPropertyMetadata(string.Empty));

        public ObservableCollection<Section> Sections
        {
            get { return (ObservableCollection<Section>)GetValue(SectionsProperty); }
            set { SetValue(SectionsProperty, value); }
        }

        public static readonly DependencyProperty SectionsProperty =
            DependencyProperty.Register("Sections", typeof(ObservableCollection<Section>), typeof(FormDefinition), new UIPropertyMetadata(null));

        public int FormCategory
        {
            get { return (int)GetValue(FormCategoryProperty); }
            set { SetValue(FormCategoryProperty, value); }
        }

        public static readonly DependencyProperty FormCategoryProperty =
            DependencyProperty.Register("FormCategory", typeof(int), typeof(FormDefinition), new UIPropertyMetadata(0));

    }
}

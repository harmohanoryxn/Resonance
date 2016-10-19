using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Cloudmaster.WCS.Classes
{
    public partial class Section : DependencyObject
    {
        internal Section()
        {
            Checks = new CheckCollection();
        }

        public Section(Guid id)
        {
            Id = id;

            Checks = new CheckCollection();
        }

        public Guid Id
        {
            get { return (Guid) GetValue(IdProperty); }
            set { SetValue(IdProperty, value); }
        }

        public static readonly DependencyProperty IdProperty =
            DependencyProperty.Register("Id", typeof(Guid), typeof(Section), new UIPropertyMetadata(null));

        public string Name
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        public static readonly DependencyProperty NameProperty =
            DependencyProperty.Register("Name", typeof(string), typeof(Section), new UIPropertyMetadata(string.Empty));

        public CheckCollection Checks
        {
            get { return (CheckCollection)GetValue(ChecksProperty); }
            set { SetValue(ChecksProperty, value); }
        }

        public static readonly DependencyProperty ChecksProperty =
            DependencyProperty.Register("Checks", typeof(CheckCollection), typeof(Section), new UIPropertyMetadata(null));

        public string Comments
        {
            get { return (string)GetValue(CommentsProperty); }
            set { SetValue(CommentsProperty, value); }
        }

        public static readonly DependencyProperty CommentsProperty =
            DependencyProperty.Register("Comments", typeof(string), typeof(FormInstance), new UIPropertyMetadata(string.Empty));
    }
}

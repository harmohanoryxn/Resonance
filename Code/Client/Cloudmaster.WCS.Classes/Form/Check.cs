using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;

namespace Cloudmaster.WCS.Classes
{
    public partial class Check : DependencyObject, ICloneable
    {
        internal Check()
        {
            UserImages = new ObservableCollection<RelatedFile>();
        }

        public Check(Guid id)
        {
            Id = id;

            UserImages = new ObservableCollection<RelatedFile>();

            InvalidateIsValid();
        }

        public static Check CreateNew (string categoryName, Guid categoryId, string name)
        {
            Check check = new Check();

            check.Id = Guid.NewGuid();
            check.Name = name;

            return check;
        }

        public object Clone()
        {
            Check clonedCheck = new Check();

            clonedCheck.Id = Id;
            clonedCheck.CWorksId = clonedCheck.CWorksId;
            clonedCheck.Name = Name;
            clonedCheck.Description = Description;
            clonedCheck.DefaultValue = DefaultValue;
            clonedCheck.IsRequired = IsRequired;
            clonedCheck.Result = "True";

            return clonedCheck;
        }

        public Guid Id
        {
            get { return (Guid)GetValue(IdProperty); }
            set { SetValue(IdProperty, value); }
        }

        public static readonly DependencyProperty IdProperty =
            DependencyProperty.Register("Id", typeof(Guid), typeof(Check), new UIPropertyMetadata(null));

        public string CWorksId
        {
            get { return (string)GetValue(CWorksIdProperty); }
            set { SetValue(CWorksIdProperty, value); }
        }

        public static readonly DependencyProperty CWorksIdProperty =
            DependencyProperty.Register("CWorksId", typeof(string), typeof(Check), new UIPropertyMetadata(string.Empty));

        public string Target
        {
            get { return (string)GetValue(TargetProperty); }
            set { SetValue(TargetProperty, value); }
        }

        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.Register("Target", typeof(string), typeof(Check), new UIPropertyMetadata(string.Empty));

        public string Name
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        public static readonly DependencyProperty NameProperty =
            DependencyProperty.Register("Name", typeof(string), typeof(Check), new UIPropertyMetadata(string.Empty));

        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(Check), new UIPropertyMetadata(string.Empty));

        public ObservableCollection<RelatedFile> UserImages
        {
            get { return (ObservableCollection<RelatedFile>)GetValue(UserImagesProperty); }
            set { SetValue(UserImagesProperty, value); }
        }

        public static readonly DependencyProperty UserImagesProperty =
            DependencyProperty.Register("UserImages", typeof(ObservableCollection<RelatedFile>), typeof(Check), new UIPropertyMetadata(null));

        public string Result
        {
            get { return (string)GetValue(ResultProperty); }
            set { SetValue(ResultProperty, value); }
        }

        public static readonly DependencyProperty ResultProperty =
            DependencyProperty.Register("Result", typeof(string), typeof(Check), new UIPropertyMetadata(string.Empty, new PropertyChangedCallback(OnResultPropertyChanged)));

        private static void OnResultPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            Check check = (Check)dependencyObject;

            string newValue = (string) e.NewValue;

            check.InvalidateIsValid();
        }

        private void InvalidateIsValid()
        {
            if (Result.CompareTo(string.Empty) == 0)
            {
                IsValid = !IsRequired;
            }
            else
            {
                IsValid = (Result.CompareTo("True") == 0);
            }
        }

        public bool IsValid
        {
            get { return (bool)GetValue(IsValidProperty); }
            set { SetValue(IsValidProperty, value); }
        }

        public static readonly DependencyProperty IsValidProperty =
            DependencyProperty.Register("IsValid", typeof(bool), typeof(Check), new UIPropertyMetadata(false));

        public string DefaultValue
        {
            get { return (string)GetValue(DefaultValueProperty); }
            set { SetValue(DefaultValueProperty, value); }
        }

        public static readonly DependencyProperty DefaultValueProperty =
            DependencyProperty.Register("DefaultValue", typeof(string), typeof(Check), new UIPropertyMetadata(string.Empty));

        public bool IsRequired
        {
            get { return (bool)GetValue(IsRequiredProperty); }
            set { SetValue(IsRequiredProperty, value); }
        }

        public static readonly DependencyProperty IsRequiredProperty =
            DependencyProperty.Register("IsRequired", typeof(bool), typeof(Check), new UIPropertyMetadata(false));

        public string AssetNumber
        {
            get { return (string)GetValue(AssetNumberProperty); }
            set { SetValue(AssetNumberProperty, value); }
        }

        public static readonly DependencyProperty AssetNumberProperty =
            DependencyProperty.Register("AssetNumber", typeof(string), typeof(Check), new UIPropertyMetadata(string.Empty));

        public string Comments
        {
            get { return (string)GetValue(CommentsProperty); }
            set { SetValue(CommentsProperty, value); }
        }

        public static readonly DependencyProperty CommentsProperty =
            DependencyProperty.Register("Comments", typeof(string), typeof(Check), new UIPropertyMetadata(string.Empty ));

        public string TaskId
        {
            get { return (string)GetValue(TaskIdProperty); }
            set { SetValue(TaskIdProperty, value); }
        }

        public static readonly DependencyProperty TaskIdProperty =
            DependencyProperty.Register("TaskId", typeof(string), typeof(Check), new UIPropertyMetadata(string.Empty));
    }
}

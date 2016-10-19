using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Cloudmaster.WCS.Classes
{
    public partial class RelatedFile : DependencyObject
    {
        internal RelatedFile()
        {

        }

        public RelatedFile(Guid id)
        {
            Id = id;
        }

        public Guid Id
        {
            get { return (Guid) GetValue(IdProperty); }
            set { SetValue(IdProperty, value); }
        }

        public static readonly DependencyProperty IdProperty =
            DependencyProperty.Register("Id", typeof(Guid), typeof(RelatedFile), new UIPropertyMetadata(null));

        public string LocalFilename
        {
            get { return (string)GetValue(LocalFilenameProperty); }
            set { SetValue(LocalFilenameProperty, value); }
        }

        public static readonly DependencyProperty LocalFilenameProperty =
            DependencyProperty.Register("LocalFilename", typeof(string), typeof(RelatedFile), new UIPropertyMetadata(string.Empty));

        public string StorageFilename
        {
            get { return (string)GetValue(StorageFilenameProperty); }
            set { SetValue(StorageFilenameProperty, value); }
        }

        public static readonly DependencyProperty StorageFilenameProperty =
            DependencyProperty.Register("StorageFilename", typeof(string), typeof(RelatedFile), new UIPropertyMetadata(string.Empty));
    }
}

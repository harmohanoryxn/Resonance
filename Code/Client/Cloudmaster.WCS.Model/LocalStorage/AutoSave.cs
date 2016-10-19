using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Cloudmaster.WCS.IO;
using Cloudmaster.WCS.Packaging;
using Cloudmaster.WCS.Classes;
using System.IO;
using System.Collections.ObjectModel;

namespace Cloudmaster.WCS.Model.LocalStorage
{
    public class AutoSave<T, P> : DependencyObject where  T : IAutoSaveable where P : IPackageable<T> 
    {
        private P Packager;

        private static readonly string LocalStorageFolderName = "AutoSave";

        public AutoSave(P packager)
        {
            Packager = packager;
        }

        public void Initialize ()
        {
            string absolutePath = LocalStorageHelper.GetAbsoluteFilenameInLocalStorage(LocalStorageFolderName);

            if (Directory.Exists(absolutePath))
            {
                string[] files = Directory.GetFiles(absolutePath);

                if (files.Count() > 0)
                {
                    HasItems = true;
                }

                foreach (string file in files)
                {
                    try
                    {
                        T item = Packager.Unpackage(file);

                        item.LastSaved = File.GetLastWriteTime(file);

                        Items.Add(item);
                    }
                    catch (Exception ex)
                    {
                        // Ignore
                    }
                }
            }
        }

        public void Clear()
        {
            string absolutePath = LocalStorageHelper.GetAbsoluteFilenameInLocalStorage(LocalStorageFolderName);

            if (Directory.Exists(absolutePath))
            {
                string[] files = Directory.GetFiles(absolutePath);

                 foreach (string file in files)
                 {
                     File.Delete(file);
                 }
            }
        }

        public void Save(T item)
        {
            string uniqueFilename = item.CreateUniqueFilename();

            string filenameWithoutExtension = LocalStorageHelper.GetAbsoluteFilenameInLocalStorage(LocalStorageFolderName, uniqueFilename);

            string filename = string.Format("{0}.zip", filenameWithoutExtension);

            Packager.Package(item, filename);
        }

        public ObservableCollection<T> Items
        {
            get { return (ObservableCollection<T>)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register("Queue", typeof(ObservableCollection<T>), typeof(AutoSave<T, P>), new UIPropertyMetadata(null));

        public bool HasItems
        {
            get { return (bool)GetValue(HasPreviousAutoSavesProperty); }
            set { SetValue(HasPreviousAutoSavesProperty, value); }
        }

        public static readonly DependencyProperty HasPreviousAutoSavesProperty =
            DependencyProperty.Register("HasPreviousAutoSaves", typeof(bool), typeof(AutoSave<T, P>), new UIPropertyMetadata(false));


        public T GetMostRecentItem()
        {
            T result = Items.OrderByDescending(item => item.LastSaved).FirstOrDefault();

            return result;
        }
    }
}

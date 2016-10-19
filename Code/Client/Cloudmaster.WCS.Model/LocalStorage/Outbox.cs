using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows;
using Cloudmaster.WCS.Classes;
using System.Xml.Serialization;
using System.IO;
using Cloudmaster.WCS.IO;
using Cloudmaster.WCS.Packaging;

namespace Cloudmaster.WCS.Model.LocalStroage
{
    public class Outbox<T, P> : DependencyObject where T : IOutboxable where P : IPackageable<T>
    {
        private static readonly string LocalStorageFolderName = "Outbox";

        private P Packager;

        public Outbox(P packager)
        {
            Packager = packager;
        }

        public void Initialize()
        {
            Queue = new ObservableCollection<T>();

            string absolutePath = LocalStorageHelper.GetAbsoluteFilenameInLocalStorage(LocalStorageFolderName);

            if (Directory.Exists(absolutePath))
            {
                string[] files = Directory.GetFiles(absolutePath);

                foreach (string file in files)
                {
                    try
                    {
                        T item = Packager.Unpackage(file);

                        if (item.OutboxStatus != OutboxStatus.Completed)
                        {
                            Queue.Add(item);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Ignore
                    }
                }
            }
        }

        private object saveLock = new object();

        public void Save()
        {
            lock (saveLock)
            {
                foreach (T item in Queue)
                {
                    string uniqueFilename = item.CreateUniqueFilename();

                    string filenameWithoutExtension = LocalStorageHelper.GetAbsoluteFilenameInLocalStorage(LocalStorageFolderName, uniqueFilename);

                    string filename = string.Format("{0}.zip", filenameWithoutExtension);

                    Packager.Package(item, filename);
                }
            }
        }

        public string OutboxLabel
        {
            get { return (string)GetValue(OutboxLabelProperty); }
            set { SetValue(OutboxLabelProperty, value); }
        }

        public static readonly DependencyProperty OutboxLabelProperty =
            DependencyProperty.Register("OutboxLabel", typeof(string), typeof(Outbox<T, P>), new UIPropertyMetadata("Outbox (0)"));

        public void RefreshOutboxLabel()
        {
            int numberOfOutstandingOperationsInQueue = 0;

            foreach (T t in Queue)
            {
                if (t.OutboxStatus != OutboxStatus.Completed)
                {
                    numberOfOutstandingOperationsInQueue += 1;
                }
            }

            string outboxLabel = string.Format("Outbox ({0})", numberOfOutstandingOperationsInQueue.ToString());

            OutboxLabel = outboxLabel;
        }

        public ObservableCollection<T> Queue
        {
            get { return (ObservableCollection<T>)GetValue(QueueProperty); }
            set { SetValue(QueueProperty, value); }
        }

        public static readonly DependencyProperty QueueProperty =
            DependencyProperty.Register("Queue", typeof(ObservableCollection<T>), typeof(Outbox<T, P>), new UIPropertyMetadata(null));

        
        public int CalculateNumberOfUnsentItemsInOutbox()
        {
            int result = 0;

            foreach (T item in Queue)
            {
                if (item.OutboxStatus != OutboxStatus.Completed)
                {
                    result += 1;
                }
            }

            return result;
        }

        static void Queue_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //FormManager.Instance.RefreshLabels();
        }

        public T SelectedItem
        {
            get { return (T)GetValue(SelectedFormInQueueProperty); }
            set { SetValue(SelectedFormInQueueProperty, value); }
        }

        public static readonly DependencyProperty SelectedFormInQueueProperty =
            DependencyProperty.Register("SelectedFormInQueue", typeof(T), typeof(Outbox<T, P>), new UIPropertyMetadata(null));

    }
}

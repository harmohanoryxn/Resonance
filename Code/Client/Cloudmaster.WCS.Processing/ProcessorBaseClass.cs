using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Cloudmaster.WCS.Classes;
using Cloudmaster.WCS.IO;
using Cloudmaster.WCS.IO;
using Cloudmaster.WCS.Model;
using System.IO;

namespace Cloudmaster.WCS.Processing
{
	/// <summary>
	/// Base View Model type with in-built background processing functionality
	/// </summary>
    public abstract class ProcessorBaseClass : DependencyObject
    {
        protected BackgroundWorker backgroundWorker;

        protected ProcessorBaseClass()
        {
            //Log = new ObservableCollection<string>();

            InitializeBackgroundWorker();
        }

        protected void InitializeBackgroundWorker()
        {
            if (backgroundWorker != null)
            {
                DisposeOfBackgroundWorker();
            }

            backgroundWorker = new BackgroundWorker();

            backgroundWorker.WorkerReportsProgress = true;

            backgroundWorker.WorkerSupportsCancellation = true;
            backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(ProgressChanged);

            backgroundWorker.DoWork += new DoWorkEventHandler(DoWork);
            backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(RunWorkerCompleted);
        }

        protected void DisposeOfBackgroundWorker()
        {
            backgroundWorker.ProgressChanged -= new ProgressChangedEventHandler(ProgressChanged);
            backgroundWorker.DoWork -= new DoWorkEventHandler(DoWork);
            backgroundWorker.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(RunWorkerCompleted);

            backgroundWorker.Dispose();

            backgroundWorker = null;
        }

        void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            IsWorking = false;

            OnRunWorkerCompleted(e);
        }

        protected abstract void OnRunWorkerCompleted(RunWorkerCompletedEventArgs e);

        protected void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Progress = e.ProgressPercentage;

            string message = (string) e.UserState;

            if (message != null)
            {
                if (message.Substring(0, 5).CompareTo("Error") == 0)
                {
                    // HasErrors = true;
                }

                // Log.Add(message);
            }
        }

        protected static void CleanUpTemporaryFile(string filename)
        {
            try
            {
                // Clean up temporary file

                File.Delete(filename);
            }
            catch (Exception ex) { }
        }

        protected void ReportError(Exception ex, int percentage, string prefix)
        {
            string errorMessage = string.Format("Error ({0}%): {1} Reported Error: {2}", percentage, prefix, ex.Message);

            backgroundWorker.ReportProgress(percentage, errorMessage);
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            OnDoWork(e);
        }

        protected abstract void OnDoWork(DoWorkEventArgs e);
        
        public double Progress
        {
            get { return (double)GetValue(ProgressProperty); }
            set { SetValue(ProgressProperty, value); }
        }

        public static readonly DependencyProperty ProgressProperty =
            DependencyProperty.Register("Progress", typeof(double), typeof(ProcessorBaseClass), new UIPropertyMetadata(20.0));

        public bool IsWorking
        {
            get { return (bool)GetValue(IsWorkingProperty); }
            set { SetValue(IsWorkingProperty, value); }
        }

        public static readonly DependencyProperty IsWorkingProperty =
            DependencyProperty.Register("IsWorking", typeof(bool), typeof(ProcessorBaseClass), new UIPropertyMetadata(false));
        /*
        public bool HasErrors
        {
            get { return (bool)GetValue(HasErrorsProperty); }
            set { SetValue(HasErrorsProperty, value); }
        }

        public static readonly DependencyProperty HasErrorsProperty =
            DependencyProperty.Register("HasErrors", typeof(bool), typeof(ProcessorBaseClass), new UIPropertyMetadata(false));

        public ObservableCollection<string> Log
        {
            get { return (ObservableCollection<string>)GetValue(LogProperty); }
            set { SetValue(LogProperty, value); }
        }

        public static readonly DependencyProperty LogProperty =
            DependencyProperty.Register("Log", typeof(ObservableCollection<string>), typeof(ProcessorBaseClass), new UIPropertyMetadata(null));

        */

        public static void SaveXml(ServerInformation obj, string relativeFolder, string relativeFilename)
        {
            string absoluteFilename = LocalStorageHelper.GetAbsoluteFilenameInLocalStorage(relativeFolder, relativeFilename);

            XmlTypeSerializer<ServerInformation>.SerializeAndOverwriteFile(obj, absoluteFilename);
        }

        /*
        public DateTime LastUpdated
        {
            get { return (DateTime)GetValue(LastUpdatedProperty); }
            set { SetValue(LastUpdatedProperty, value); }
        }

        public static readonly DependencyProperty LastUpdatedProperty =
            DependencyProperty.Register("LastUpdated", typeof(DateTime), typeof(ProcessorBaseClass), new UIPropertyMetadata(null));
         * */
    }
}

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
using System.Windows.Shapes;
using CatenaLogic.Windows.Presentation.WebcamPlayer;
using System.IO;
using Cloudmaster.WCS.Classes;
using IdentityMine.Windows.Essentials;
using Cloudmaster.WCS.Model;

namespace Cloudmaster.WCS.Controls.Windows
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class CaptureImageWindow : Window
    {
        public static CaptureImageWindow Instance
        {
            get
            {
                // if null, create and initialize the one and only window instance
                if (_instance == null)
                {
                    _instance = new CaptureImageWindow();
                    _instance.InitializeComponent();
                }
                return _instance;
            }
        }

        public CaptureImageWindow()
        {
            InitializeComponent();

            _instance = this;

            CommandBindings.Add(new CommandBinding(WebcamPlayer.UI.Input.CaptureImageCommands.CaptureImage,
                new ExecutedRoutedEventHandler(CaptureImage_Executed), new CanExecuteRoutedEventHandler(CaptureImage_CanExecute)));
            CommandBindings.Add(new CommandBinding(WebcamPlayer.UI.Input.CaptureImageCommands.RemoveImage,
                new ExecutedRoutedEventHandler(RemoveImage_Executed)));
            CommandBindings.Add(new CommandBinding(WebcamPlayer.UI.Input.CaptureImageCommands.ClearAllImages,
                new ExecutedRoutedEventHandler(ClearAllImages_Executed)));

            CommandBindings.Add(new CommandBinding(WebcamPlayer.UI.Input.CaptureImageCommands.AddImage,
                new ExecutedRoutedEventHandler(AddImage_Executed), new CanExecuteRoutedEventHandler(AddImage_CanExecute)));


            // Create default device
            SelectedWebcamMonikerString = (CapDevice.DeviceMonikers.Length > 0) ? CapDevice.DeviceMonikers[0].MonikerString : "";
        
        }

        private void InitializeCommands()
        {
            CommandBindings.Add(new CommandBinding(CloseWindow, CloseWindowExecuted));
            CommandBindings.Add(new CommandBinding(ToggleMinimizedState, MinimizeWindowExecuted));
            CommandBindings.Add(new CommandBinding(ToggleMaximizedState, MaximizeWindowExecuted));
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            InitializeCommands();
        }

        #region Window Commands

        #region CloseWindow

        public static RoutedUICommand CloseWindow = new RoutedUICommand("Close", "CloseWindow", typeof(ChromelessWindow));

        
        private void CloseWindowExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Close();

            try
            {
                if (SelectedWebcam != null)
                {
                    //SelectedWebcam.Stop();

                    //SelectedWebcamMonikerString = string.Empty;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Unable to shutdown camera");
            }
            
        }

        #endregion

        #region ToggleMaximizedState

        public static RoutedUICommand ToggleMaximizedState = new RoutedUICommand("Maximize", "ToggleMaximizedState", typeof(ChromelessWindow));
        
        private void MaximizeWindowExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            CaptureImageWindow.Instance.WindowState = (CaptureImageWindow.Instance.WindowState == WindowState.Maximized) ? WindowState.Normal : WindowState.Maximized;
        }

        #endregion

        #region ToggleMinimizedState

        public static RoutedUICommand ToggleMinimizedState = new RoutedUICommand("Minimize", "ToggleMinimizedState", typeof(ChromelessWindow));
        private void MinimizeWindowExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            CaptureImageWindow.Instance.WindowState = (CaptureImageWindow.Instance.WindowState == WindowState.Minimized) ? WindowState.Normal : WindowState.Minimized;
        }

        #endregion

        #endregion

        private static CaptureImageWindow _instance = null;

        #region Command bindings
        /// <summary>
        /// Determines whether the CaptureImage command can be executed
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">EventArgs</param>
        private void CaptureImage_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            // Check if there is a valid webcam
            e.CanExecute = (SelectedWebcam != null);
        }

        private void AddImage_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            // Check if there is a valid webcam
            e.CanExecute = (PreviewImage != null);
        }

        /// <summary>
        /// Invoked when the CaptureImage command is executed
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">EventArgs</param>
        private void CaptureImage_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //// Store current image in the webcam

            BitmapSource bitmap = webcamPlayer.CurrentBitmap;

            if (bitmap != null)
            {
                //SelectedImages.Add(bitmap);
                PreviewImage = bitmap;
            }
        }

        
        private void AddImage_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Close();

            if (PreviewImage != null)
            {
                string temporaryFilename = System.IO.Path.GetTempFileName();

                FileStream stream = new FileStream(temporaryFilename, FileMode.Create);

                JpegBitmapEncoder encoder = new JpegBitmapEncoder();

                encoder.QualityLevel = 100;

                encoder.Frames.Add(BitmapFrame.Create(PreviewImage));
                encoder.Save(stream);

                stream.Close();

                if (FormManager.Instance.SelectedCheck != null)
                {
                    FormManager.Instance.SelectedCheck.UserImages.Add(new RelatedFile(Guid.NewGuid()) { LocalFilename = temporaryFilename });
                }

                try
                {
                    if (SelectedWebcam != null)
                    {
                        //SelectedWebcam.Stop();

                        SelectedWebcamMonikerString = string.Empty;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Unable to shutdown camera");
                }
            }

        }
  

        /// <summary>
        /// Invoked when the RemoveImage command is executed
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">EventArgs</param>
        private void RemoveImage_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // Store current image in the webcam
            BitmapSource bitmap = e.Parameter as BitmapSource;
            if (bitmap != null)
            {
                //SelectedImages.Remove(bitmap);
            }
        }

        /// <summary>
        /// Invoked when the ClearAllImages command is executed
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">EventArgs</param>
        private void ClearAllImages_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // Clear all images
            //SelectedImages.Clear();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Wrapper for the WebcamRotation dependency property
        /// </summary>
        public double WebcamRotation
        {
            get { return (double)GetValue(WebcamRotationProperty); }
            set { SetValue(WebcamRotationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WebcamRotation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WebcamRotationProperty =
            DependencyProperty.Register("WebcamRotation", typeof(double), typeof(CaptureImageWindow), new UIPropertyMetadata(180d));

        /// <summary>
        /// Wrapper for the SelectedWebcam dependency property
        /// </summary>
        public CapDevice SelectedWebcam
        {
            get { return (CapDevice)GetValue(SelectedWebcamProperty); }
            set { SetValue(SelectedWebcamProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedWebcam.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedWebcamProperty =
            DependencyProperty.Register("SelectedWebcam", typeof(CapDevice), typeof(CaptureImageWindow), new UIPropertyMetadata(null));

        /// <summary>
        /// Wrapper for the SelectedWebcamMonikerString dependency property
        /// </summary>
        public string SelectedWebcamMonikerString
        {
            get { return (string)GetValue(SelectedWebcamMonikerStringProperty); }
            set { SetValue(SelectedWebcamMonikerStringProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedWebcamMonikerString.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedWebcamMonikerStringProperty = DependencyProperty.Register("SelectedWebcamMonikerString", typeof(string),
            typeof(CaptureImageWindow), new UIPropertyMetadata("", new PropertyChangedCallback(SelectedWebcamMonikerString_Changed)));


        public BitmapSource PreviewImage
        {
            get { return (BitmapSource)GetValue(PreviewImageProperty); }
            set { SetValue(PreviewImageProperty, value); }
        }

        public static readonly DependencyProperty PreviewImageProperty =
            DependencyProperty.Register("PreviewImage", typeof(BitmapSource), typeof(CaptureImageWindow), new UIPropertyMetadata(null));




        /*
        /// <summary>
        /// Wrapper for the SelectedImages dependency property
        /// </summary>
        public ObservableCollection<BitmapSource> SelectedImages
        {
            get { return (ObservableCollection<BitmapSource>)GetValue(SelectedImagesProperty); }
            set { SetValue(SelectedImagesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedImages.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedImagesProperty = DependencyProperty.Register("SelectedImages", typeof(ObservableCollection<BitmapSource>),
            typeof(CaptureImageWindow), new UIPropertyMetadata(new ObservableCollection<BitmapSource>()));
        */

        #endregion

        #region Methods
        /// <summary>
        /// Invoked when the SelectedWebcamMonikerString dependency property has changed
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">EventArgs</param>
        private static void SelectedWebcamMonikerString_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            // Get typed sender
            CaptureImageWindow typedSender = sender as CaptureImageWindow;
            if (typedSender != null)
            {
                // Get new value
                string newMonikerString = e.NewValue as string;

                // Update the device
                if (typedSender.SelectedWebcam == null)
                {
                    // Create it
                    typedSender.SelectedWebcam = new CapDevice("");
                }

                // Now set the moniker string
                typedSender.SelectedWebcam.MonikerString = newMonikerString;
            }
        }
        #endregion
    }
}

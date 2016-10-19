using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Ink;
using Cloudmaster.WCS.Classes;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Effects;
using Cloudmaster.WCS.Model;
using Cloudmaster.WCS.Controls.Windows;

namespace Cloudmaster.WCS.Controls.Forms.Commands
{
    public class ImageOperationCommands
    {
        #region Sign Form

        public static RoutedUICommand SignFormCommand = new RoutedUICommand("SignFormCommand", "SignFormCommand", typeof(Window));

        public static void SignFormCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Window window = (Window) sender;

            DrawingVisual drawingVisual = new DrawingVisual();

            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                drawingContext.DrawRectangle(Brushes.White, null, new Rect(0, 0, 760, 280));

                FormManager.Instance.Strokes.SignatureStrokes.Draw(drawingContext);
            }

            FormManager.Instance.SelectedForm.Signature = ImageManager.AddDrawingVisualToRelatedFile(drawingVisual, 720, 280);

            window.Close();
        }

        public static void SignFormCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (FormManager.Instance.Strokes.SignatureStrokes.Count > 0);
        }

        #endregion

        #region Image NavigationViewModel Buttons

        public static RoutedUICommand OpenImageCommand = new RoutedUICommand("Open Image", "OpenImageCommand", typeof(UserControl));

        public static void OpenImageCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            FormManager.Instance.Strokes.ImagesStrokes = new StrokeCollection();

            FormManager.Instance.NavigationIndex = (int) FormNavigationIndex.Image;
        }

        public static void OpenImageCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (FormManager.Instance.SelectedImage != null);
        }

        #endregion

        #region Create Sketch Command

        public static RoutedUICommand CreateSketchCommand = new RoutedUICommand("Create Sketch", "CreateSketchCommand", typeof(UserControl));

        public static void CreateSketchCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            DrawingVisual drawingVisual = new DrawingVisual();

            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                drawingContext.DrawRectangle(Brushes.White, new Pen(Brushes.Black, 1.0), new Rect(0, 0, ImageManager.ImageWidth, ImageManager.ImageHeight));
            }

            FormManager.Instance.SelectedImage = ImageManager.AddDrawingVisualToSelectedCheck(drawingVisual);

             FormManager.Instance.NavigationIndex = (int)FormNavigationIndex.Image;

             FormManager.Instance.Strokes.ImagesStrokes.Clear();
        }

        public static void CreateSketchCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (FormManager.Instance.SelectedCheck != null);
        }

        #endregion

        #region Undo Image Changes Commands

        public static RoutedUICommand UndoImageCommand = new RoutedUICommand("UndoImageCommand", "UndoImageCommand", typeof(UserControl));

        public static void UndoImageCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            FormManager.Instance.Strokes.ImagesStrokes.Clear();

            FormManager.Instance.NavigationIndex = (int) FormNavigationIndex.Check;
        }

        public static void UndoImageCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (FormManager.Instance.Strokes.ImagesStrokes.Count > 0);
        }

        public static RoutedUICommand SaveImageCommand = new RoutedUICommand("Save Image", "SaveImageCommand", typeof(UserControl));

        public static void SaveImageCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            ImageManager.SaveImage();

            FormManager.Instance.NavigationIndex = (int)FormNavigationIndex.Check;
        }

        public static void SaveImageCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (FormManager.Instance.Strokes.ImagesStrokes.Count > 0);
        }

        #endregion

        #region Remove Image Command

        public static RoutedUICommand RemoveImageCommand = new RoutedUICommand("Remove Image", "RemoveImageCommand", typeof(UserControl));

        public static void RemoveImageCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            RelatedFile selectedImage = FormManager.Instance.SelectedImage;

            FormManager.Instance.SelectedCheck.UserImages.Remove(selectedImage);

            FormManager.Instance.NavigationIndex = (int)FormNavigationIndex.Check;
        }

        public static void RemoveImageCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            RelatedFile selectedImage = FormManager.Instance.SelectedImage;

            if (selectedImage != null)
            {
                e.CanExecute = (FormManager.Instance.SelectedCheck.UserImages.Contains(selectedImage));
            }
        }

        #endregion

        #region Capture Image

        public static RoutedUICommand CaptureImageCommand = new RoutedUICommand("Capture Image", "CaptureImageCommand", typeof(Window));

        public static void CaptureImageCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            CaptureImageWindow captureImageWindow = new CaptureImageWindow();

            captureImageWindow.ShowDialog();
        }

        public static void CaptureImageCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            Check selectedCheck = FormManager.Instance.SelectedCheck;

            if (selectedCheck != null)
            {
                e.CanExecute = !selectedCheck.IsValid;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Ink;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.IO;
using System.Windows;
using Cloudmaster.WCS.Classes;

namespace Cloudmaster.WCS.Model
{
    public class ImageManager
    {
        #region Image Processing

        public static int ImageWidth = 700;

        public static int ImageHeight = 520;

        public static void SaveImage()
        {
            StrokeCollection strokes = FormManager.Instance.Strokes.ImagesStrokes;

            Check selectedCheck = FormManager.Instance.SelectedCheck;

            RelatedFile selectedImage = FormManager.Instance.SelectedImage;

            BitmapImage originalBitmap = new BitmapImage(new Uri(selectedImage.LocalFilename, UriKind.Absolute));

            DrawingVisual drawingVisual = new DrawingVisual();

            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                drawingContext.DrawImage(originalBitmap, new Rect(0, 0, ImageWidth, ImageHeight));

                strokes.Draw(drawingContext);
            }

            AddDrawingVisualToSelectedCheck(drawingVisual);

            selectedCheck.UserImages.Remove(selectedImage);

            FormManager.Instance.Strokes.ImagesStrokes.Clear();
        }

        public static RelatedFile AddDrawingVisualToSelectedCheck(DrawingVisual drawingVisual)
        {
            RelatedFile newImage = AddDrawingVisualToRelatedFile(drawingVisual, ImageWidth, ImageHeight);

            Check selectedCheck = FormManager.Instance.SelectedCheck;

            selectedCheck.UserImages.Add(newImage);

            return newImage;
        }

        public static RelatedFile AddDrawingVisualToRelatedFile(DrawingVisual drawingVisual, int width, int height)
        {
            string filename = WriteDrawingVisualToTemporaryFile(drawingVisual, width, height);

            RelatedFile newImage = new RelatedFile(Guid.NewGuid()) { LocalFilename = filename };

            return newImage;
        }

        private static string WriteDrawingVisualToTemporaryFile(DrawingVisual drawingVisual, int width, int height)
        {
            string result = Path.GetTempFileName();

            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)width, (int)height, 96, 96, PixelFormats.Default);

            renderTargetBitmap.Render(drawingVisual);

            PngBitmapEncoder pngEncoder = new PngBitmapEncoder();

            pngEncoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));

            using (FileStream fileStream = File.Create(result))
            {
                pngEncoder.Save(fileStream);
            }

            return result;
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Ink;

namespace Cloudmaster.WCS.Model
{
    public class Strokes : DependencyObject 
    {
        public Strokes()
        {
            SignatureStrokes = new StrokeCollection();

            SignatureStrokes.StrokesChanged += new StrokeCollectionChangedEventHandler(SignatureStrokes_StrokesChanged);

            ImagesStrokes = new StrokeCollection();
        }

        void SignatureStrokes_StrokesChanged(object sender, StrokeCollectionChangedEventArgs e)
        {
            IsSignatureEntered = (SignatureStrokes.Count > 0);
        }

        public StrokeCollection SignatureStrokes
        {
            get { return (StrokeCollection )GetValue(SignatureStrokesProperty); }
            set { SetValue(SignatureStrokesProperty, value); }
        }

        public static readonly DependencyProperty SignatureStrokesProperty =
            DependencyProperty.Register("SignatureStrokes", typeof(StrokeCollection), typeof(Strokes), new UIPropertyMetadata(null));

        public bool IsSignatureEntered
        {
            get { return (bool)GetValue(IsSignatureEnteredProperty); }
            set { SetValue(IsSignatureEnteredProperty, value); }
        }

        public static readonly DependencyProperty IsSignatureEnteredProperty =
            DependencyProperty.Register("IsSignatureEntered", typeof(bool), typeof(Strokes), new UIPropertyMetadata(false));

        public StrokeCollection ImagesStrokes
        {
            get { return (StrokeCollection)GetValue(ImagesStrokesProperty); }
            set { SetValue(ImagesStrokesProperty, value); }
        }

        public static readonly DependencyProperty ImagesStrokesProperty =
            DependencyProperty.Register("ImagesStrokes", typeof(StrokeCollection), typeof(Strokes), new UIPropertyMetadata(null));

        
    }
}

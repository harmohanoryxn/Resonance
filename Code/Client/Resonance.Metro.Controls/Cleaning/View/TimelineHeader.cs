using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Resonance.Metro.Controls.Cleaning.View
{
    public class TimelineHeader : TimelineControl
    {
        static TimelineHeader()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TimelineHeader), new FrameworkPropertyMetadata(typeof(TimelineHeader)));
        }

        public Brush HoursBrush
        {
            get { return (Brush)GetValue(HoursBrushProperty); }
            set { SetValue(HoursBrushProperty, value); }
        }

        public static readonly DependencyProperty HoursBrushProperty = 
            DependencyProperty.Register("HoursBrush", typeof(Brush), typeof(TimelineHeader), new UIPropertyMetadata(null));

        public int Gap
        {
            get { return (int)GetValue(GapProperty); }
            set { SetValue(GapProperty, value); }
        }

        public static readonly DependencyProperty GapProperty =
            DependencyProperty.Register("Gap", typeof(int), typeof(TimelineHeader), new UIPropertyMetadata(0));

        public Style HourFontStyle
        {
            get { return (Style)GetValue(HourFontStyleProperty); }
            set { SetValue(HourFontStyleProperty, value); }
        }

        public static readonly DependencyProperty HourFontStyleProperty =
            DependencyProperty.Register("HourFontStyle", typeof(Style), typeof(TimelineHeader), new UIPropertyMetadata(0));

        protected Typeface TextTypeFace = new Typeface(new FontFamily("Segoe UI Semibold"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);

        protected override void OnRender(DrawingContext context)
        {
            if (ActualWidth > 0)
                DrawHours(context);
        }

        private void DrawHours(DrawingContext context)
        {
            var oneHourWidth = GetOneHourWidth(ActualWidth, StartHour, EndHour);

            if (Gap < oneHourWidth)
            {
                var width = oneHourWidth - Gap;

                for (int currentHour = StartHour; currentHour < EndHour; currentHour++)
                {
                    if (Background != null)
                    {
                        var x = (currentHour - StartHour)*oneHourWidth;

                        var rect = new Rect(x, 0, width, ActualHeight);

                        context.DrawRectangle(Background, null, rect);

                        var text = string.Format("{0}", currentHour);

                        if ((HoursBrush != null) && (FontSize > 0))
                        {
                            var formattedText = new FormattedText(text, CultureInfo.GetCultureInfo("en-us"),
                                                                  FlowDirection.LeftToRight, TextTypeFace, FontSize,
                                                                  HoursBrush);

                            var textX = x + 5;
                            var textY = (ActualHeight - formattedText.Height)/2;

                            context.DrawText(formattedText, new Point(textX, textY));
                        }
                    }
                }
            }
        }


    }
}

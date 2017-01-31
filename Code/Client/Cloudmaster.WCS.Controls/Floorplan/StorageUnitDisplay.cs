﻿using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Cloudmaster.WCS.Controls
{
    public class StorageUnitDisplay : Border 
    {
        protected static Pen OutlinePen;

        protected static Brush OuterBrush;

        protected static Brush InnerBrush;

        protected static Brush HandleBrush;

        static StorageUnitDisplay()
        {
            OutlinePen = new Pen(Brushes.Black, 1.5);
            OutlinePen.Freeze();

            OuterBrush = Brushes.Sienna;
            OuterBrush.Freeze();

            InnerBrush = Brushes.Peru;
            InnerBrush.Freeze();

            HandleBrush = Brushes.PaleGoldenrod;
            HandleBrush.Freeze();
        }

        protected override void OnRender(DrawingContext dc)
        {
            DrawStorageUnit(dc, Width, Height);
        }

        internal static void DrawStorageUnit(DrawingContext context, double width, double height)
        {
            if ((width > 0) && (height > 0))
            {
                double halfWidth = width / 2;
                double halfHeight = height / 2;

                Rect outerRect = new Rect(-halfWidth, -halfHeight, width - 6, height);
                Rect innerRect = new Rect(-halfWidth + 6, -halfHeight + 6, width - 18, height - 12);
                Rect handleRect = new Rect(halfWidth - 12, -10, 12, 20);

                context.DrawRoundedRectangle(HandleBrush, OutlinePen, handleRect, 4, 4);
                context.DrawRectangle(OuterBrush, OutlinePen, outerRect);
                context.DrawRectangle(InnerBrush, OutlinePen, innerRect);
            }
        }
    }
}
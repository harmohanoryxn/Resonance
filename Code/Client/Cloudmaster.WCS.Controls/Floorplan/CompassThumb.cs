using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls.Primitives;
using System.Windows;

namespace Cloudmaster.WCS.Controls
{
    public class CompassThumb : Thumb
    {
        static CompassThumb()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CompassThumb), new FrameworkPropertyMetadata(typeof(CompassThumb)));
        }
    }
}

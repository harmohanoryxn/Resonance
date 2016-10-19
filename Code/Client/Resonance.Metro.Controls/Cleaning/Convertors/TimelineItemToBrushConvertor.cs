using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;


namespace Resonance.Metro.Controls.Cleaning.Convertors
{
    public class TimelineItemToBrushConvertor : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Count() != 3)
                throw new ArgumentException("Wrong Argument amount");

            if (!(values[0] is FrameworkElement) || values[1].GetType() != typeof(BedStatus) || values[2].GetType() != typeof(bool))
                return null;

            var ui = (FrameworkElement) values[0];
            var status = (BedStatus) values[1];
            var isAvailable = (bool) values[2];

            if (status == BedStatus.RequiresDeepClean)
            {
                return ui.TryFindResource("warningColor") as Brush;
            }
            else if (status == BedStatus.Clean)
            {
                return ui.TryFindResource("inactiveColor") as Brush;
            }
            else if (status == BedStatus.Dirty)
            {
                if (isAvailable)
                {
                    return ui.TryFindResource("activeColor") as Brush;
                }
                else
                {
                    return ui.TryFindResource("pendingColor") as Brush;
                }
            }

            return null;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
    }
}
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;

namespace WCS.Shared.Converters
{
public	class BedStatusConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (targetType != typeof(Brush))
				throw new InvalidOperationException("Wrong return type");

			if (values.Count() != 2)
				throw new ArgumentException("Wrong Argument amount");
			if (!(values[0] is FrameworkElement) || values[1].GetType() != typeof(BedStatus))
				return ((FrameworkElement)values[0]).TryFindResource("CritialWardBrush") as Brush;

			BedStatus status =  (BedStatus)values[1];
		 
			if (status == BedStatus.Clean)
				return ((FrameworkElement)values[0]).TryFindResource("BedCleanStatusBrush") as Brush;
			else if (status == BedStatus.Dirty)
				return ((FrameworkElement)values[0]).TryFindResource("BedDirtyStatusBrush") as Brush;
			else if (status == BedStatus.RequiresDeepClean)
				return ((FrameworkElement)values[0]).TryFindResource("BedDeepCleanRequiredBrush") as Brush;
			//else if (status == BedStatus.NR)
			//    return ((FrameworkElement)values[0]).TryFindResource("BedNotReadyStatusBrush") as Brush;
			//else if (status == BedStatus.OO)
			//    return ((FrameworkElement)values[0]).TryFindResource("BedOutOfOrderStatusBrush") as Brush;
			//else if (status == BedStatus.RB)
			//    return ((FrameworkElement)values[0]).TryFindResource("BedReservedStatusBrush") as Brush;
			//else if (status == BedStatus.RE)
			//    return ((FrameworkElement)values[0]).TryFindResource("BedReadyStatusBrush") as Brush;

			return ((FrameworkElement)values[0]).TryFindResource("CritialWardBrush") as Brush;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}

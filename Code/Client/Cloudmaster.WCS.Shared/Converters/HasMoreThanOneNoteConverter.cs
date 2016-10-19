
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using WCS.Shared.Notes;
using WCS.Shared.Schedule;

namespace WCS.Shared.Converters
{
	class HasMoreThanOneNoteConverter : IValueConverter
	{

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is IList))
				return false;
			try
			{
				var notes = (ObservableCollection<NoteViewModel>)value;
				var filteredNotes = notes.Where(n => n.DateCreated.Date == DateTime.Today.Date).ToList();
				return (filteredNotes.Any() || filteredNotes.Count > 1);
			}
			catch
			{
				return false;
			}

			//if (targetType != typeof(Brush))
			//    throw new InvalidOperationException("Wrong return type");

			//if (values.Count() != 2)
			//    throw new ArgumentException("Wrong Argument amount");
			//if (!(values[0] is FrameworkElement) || (!(values[1] is IList)))
			//    return Brushes.Transparent;

			//try
			//{
			//    var notes = (ObservableCollection<NoteViewModel>)values[1];
			//    return notes.Any() || notes.Count > 1 ? ((FrameworkElement)values[0]).TryFindResource("notesBrush") as Brush : ((FrameworkElement)values[0]).TryFindResource("notesBrush") as Brush;
			//    }
			//catch
			//{
			//    return Brushes.Transparent;
			//}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}


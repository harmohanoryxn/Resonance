
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using WCS.Shared.Notes;
using WCS.Shared.Schedule;

namespace WCS.Shared.Converters
{
	class AmountOfNotesConverter : IValueConverter
	{

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			 	if (!(value is IList))
				return "?";
			try
			{
				var notes = (ObservableCollection<NoteViewModel>)value;
				var filteredNotes = notes.Where(n => n.DateCreated.Date == DateTime.Today.Date).ToList();
				return filteredNotes.Any() ? filteredNotes.Count().ToString() : "+";
			}
			catch
			{
				return "?";
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
	
}


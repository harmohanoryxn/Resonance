using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WCS.Services.DataServices;

namespace WCS.Services.DataServices.Data
{
	public static class NoteExtensions
	{
		public static Note Convert(this WCS.Data.EF.Note wcsNote)
		{
			var note = new Note();
			note.NoteId = wcsNote.noteId;
			if (wcsNote.Order != null)
				note.OrderId = wcsNote.Order.orderId;
			if (wcsNote.Bed != null)
				note.BedId = wcsNote.Bed.bedId;
			note.Source = wcsNote.source;
			note.NoteMessage = wcsNote.notes;
			note.DateCreated = wcsNote.dateCreated.HasValue ? wcsNote.dateCreated.Value : default(DateTime);
			return note;
		}
	}
}

using System;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using GalaSoft.MvvmLight;
using WCS.Core;
using WCS.Core.Composition;

namespace WCS.Shared.Notes
{
	public class NoteViewModel : ViewModelBase, IIdentifable
	{
		private string _currentDevice;

		public NoteViewModel(Note note)
		{
			Note = note;
			_currentDevice = new DefaultDeviceIdentity().DeviceName;
		}

		public Note Note { get; private set; }

		public int NoteId
		{
			get { return Note.NoteId; }
		}
		public string Message
		{
			get { return Note.NoteMessage; }
		}
		public string Source
		{
			get { return Note.Source; }
		}
		public string Time
		{
			get { return Note.DateCreated.ToNoteTimeFormat(); }
		}
		public bool IsNativeNote
		{
			get { return Note.Source == _currentDevice; }
		}
		public DateTime DateCreated
		{
			get { return Note.DateCreated; }
		}

		internal void Synchronise(Note note)
		{
			Note = note;
			this.DoRaisePropertyChanged(() => Message, RaisePropertyChanged);
			this.DoRaisePropertyChanged(() => Source, RaisePropertyChanged);
			this.DoRaisePropertyChanged(() => Time, RaisePropertyChanged);
		}

		public int Id
		{
			get { return Note.NoteId; }
		}

		public int GetFingerprint()
		{
			return Note.GetFingerprint();
		}
	}
}

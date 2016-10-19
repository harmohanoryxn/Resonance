using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using GalaSoft.MvvmLight.Command;
using WCS.Shared.Controls;

namespace WCS.Shared.Notes
{
	public class InLineNotesViewModel : ViewModelBase
	{
		private ObservableCollection<NoteViewModel> _notes;
		private NoteViewModel _topNote;

		private Action _showNotesDelegate;

		public InLineNotesViewModel(IEnumerable<Note> notes, Action showNotesDelegate)
		{
			_showNotesDelegate = showNotesDelegate;
			_notes = new ObservableCollection<NoteViewModel>(notes.OrderBy(n => n.DateCreated).Reverse().Take(1).Select(n => new NoteViewModel(n)));
			_topNote = new ObservableCollection<NoteViewModel>(notes.OrderBy(n => n.DateCreated).Reverse().Take(1).Select(n => new NoteViewModel(n))).FirstOrDefault();
		}

		public ObservableCollection<NoteViewModel> Notes
		{
			get { return _notes; }
			set
			{
				_notes = value;
				this.DoRaisePropertyChanged(() => Notes, RaisePropertyChanged);
			}
		}
		public NoteViewModel TopNote
		{
			get { return _topNote; }
			set
			{
				_topNote = value;
				this.DoRaisePropertyChanged(() => TopNote, RaisePropertyChanged);
			}
		}

		public bool HasNotes
		{
			//get { return Notes.Any(); }
			get { return _topNote!=null; }
		}

		public RelayCommand ShowNotesCommand
		{
			get { return new RelayCommand(DoShowNotes); }
		}
		private void DoShowNotes()
		{
			if (_showNotesDelegate != null)
				_showNotesDelegate.Invoke();
		}

		internal void Synchronise(Order order)
		{
            if (Application.Current != null)
            {
                Application.Current.Dispatcher.InvokeIfRequired((() =>
                                                                     {
                                                                         _notes.Clear();

                                                                         Notes =
                                                                             new ObservableCollection<NoteViewModel>(
                                                                                 order.Notes.OrderBy(n => n.DateCreated)
                                                                                     .Reverse().Take(1).Select(
                                                                                         n => new NoteViewModel(n)));
                                                                         TopNote =
                                                                             new ObservableCollection<NoteViewModel>(
                                                                                 order.Notes.OrderBy(n => n.DateCreated)
                                                                                     .Reverse().Take(1).Select(
                                                                                         n => new NoteViewModel(n))).
                                                                                 FirstOrDefault();
                                                                     }));
            }

		    this.DoRaisePropertyChanged(() => HasNotes, RaisePropertyChanged);
		}
	}
}

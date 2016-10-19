using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Cloudmaster.WCS.Controls;
using Cloudmaster.WCS.DataServicesInvoker;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using WCS.Core;
using WCS.Shared.Controls;
using WCS.Shared.Schedule;

namespace WCS.Shared.Notes
{
	/// <summary>
	/// This is another diabolical implementation. It was fine until it;s purpose doubled up and the generic approach didnt quite fit when calling the server
	/// </summary>
	public class ConversationNotesViewModel : ViewModelBase
	{
		public event Action<Order> OrderUpdateAvailable;
		public event Action<Bed> BedUpdateAvailable;
		
		private string _noteText;
		private int _hostObjectId;
		private bool _showNotes;
		private bool _isInFocus;

		private ObservableCollection<NoteViewModel> _notes;

		public ConversationNotesViewModel(Order order)
		{
			_hostObjectId = order.OrderId;
			Notes = new ObservableCollection<NoteViewModel>();
			Synchronise(order);
		}
		public ConversationNotesViewModel(Bed bed)
		{
			_hostObjectId = bed.BedId;
			Notes = new ObservableCollection<NoteViewModel>();
			Synchronise(bed);
		}


		public bool ShowNotes
		{
			get { return _showNotes; }
			set
			{
				_showNotes = value;
				this.DoRaisePropertyChanged(() => ShowNotes, RaisePropertyChanged);
			}
		}

		public string NoteText
		{
			get { return _noteText; }
			set
			{
				_noteText = value;
				this.DoRaisePropertyChanged(() => NoteText, RaisePropertyChanged);
			}
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

		public RelayCommand DismissCommand
		{
			get { return new RelayCommand(DoDismiss); }
		}

		public RelayCommand ToggleNotesCommand
		{
			get { return new RelayCommand(DoToggleNotes); }
		} 

		public RelayCommand AddNewNoteCommand
		{
			get { return new RelayCommand(DoAddNewNote); }
		}

		#region Focus Hack

		/// <summary>
		/// This property is used to bind to an attached property FocusExtension.IsFocused so that we can MVVM focus
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is in focus; otherwise, <c>false</c>.
		/// </value>
		public bool IsInFocus
		{
			get { return _isInFocus; }
			set
			{
				_isInFocus = value;
				this.DoRaisePropertyChanged(() => IsInFocus, RaisePropertyChanged);
			}
		}

		/// <summary>
		/// Used to force focus into the textbox. It has to be done on another task because something else retakes focus. I dont know what that control 
		/// is, but otherwise, this just won't work
		/// </summary>
		internal void ForceFocusToTextbox()
		{
			Task.Factory.StartNew(() =>
									{
										IsInFocus = true;
									}).LogExceptionIfThrownAndIgnore();
		} 
		#endregion

		private void DoToggleNotes()
		{
			Sound.ButtonClick.Play();
			
			ShowNotes = !ShowNotes;
		}
		private void DoDismiss()
		{
			ShowNotes = false;
		}

		private void DoAddNewNote()
		{
			if (OrderUpdateAvailable != null)
				DoAddNewOrderNote();
			if (BedUpdateAvailable != null)
				DoAddNewBedNote();
		}

		private void DoAddNewOrderNote()
		{
			var noteText = NoteText;
			NoteText = "";
			var invoker = WcsViewModel.MefContainer.GetExportedValue<IWcsAsyncInvoker>();
			if (invoker != null)
				invoker.AddOrderNoteAsync(_hostObjectId, noteText, SynchroniseWithUpdate);
		}

		private void DoAddNewBedNote()
		{
			var noteText = NoteText;
			NoteText = "";
			var invoker = WcsViewModel.MefContainer.GetExportedValue<IWcsAsyncInvoker>();
			if (invoker != null)
				invoker.AddBedNoteAsync(_hostObjectId, noteText, SynchroniseWithUpdate);
		}

		internal void Synchronise(Order order)
		{
			MergeAlgorithm<Note, NoteViewModel>.Merge(order.Notes,
				_notes,
				(toAdd) => DoAddItem(toAdd),
				(toRemove) => DoRemoveItem(toRemove),
				(old, @new) => DoSynchronise(old, @new));

			this.DoRaisePropertyChanged(() => Notes, RaisePropertyChanged);
		}

		internal void Synchronise(Bed bed)
		{
			// Notes
			MergeAlgorithm<Note, NoteViewModel>.Merge(bed.Notes,
				_notes,
				(toAdd) => DoAddItem(toAdd),
				(toRemove) => DoRemoveItem(toRemove),
				(old, @new) => DoSynchronise(old, @new));

			this.DoRaisePropertyChanged(() => Notes, RaisePropertyChanged);
		}

		private void DoSynchronise(NoteViewModel nvm, Note note)
		{
			nvm.Synchronise(note);
		}

		private void DoAddItem(Note itemToAdd)
		{
            if (Application.Current != null)
            {
                Application.Current.Dispatcher.InvokeIfRequired((() =>
                                                                     {
                                                                         Notes.Add(new NoteViewModel(itemToAdd));
                                                                     }));
            }
		}
	
		private void DoRemoveItem(NoteViewModel itemToRemove)
		{
            if (Application.Current != null)
            {
                Application.Current.Dispatcher.InvokeIfRequired((() =>
                                                                     {
                                                                         Notes.Remove(itemToRemove);
                                                                     }));
            }
		}

		/// <summary>
		/// Callback from when the user updates the order. Need to signal anybody who's interested in this
		/// </summary>
		/// <param name="order">The order.</param>
		private void SynchroniseWithUpdate(Order order)
		{
			var ora = OrderUpdateAvailable;
			if (ora != null)
			{
				ora.Invoke(order);
			}
		}

		/// <summary>
		/// Callback from when the user updates the bed. Need to signal anybody who's interested in this
		/// </summary>
		/// <param name="bed">The bed.</param>
		private void SynchroniseWithUpdate(Bed bed)
		{
			var bua = BedUpdateAvailable;
			if (bua != null)
			{
				bua.Invoke(bed);
			}
		}
	}
}

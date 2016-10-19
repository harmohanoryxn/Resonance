using System;
using System.Threading.Tasks;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using GalaSoft.MvvmLight.Command;
using WCS.Core;
using WCS.Shared.Schedule;

namespace WCS.Shared.Admissions.Schedule
{
	/// <summary>
	/// Handles all the manipulation and updating of an bed that appears in the schedule
	/// </summary>
	public class AdmissionBedViewModel : UpdatingViewModel, ISynchroniseable<AdmissionBedViewModel>
	{
		public event Action<AdmissionBedViewModel, ScreenSelectionType?> TrySelect;
		public event Action ShouldRecalculateStatistics;

		private BedDischargeData _bed;
		private Action<AdmissionBedViewModel> _popupCallback;
		private Action _dismissPopupCallback;
		private Action _toggleInfoCardCallback;
		private Action _toggleNotesCallback;
		private Action _toggleHistoryCallback;

		private bool _isUpdating;
		private bool _isHighlighted;
		private ScreenSelectionType? _selectionType;
		
		public AdmissionBedViewModel(BedDischargeData bed, Action<AdmissionBedViewModel> popupCallback, Action dismissPopupCallback, Action toggleNotesCallback, Action toggleHistoryCallback, Action toggleInfoCardCallback)
		{
			_popupCallback = popupCallback;
			_dismissPopupCallback = dismissPopupCallback;
			_toggleNotesCallback = toggleNotesCallback;
			_toggleHistoryCallback = toggleHistoryCallback;
			_toggleInfoCardCallback = toggleInfoCardCallback;

			_bed = bed;

			SelectionType = null;
		}
		 
		/// <summary>
		/// Synchronises the bed as well as signalling that the labels need updating 
		/// </summary>
		/// <param name="patient">The bed</param>
		public void SynchroniseWithUpdate(BedDischargeData patient)
		{
			MarkUpdateAsArrived();

			Synchronise(patient);

			var srs = ShouldRecalculateStatistics;
			if (srs != null)
				srs.Invoke();
		}

		/// <summary>
		/// Set and resets all of the view models properties with a new or modified bed
		/// </summary>
		/// <param name="bed">The bed.</param>
		public void Synchronise(BedDischargeData bed)
		{
			if (_disposed)
				return;

			if (bed.BedId != _bed.BedId)
				return;

			if (CheckIfUpdateIsPending) return;

			if (bed.GetFingerprint() == _bed.GetFingerprint())
				return;

			IsUpdating = true;

			_bed = bed;

		 		IsUpdating = false;

			this.DoRaisePropertyChanged(() => Id, RaisePropertyChanged);
			this.DoRaisePropertyChanged(() => LocationCode, RaisePropertyChanged);
			this.DoRaisePropertyChanged(() => Room, RaisePropertyChanged);
			this.DoRaisePropertyChanged(() => Bed, RaisePropertyChanged);
		}


		/// <summary>
		/// Set and resets all of the view models properties with a new or modified bed
		/// </summary>
		/// <param name="bed">The bed.</param>
		public void Synchronise(AdmissionBedViewModel bed)
		{
			if (_disposed)
				return;

			if (bed.Id != _bed.BedId)
				return;

			Task.Factory.StartNew(() => Synchronise(bed._bed)).LogExceptionIfThrownAndIgnore();
		}

		public string LocationCode
		{
			get { return _bed.Location.Name; }
		}

		public string Room
		{
			get { return _bed.Location.Room; }
		}

		public string Bed
		{
			get { return _bed.Location.Bed; }
		}

		 
		public int Id
		{
			get { return _bed.BedId; }
		}

		  
		public bool IsHighlighted
		{
			get { return _isHighlighted; }
			set
			{
				_isHighlighted = value;
				this.DoRaisePropertyChanged(() => IsHighlighted, RaisePropertyChanged);
			}
		} 
 
		public ScreenSelectionType? SelectionType
		{
			get { return _selectionType; }
			set
			{
				_selectionType = value;
				this.DoRaisePropertyChanged(() => SelectionType, RaisePropertyChanged);

				IsHighlighted = _selectionType.HasValue && _selectionType.Value == ScreenSelectionType.Selected;
			}
		}

		public RelayCommand ToggleNotesCommand
		{
			get { return new RelayCommand(DoToggleNotes); }
		}

		public RelayCommand ShowHistoryCommand
		{
			get { return new RelayCommand(DoShowHistory); }
		}

		public RelayCommand ShowInfoCommand
		{
			get { return new RelayCommand(DoShowInfo); }
		}

		#region IDispose

		private volatile bool _disposed = false;
		private IDisposable _requestUpdateEstimatedDischargeTimeSource;

		protected override void Dispose(bool disposing)
		{
			if (!this._disposed)
			{
				_disposed = true;

				if (_requestUpdateEstimatedDischargeTimeSource != null)
					_requestUpdateEstimatedDischargeTimeSource.Dispose();


				//if (_timelineCoordinator != null)
				//{
				//    _timelineCoordinator.Dispose();
				//    _timelineCoordinator = null;
				//}
				//if (_conversationNotesViewModel != null)
				//{
				//    _conversationNotesViewModel.BedUpdateAvailable -= SynchroniseWithUpdate;
				//    _conversationNotesViewModel.Dispose();
				//    _conversationNotesViewModel = null;
				//}

				base.Dispose(disposing);
			}
		}
		#endregion

		public int GetFingerprint()
		{
			return _bed.GetFingerprint();
		}

		public bool IsUpdating
		{
			get { return _isUpdating; }
			set
			{
				_isUpdating = value;
				this.DoRaisePropertyChanged(() => IsUpdating, RaisePropertyChanged);
			}
		}

		public RelayCommand PopupBedCommand
		{
			get { return new RelayCommand(DoPopupBed); }
		}

		public RelayCommand TrySelectCommand
		{
			get { return new RelayCommand(DoTrySelect); }
		}

		private void DoTrySelect()
		{
			var ts = TrySelect;
			if (ts != null)
				ts.Invoke(this, SelectionType);
		}

		private void DoPopupBed()
		{
			if (_popupCallback != null)
				_popupCallback.Invoke(this);

			DoTrySelect();
		}
	
		private void DoToggleNotes()
		{
			if (_toggleNotesCallback != null)
				_toggleNotesCallback.Invoke();
		}

		private void DoShowHistory()
		{
			if (_toggleHistoryCallback != null)
				_toggleHistoryCallback.Invoke();
		}

		private void DoShowInfo()
		{
			if (_toggleInfoCardCallback != null)
				_toggleInfoCardCallback.Invoke();
		}

		internal void HandleMinuteTimerTick()
		{
		}
 

		public void DoDismissPopup()
		{
			if (_dismissPopupCallback != null)
				_dismissPopupCallback.Invoke();
		}

		internal void ToggleBedSelection(AdmissionBedViewModel bed)
		{
			SelectionType = SelectionType == ScreenSelectionType.DeSelected ? ScreenSelectionType.Selected : ScreenSelectionType.DeSelected;
		}

		internal void ClearBedsSelectionType()
		{
			SelectionType = null;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using GalaSoft.MvvmLight.Command;
using WCS.Core;
using WCS.Shared.Schedule;

namespace WCS.Shared.Admissions.Schedule
{
	/// <summary>
	/// Handles all the manipulation and updating of an patient that appears in the schedule
	/// </summary>
	public class WaitingPatientViewModel : UpdatingViewModel, ISynchroniseable<WaitingPatientViewModel>
	{
		public event Action<WaitingPatientViewModel, ScreenSelectionType?> TrySelect;
		public event Action ShouldRecalculateStatistics;

        private Admission _admission;
		private Action<WaitingPatientViewModel> _popupCallback;
		private Action _dismissPopupCallback;
		private Action _toggleInfoCardCallback;

		private Action _toggleNotesCallback;
		private Action _toggleHistoryCallback;

		private bool _isUpdating;
		private bool _isHighlighted;
		private LocationClockViewModel _locationTimer;
		private ScreenSelectionType? _selectionType;

        public WaitingPatientViewModel(Admission admission, Action<WaitingPatientViewModel> popupCallback, Action dismissPopupCallback, Action toggleNotesCallback, Action toggleHistoryCallback, Action toggleInfoCardCallback)
		{
			_popupCallback = popupCallback;
			_dismissPopupCallback = dismissPopupCallback;
			_toggleNotesCallback = toggleNotesCallback;
			_toggleHistoryCallback = toggleHistoryCallback;
			_toggleInfoCardCallback = toggleInfoCardCallback;

            _admission = admission;

			SelectionType = null;

            LocationTimer = new LocationClockViewModel(admission.Location.Name);
		}
		 

        public void SynchroniseWithUpdate(Admission admission)
		{
			MarkUpdateAsArrived();

            Synchronise(admission);

			var srs = ShouldRecalculateStatistics;
			if (srs != null)
				srs.Invoke();
		}


        public void Synchronise(Admission admission)
		{
			if (_disposed)
				return;

            if (admission.Patient.PatientId != _admission.Patient.PatientId)
				return;

			if (CheckIfUpdateIsPending) return;

            if (admission.GetFingerprint() == _admission.GetFingerprint())
				return;

			IsUpdating = true;

            _admission = admission;

		 		IsUpdating = false;

			this.DoRaisePropertyChanged(() => Id, RaisePropertyChanged);
			this.DoRaisePropertyChanged(() => PatientName, RaisePropertyChanged);
			this.DoRaisePropertyChanged(() => AdmittingDoctor, RaisePropertyChanged);
			this.DoRaisePropertyChanged(() => IsMale, RaisePropertyChanged);
			
		}

		/// <summary>
		/// Updates the order with RFID detections 
		/// </summary>
		/// <param name="detections">The order</param>
		public void Synchronise(IList<Detection> detections)
		{
			if (_disposed)
				return;
 
			if (detections.Any())
			{
				LocationTimer.SetLatestDetection(detections.OrderBy(d => d.Timestamp).Last());
			}
		}

		public void Synchronise(WaitingPatientViewModel viewModel)
		{
			if (_disposed)
				return;

            if (viewModel.Id != _admission.Patient.PatientId)
				return;

            Task.Factory.StartNew(() => Synchronise(viewModel._admission)).LogExceptionIfThrownAndIgnore();
		}
 
		public int? PatientId
		{
			get { return null; }
		}

		 
		public int Id
		{
			get { return _admission.Id; }
		}

		public string PatientName
		{
            get { return string.Format("{0}, {1}", _admission.Patient.FamilyName, _admission.Patient.GivenName); }
		}

		public string AdmittingDoctor
		{
			get { return "?"; } 
		}

		public bool IsMale
		{
            get { return _admission.Patient.Sex == PatientSex.Male; }
		}

		public bool IsHighlighted
		{
			get { return _isHighlighted; }
			set
			{
				_isHighlighted = value;
				this.DoRaisePropertyChanged(() => IsMale, RaisePropertyChanged);
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

		public LocationClockViewModel LocationTimer
		{
			get { return _locationTimer; }
			set
			{
				_locationTimer = value;
				this.DoRaisePropertyChanged(() => LocationTimer, RaisePropertyChanged);
			}
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
			return _admission.GetFingerprint();
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

		internal void ToggleBedSelection(WaitingPatientViewModel bed)
		{
			SelectionType = SelectionType == ScreenSelectionType.DeSelected ? ScreenSelectionType.Selected : ScreenSelectionType.DeSelected;
		}

		internal void ClearBedsSelectionType()
		{
			SelectionType = null;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using WCS.Core;
using WCS.Shared.Alerts;
using WCS.Shared.Browser;
using WCS.Shared.Ward.Schedule;

namespace WCS.Shared.Schedule
{
	public abstract class WcsScheduleViewModel<T> : ViewModelBase, IAlertable where T : class, ISynchroniseable<T>
	{
		public event Action<DateTime> RequestOrdersFromDate;

		public event Action<T> StartingManipulation;
		public event Action<T> EndedManipulation;

		private DateTime _currentDate;
		private DateTime? _lastSynchronised;
		private SynchronousStatus _synchronisedStatus;
		private bool _showActionBar;
		private bool _showCardPanel;
		private bool _showRfidPanel;
		private bool _showNotesPanel;
		private bool _showHistoryPanel;
		private bool _isRfidEnabled;

		private Timer _minuteTimer;

		protected WcsScheduleViewModel()
		{
			_currentDate = DateTime.Today;

			_minuteTimer = new Timer(HandleMinuteTimerTick);
			_minuteTimer.Change(60000, 60000);

			_isRfidEnabled = false;

			ShowCardPanel = false;
			ShowActionBar = false;
			ShowRfidPanel = false;
			ShowNotesPanel = false;
			ShowHistoryPanel = false;
		}

		public virtual IEnumerable<string> GetAlertMessages()
		{
			return Enumerable.Empty<string>();
		}

		#region IDispose

		private bool _disposed = false;

		public new void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this._disposed)
			{
				_disposed = true;

				if (_minuteTimer != null)
				{
					_minuteTimer.Dispose();
					_minuteTimer = null;
				}

			}
		}
		#endregion

		public RelayCommand LoadOrdersForTodayCommand
		{
			get { return new RelayCommand(DoLoadOrdersForToday); }
		}
		public RelayCommand LoadOrdersForYesterdayCommand
		{
			get { return new RelayCommand(DoLoadOrdersForYesterday); }
		}
		public RelayCommand ManualLockCommand
		{
			get { return new RelayCommand(DoManualLock); }
		}
		public RelayCommand CancelSelectionCommand
		{
			get { return new RelayCommand(HandleCancelSelection); }
		}

		public DateTime CurrentDate
		{
			get { return _currentDate; }
			set
			{
				_currentDate = value;
				this.DoRaisePropertyChanged(() => CurrentDate, RaisePropertyChanged);
				this.DoRaisePropertyChanged(() => IsCurrentDateToday, RaisePropertyChanged);
			}
		}

		public bool IsCurrentDateToday
		{
			get { return _currentDate == DateTime.Today; }
		}

		public WcsViewModel Main { get; set; }
		
		public bool ShowCardPanel
		{
			get { return _showCardPanel; }
			set
			{
				_showCardPanel = value;
				this.DoRaisePropertyChanged(() => ShowCardPanel, RaisePropertyChanged);
			}
		}
		 
		public bool ShowActionBar
		{
			get { return _showActionBar; }
			set
			{
				_showActionBar = value;
				this.DoRaisePropertyChanged(() => ShowActionBar, RaisePropertyChanged);
			}
		}

		public bool ShowRfidPanel
		{
			get { return _showRfidPanel; }
			set
			{
				_showRfidPanel = value;
				this.DoRaisePropertyChanged(() => ShowRfidPanel, RaisePropertyChanged);
			}
		}
		public bool ShowNotesPanel
		{
			get { return _showNotesPanel; }
			set
			{
				_showNotesPanel = value;
				this.DoRaisePropertyChanged(() => ShowNotesPanel, RaisePropertyChanged);
			}
		}
		public bool ShowHistoryPanel
		{
			get { return _showHistoryPanel; }
			set
			{
				_showHistoryPanel = value;
				this.DoRaisePropertyChanged(() => ShowHistoryPanel, RaisePropertyChanged);
			}
		}

		private void DoLoadOrdersForToday()
		{
			CurrentDate = DateTime.Today;

			RaiseDateRequest();
		}

		private void DoManualLock()
		{
			Main.SecurityViewModel.IsLocked = true;
		}

		private void DoLoadOrdersForYesterday()
		{
			CurrentDate = DateTime.Today.AddDays(-1);

			RaiseDateRequest();

			// We want the schedule to automatically revert to today after a minute
			var revertResetEvent = new ManualResetEventSlim();
			System.Threading.Tasks.Task.Factory.StartNew(() =>
					{
						revertResetEvent.Wait(60000);

						if (!IsCurrentDateToday)
							DoLoadOrdersForToday();
					}).LogExceptionIfThrownAndIgnore();
		}

		protected virtual void HandleChangeToItemSelection(T obj, ScreenSelectionType? selectionType)
		{
		}


		public DateTime? LastSynchronised
		{
			get { return _lastSynchronised; }
			set
			{
				_lastSynchronised = value;
				this.DoRaisePropertyChanged(() => LastSynchronised, RaisePropertyChanged);

				TimeSpan twoMinutes = TimeSpan.FromMinutes(2);
				TimeSpan oneHour = TimeSpan.FromHours(1);
				TimeSpan oneDay = TimeSpan.FromDays(1);

				DateTime now = DateTime.Now;


				if (!_lastSynchronised.HasValue)
				{
					SynchronisedStatus = SynchronousStatus.Loading;
				}
				else
				{
					if (now.Subtract(_lastSynchronised.Value) < twoMinutes)
					{
						SynchronisedStatus = SynchronousStatus.UpToDate;
					}
					else if (now.Subtract(_lastSynchronised.Value) < oneHour)
					{
						SynchronisedStatus = SynchronousStatus.DelayedUpTo1Hour;
					}
					else if (now.Subtract(_lastSynchronised.Value) < oneDay)
					{
						SynchronisedStatus = SynchronousStatus.DelayedUpTo1Day;
					}
					else
					{
						SynchronisedStatus = SynchronousStatus.DelayedMoreThanDay;
					}
				}
			}
		}

		public SynchronousStatus SynchronisedStatus
		{
			get { return _synchronisedStatus; }
			set
			{
				_synchronisedStatus = value;
				this.DoRaisePropertyChanged(() => SynchronisedStatus, RaisePropertyChanged);
			}
		}
		public virtual void HandleLockedEvent()
		{
			Tombstone();
		}

		public virtual void Tombstone()
		{ }
		
		protected virtual void UpdateStatistics()
		{
		}
		protected virtual void ClearAll()
		{
		}
		protected virtual void HandleMinuteTimerTick()
		{
		}
		protected virtual void HandleCancelSelection()
		{
		}

		private void RaiseDateRequest()
		{
			var rofd = RequestOrdersFromDate;
			if (rofd != null)
				RequestOrdersFromDate(CurrentDate);
		}

		/// <summary>
		/// Recalculate completed and patient type statistics statistics
		/// </summary>
		protected void HandleRequestToRecalculateStatistics()
		{
			UpdateStatistics();
		}

		protected void HandleStartingManipulation(T order)
		{
			var sm = StartingManipulation;
			if (sm != null)
				sm(order);
		}

		protected void HandleEndedManipulation(T order)
		{
			var em = EndedManipulation;
			if (em != null)
				em(order);
		}
 
		/// <summary>
		/// We need to alert the UI every minute as the order might have slipped into 'requiring acknowledging'. This is the top level 
		/// object and propagates the tick downwards through the object graph
		/// </summary>
		/// <param name="sender">The sender.</param>
		void HandleMinuteTimerTick(object sender)
		{
			HandleMinuteTimerTick();
		}
		 

		#region Default Location


		private LocationSummaryViewModel _defaultAlternativeLocation;
	
		public LocationSummaryViewModel DefaultLocation
		{
			get { return _defaultAlternativeLocation; }
			set
			{
				_defaultAlternativeLocation = value;
				this.DoRaisePropertyChanged(() => DefaultLocation, RaisePropertyChanged);
			}
		}

		public bool IsRfidEnabled
		{
			get { return   _isRfidEnabled; }
			set
			{
				if (_isRfidEnabled != value)
				{
					_isRfidEnabled = value;
					this.DoRaisePropertyChanged(() => IsRfidEnabled, RaisePropertyChanged);
				}
			}
		}

		public RelayCommand HidePanelsAndDeselectCommand
		{
			get { return new RelayCommand(DoHidePanelsAndDeselect); }
		}

		public RelayCommand HidePanelsAndReselectCommand
		{
			get { return new RelayCommand(DoHidePanelsAndReselect); }
		}

		public void HidePanels()
		{
			ShowCardPanel = false;
			ShowActionBar = false;
			ShowRfidPanel = false;
			ShowNotesPanel = false;
			ShowHistoryPanel = false;
		}

		public void ToggleRfidView(PatientViewModel patient)
		{
			if (patient == null)
				throw new ArgumentException("Missing", "Patient");

			ShowRfidPanel = !ShowRfidPanel;
			ShowCardPanel = false;
			ShowActionBar = false;
			ShowNotesPanel = false;

			Sound.ButtonClick.Play();
		}

		public void ToggleNotesVisibility()
		{
			ShowNotesPanel = !ShowNotesPanel;
			ShowCardPanel = false;
			ShowActionBar = false;
			ShowRfidPanel = false;
			ShowHistoryPanel = false;

			Sound.ButtonClick.Play();
		}

		public void ToggleHistoryVisibility()
		{
			ShowHistoryPanel = !ShowHistoryPanel;
			ShowCardPanel = false;
			ShowActionBar = false;
			ShowRfidPanel = false;
			ShowNotesPanel = false;

			Sound.ButtonClick.Play();
		}

		public void ToggleCardInfoVisibility()
		{
			ShowCardPanel = !ShowCardPanel;
			ShowHistoryPanel = false;
			ShowActionBar = false;
			ShowRfidPanel = false;
			ShowNotesPanel = false;

			Sound.ButtonClick.Play();
		}

		public LocationSummary GetDefaultLocation()
		{
			if (DefaultLocation == null)
				return null;

			return DefaultLocation.DataItem;
		}

		#endregion



		protected void DoHidePanelsAndDeselect()
		{
			HidePanels();

			HandleCancelSelection();
		}
		protected void DoHidePanelsAndReselect()
		{
			ShowCardPanel = false;
			ShowActionBar = true;
			ShowRfidPanel = false;
			ShowNotesPanel = false;
			ShowHistoryPanel = false;
		} 
		
	}
}

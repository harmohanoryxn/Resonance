using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition.Hosting;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using WCS.Core;
using WCS.Core.Composition;
using WCS.Shared.Admissions.Schedule;
using WCS.Shared.Alerts;
using WCS.Shared.Beds;
using WCS.Shared.Browser;
using WCS.Shared.Cleaning.Schedule;
using WCS.Shared.Controls;
using WCS.Shared.Discharge.Schedule;
using WCS.Shared.Location;
using WCS.Shared.Orders;
using WCS.Shared.Security;
using WCS.Shared.Ward.Schedule;
using WCS.Core.Instrumentation;

namespace WCS.Shared.Schedule
{
	/// <summary>
	/// Acts as a base to the Department and Ward DataContexts so that they can be handled homogenously
	/// </summary>
	public class WcsViewModel : ViewModelBase
	{
		private SecurityViewModel _securityViewModel;
		private AlertViewModel _alertViewModel;
		private ErrorBarViewModel _errorBarViewModel;
		private ViewModelBase _popup;
		private DateTime _lastUserActivity;
		private Timer _activityimer;
		private CollectionViewSource _collectionSource;
			private bool _showNavigationBar;

		public IWcsExceptionHandler ExceptionHandler { get; set; }

		private ObservableCollection<DeviceConfigurationInstance> _configurations;

		static WcsViewModel()
		{
			MefContainer = new CompositionContainer(WcsMef.Container);
		}

		public WcsViewModel(bool isLocked, string version, string clientName, string serverName)
		{
			SecurityViewModel = new LockViewModel(isLocked);
			SecurityViewModel.Initialize();
			SecurityViewModel.Locked += SecurityViewModel_Locked;
			SecurityViewModel.Unlocked += SecurityViewModel_Unlocked;

			AlertViewModel = new AlertViewModel();
			ErrorBarViewModel = new ErrorBarViewModel();

			Configurations = new ObservableCollection<DeviceConfigurationInstance>();

			Application.Current.Dispatcher.InvokeIfRequired((() =>
			{
				ConfigurationSource = new CollectionViewSource();
				ConfigurationSource.Source = Configurations;
			}));

			ExceptionHandler = WcsViewModel.MefContainer.GetExportedValue<IWcsExceptionHandler>();
			if (ExceptionHandler != null)
			{
				ExceptionHandler.ExceptionArrived += HandleException;
				ExceptionHandler.ServerStatusUpdate += HandleException;
			}

			ApplicationVersion = version;
			ClientName = clientName;
			ServerName = serverName;

			_activityimer = new Timer(ActivityTimer_Tick);
		}


		public static CompositionContainer MefContainer { get; set; }

		public string ApplicationVersion { get; set; }

		public string ServerName { get; set; }

		public string ClientName { get; set; }

		public SecurityViewModel SecurityViewModel
		{
			get { return _securityViewModel; }
			set
			{
				_securityViewModel = value;
				this.DoRaisePropertyChanged(() => SecurityViewModel, RaisePropertyChanged);
			}
		}

		public AlertViewModel AlertViewModel
		{
			get { return _alertViewModel; }
			set
			{
				_alertViewModel = value;
				this.DoRaisePropertyChanged(() => AlertViewModel, RaisePropertyChanged);
			}
		}

		public ErrorBarViewModel ErrorBarViewModel
		{
			get { return _errorBarViewModel; }
			set
			{
				_errorBarViewModel = value;
				this.DoRaisePropertyChanged(() => ErrorBarViewModel, RaisePropertyChanged);
			}
		}

		public ObservableCollection<DeviceConfigurationInstance> Configurations
		{
			get { return _configurations; }
			set
			{
				_configurations = value;
				this.DoRaisePropertyChanged(() => Configurations, RaisePropertyChanged);
			}
		}


		public CollectionViewSource ConfigurationSource
		{
			get { return _collectionSource; }
			set
			{
				_collectionSource = value;
				this.DoRaisePropertyChanged(() => ConfigurationSource, RaisePropertyChanged);
			}
		}

		public RelayCommand ToggleNavigationBarVisibilityCommand
		{
			get { return new RelayCommand(DoToggleNavigationBarVisibility); }
		}

		public bool ShowNavigationBar
		{
			get { return _showNavigationBar; }
			set
			{
				_showNavigationBar = value;
				this.DoRaisePropertyChanged(() => ShowNavigationBar, RaisePropertyChanged);
			}
		}

		/// <summary>
		/// Forces the navigation bar disappear. THis unfortunately has to be called because of a bug in WPF. Our bindings break around the visibility of the 
		/// navigation bar when 2 screens of the same type are selected consequtively. Something to do about the runtime resuing controls between templates
		/// </summary>
		public void ForceNavigationBarDisappear()
		{
			ShowNavigationBar = true;
			ShowNavigationBar = false;
		}
		 
		/// <summary>
		/// Opens an order into it's own popout panel
		/// </summary>
		/// <param name="order">The order</param>
		public void Popup(OrderViewModel order)
		{
			if (order == null)
				throw new ArgumentException("Missing", "Order");

			PopupViewModel = new OrderCardViewModel(order);

			Sound.ButtonClick.Play();
		}

		/// <summary>
		/// Opens an bed into a popout
		/// </summary>
		/// <param name="bed">The bed</param>
		public void Popup(BedViewModel bed)
		{
			if (bed == null)
				throw new ArgumentException("Missing", "Bed");

			PopupViewModel = new BedCardViewModel(bed);
		}

		/// <summary>
		/// Opens an discharge into a popout
		/// </summary>
		/// <param name="discharge">The discharge.</param>
		public void Popup(DischargeBedViewModel discharge)
		{
		}

		/// <summary>
		/// Opens an admission into a popout
		/// </summary>
		/// <param name="admissions">The admissions.</param>
		public void Popup(WaitingPatientViewModel admissions)
		{
		}

		/// <summary>
		/// Opens an admission into a popout
		/// </summary>
		/// <param name="bed">The bed.</param>
		public void Popup(AdmissionBedViewModel bed)
		{
		}

		/// <summary>
		/// Opens an trace record for the client
		/// </summary>
		/// <remarks>
		/// DO NOT DELETE: THIS METHOD IS CALLED DYNAMICALLY FROM ShellViewModel
		/// </remarks>
		public void PopupDiagnostics(ObservableCollection<LogMessage> messages)
		{
			PopupViewModel = new TraceLogViewModel(messages);

			Sound.ButtonClick.Play();
		}

		/// <remarks>
		/// DO NOT DELETE: THIS METHOD IS CALLED DYNAMICALLY FROM ShellViewModel
		/// </remarks>
		public Action<int> SetConfigurationCallback { get; set; }

		/// <remarks>
		/// DO NOT DELETE: THIS METHOD IS CALLED DYNAMICALLY FROM ShellViewModel
		/// </remarks>
		public void SetConfigurations(List<DeviceConfigurationInstance> configurations)
		{
			configurations.ForEach(Configurations.Add);
			this.DoRaisePropertyChanged(() => ConfigurationSource, RaisePropertyChanged);
		}

		public ViewModelBase PopupViewModel
		{
			get { return _popup; }
			set
			{
				_popup = value;
				this.DoRaisePropertyChanged(() => PopupViewModel, RaisePropertyChanged);
			}
		}
		public RelayCommand DismissPopupCommand
		{
			get { return new RelayCommand(DismissPopup); }
		}

		internal void DismissPopup()
		{
			PopupViewModel = null;
		}

		/// <summary>
		/// Handles the event the the app locks
		/// </summary>
		protected void SecurityViewModel_Locked()
		{
			//ScheduleViewModel.HandleLockedEvent();
			AlertViewModel.JustBeenLocked = true;
		}

		protected void SecurityViewModel_Unlocked()
		{
			AlertViewModel.DismissCommand.Execute(null);
		}

		public void Tombstone()
		{
			HandleTombstone();
		}

		public void SetPollingTimeouts(PollingTimeouts timeouts)
		{
			HandleNewTimeouts(timeouts);
		}

		protected virtual void HandleNewOrders(IList<Order> orders)
		{ }
		protected virtual void HandleNewDetections(IList<Detection> detections)
		{ }
		protected virtual void HandleNewCleaningBedData(IList<Bed> bed)
		{ }
		protected virtual void HandleNewPresences(IList<Presence> presences)
		{ }
		protected virtual void HandleNewDischargeBedData(IList<BedDischargeData> discharges)
		{ }
		protected virtual void HandleNewAdmissionsData(AdmissionsData admissionData)
		{ }
	
		protected virtual void HandleTombstone()
		{ }
		protected virtual void HandleMinuteOfInActivity()
		{ }
		public virtual void ClearAllSelections()
		{}
		protected virtual void ResetInActivity()
		{ }
		protected virtual void HandleNewTimeouts(PollingTimeouts timeouts)
		{ }
		protected void HandleException(ErrorDetails details)
		{
			Logger _logger = new Logger("SVM Unhandled", false);
			_logger.Error(details.Message);


			ErrorBarViewModel.SetError(details);
		}

		protected void HandleException(ErrorDetails details, ServerStatus serverStatus)
		{
			ErrorBarViewModel.SetDetails(details, serverStatus);
		}

		#region User Activity and Inactivity

		/// <summary>
		/// Resets user activity tracking to zero. This will reset any time-out-inactivity events from being raised
		/// </summary>
		internal void InvalidateInactivity()
		{
			_lastUserActivity = DateTime.Now;
			_activityimer.Change(1000, 1000);

			// propagate event to the security object so that it can track when to lock the screen
			SecurityViewModel.ResetAutoLockMechanism();
			AlertViewModel.ResetAutoLockMechanism();
			ResetInActivity();
		}

		/// <summary>
		/// Every second - we check if there has been any user activity. If there has been none for a minute then we have to raise a virtual method
		/// </summary>
		/// <param name="sender">The sender.</param>
		void ActivityTimer_Tick(object sender)
		{
			if (_lastUserActivity.AddMinutes(1) < DateTime.Now)
			{
				_activityimer.Change(Timeout.Infinite, Timeout.Infinite);
				HandleMinuteOfInActivity();

			}
		}


		internal void DoToggleNavigationBarVisibility()
		{
			ShowNavigationBar = !ShowNavigationBar;
		}
		#endregion
	}
}

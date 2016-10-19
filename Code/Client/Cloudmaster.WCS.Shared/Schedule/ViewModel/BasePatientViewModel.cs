using System;
using System.Linq;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using Cloudmaster.WCS.DataServicesInvoker.Types;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using WCS.Shared.Ward.Schedule;

namespace WCS.Shared.Schedule
{
	public abstract class BasePatientViewModel : ViewModelBase
	{
		public PatientViewModel Patient { get; set; }
		public TopPatient CorePatient { get; set; }

		protected Func<LocationSummary> GetDefaultLocationCallback { get; set; }
		protected Action<PatientViewModel> LocatePatientCallback { get; set; }
		protected Action ToggleNotesCallback { get; set; }
		
		private DeviceInfoViewModel _wardLocationPresence;
		private LocationClockViewModel _locationTimer;

        private bool _isSecurityLocked;

		public WcsViewModel Main { get; set; }

		private bool _showTracking;


		protected BasePatientViewModel(TopPatient patient, Action<PatientViewModel> locatePatientCallback, Func<LocationSummary> getDefaultLocationCallback)
		{
			CorePatient = patient;

			Patient = new PatientViewModel(patient);
			GetDefaultLocationCallback = getDefaultLocationCallback;
			LocatePatientCallback = locatePatientCallback;

			WardLocationPresence = new DeviceInfoViewModel { Location = patient.Admission.Location.Name, LocationFullName = patient.Admission.Location.FullName, LocationType = DeviceInfoViewModel.Mode.Ward };

            if (GetDefaultLocationCallback.Invoke() != null)
                LocationTimer = new LocationClockViewModel(GetDefaultLocationCallback.Invoke().WaitingRoomCode);

			_showTracking = false;

            // Listen for Security Locked Property Changes, set in SecurityViewModel

            Messenger.Default.Register<PropertyChangedMessage<bool>>(this, changed =>
            {
                if (changed.PropertyName == "IsSecurityLocked")
                {
                    IsSecurityLocked = changed.NewValue;
                }
            });
		}

		public int Id
		{
			get { return PatientId; }
		}

		public int GetFingerprint()
		{
			return CorePatient.GetFingerprint();
		}

		internal PatientViewModel GetPatient()
		{
			return Patient;
		}

		public int PatientId
		{
			get
			{
				if (Patient != null)
					return Patient.PatientId;
				else
					return 0;
			}
		}

        public bool IsSecurityLocked
        {
            get { return _isSecurityLocked; }
            set
            {
                _isSecurityLocked = value;

                this.DoRaisePropertyChanged(() => IsSecurityLocked, RaisePropertyChanged);
            }
        }
		 
		public DeviceInfoViewModel WardLocationPresence
		{
			get { return _wardLocationPresence; }
			private set
			{
				_wardLocationPresence = value;
				this.DoRaisePropertyChanged(() => WardLocationPresence, RaisePropertyChanged);
			}
		}

		public string SearchString
		{
			get
			{
				return Patient.SearchString;
			}
		}

		public MultiSelectAdmissionStatusFlag AdmissionStatusFlag
		{
			get { return CorePatient.Admission.AdmissionStatusFlag; }
			protected set
			{
				if (CorePatient.Admission.AdmissionStatusFlag != value)
				{
					CorePatient.Admission.AdmissionStatusFlag = value;
					this.DoRaisePropertyChanged(() => AdmissionStatusFlag, RaisePropertyChanged);
				}
			}
		}

		public string WardCode
		{
			get { return CorePatient.Admission.Location.Name; }
			protected set
			{
				if (CorePatient.Admission.Location.Name != value)
				{
					CorePatient.Admission.Location.Name = value;
					this.DoRaisePropertyChanged(() => WardCode, RaisePropertyChanged);
				}
			}
		}

		public string WardName
		{
			get { return CorePatient.Admission.Location.FullName; }
			protected set
			{
				if (CorePatient.Admission.Location.FullName != value)
				{
					CorePatient.Admission.Location.FullName = value;
					this.DoRaisePropertyChanged(() => WardName, RaisePropertyChanged);
				}
			}
		}

		public RelayCommand LocatePatientCommand
		{
			get { return new RelayCommand(DoLocatePatient); }
		}
		
		private void DoLocatePatient()
		{
			if (LocatePatientCallback != null)
				LocatePatientCallback.Invoke(Patient);
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

		public RelayCommand ToggleTrackingCommand
		{
			get { return new RelayCommand(DoToggleTracking); }
		}

		private void DoToggleTracking()
		{
			ShowTracking = !ShowTracking;
		}

		public bool ShowTracking
		{
			get { return _showTracking; }
			set
			{
				_showTracking = value;
				this.DoRaisePropertyChanged(() => ShowTracking, RaisePropertyChanged);
			}
		}

		#region IDispose

		private volatile bool _disposed = false;


		protected override void Dispose(bool disposing)
		{
			if (!this._disposed)
			{
				_disposed = true;

				if (Patient != null)
				{
					Patient.Dispose();
					Patient = null;
				}
			}
		}
		#endregion
	}
}

using System;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using Cloudmaster.WCS.DataServicesInvoker.Types;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using WCS.Shared.Admissions.Schedule;
using WCS.Shared.Beds;
using WCS.Shared.Discharge.Schedule;

namespace WCS.Shared.Schedule
{
	public class DeviceInfoViewModel : ViewModelBase
	{
		public enum Mode
		{
			Ward,
			Department
		}

		private DateTime? _lastConnectionTime;
		private string _location;
		private string _locationContact;
		private int _shortTermWardNotificationsPending;
		private int _longTermWardNotificationsPending;
		private string _locationFullName;
		private Mode _locationType;
		private bool _isPopupOpen;

		public DeviceInfoViewModel()
		{
			LocationContact = "";
		}

		public string Location
		{
			get { return _location; }
			set
			{
				_location = value;
				this.DoRaisePropertyChanged(() => Location, RaisePropertyChanged);
			}
		}
		public string LocationFullName
		{
			get { return _locationFullName; }
			set
			{
				_locationFullName = value;
				this.DoRaisePropertyChanged(() => LocationFullName, RaisePropertyChanged);
			}
		}

		public Mode LocationType
		{
			get { return _locationType; }
			set
			{
				_locationType = value;
				this.DoRaisePropertyChanged(() => LocationType, RaisePropertyChanged);
			}
		}

		public string LocationContact
		{
			get { return _locationContact; }
			set
			{
				_locationContact = value;
				this.DoRaisePropertyChanged(() => LocationContact, RaisePropertyChanged);
			}
		}
		
		public DateTime? LastLocationConnectionTime
		{
			get { return _lastConnectionTime; }
			set
			{
				_lastConnectionTime = value;
				this.DoRaisePropertyChanged(() => LastLocationConnectionTime, RaisePropertyChanged);
			}
		}
		public TimeSpan? TimeSinceLastConnection
		{
			set { if (value.HasValue) LastLocationConnectionTime = DateTime.Now - value.Value; }
		}

		public int LongTermWardNotificationsPending
		{
			get { return _longTermWardNotificationsPending; }
			set
			{
				_longTermWardNotificationsPending = value;
				this.DoRaisePropertyChanged(() => LongTermWardNotificationsPending, RaisePropertyChanged);
			}
		}

		public int ShortTermWardNotificationsPending
		{
			get { return _shortTermWardNotificationsPending; }
			set
			{
				_shortTermWardNotificationsPending = value;
				this.DoRaisePropertyChanged(() => ShortTermWardNotificationsPending, RaisePropertyChanged);
			}
		}

		public RelayCommand OpenPopupCommand
		{
			get { return new RelayCommand(DoOpenPopup); }
		}

		public bool IsPopupOpen
		{
			get { return _isPopupOpen; }
			set
			{
				_isPopupOpen = value;
				this.DoRaisePropertyChanged(() => IsPopupOpen, RaisePropertyChanged);
			}
		}

		internal void Synchronise(TopPatient patient)
		{
			Location = patient.Admission.Location.Name;
			LocationFullName = patient.Admission.Location.FullName;
		}

		internal void Synchronise(TopRoom room)
		{
			Location = room.Room.Ward;
			LocationFullName = room.Room.Ward;
		}

		internal void Synchronise(DischargeTopRoom discharge)
		{
			Location = discharge.Room.Ward;
			LocationFullName = discharge.Room.Ward;
		}

		internal void Synchronise(AdmissionsTopWard admission)
		{
			Location = admission.Ward.Name;
			LocationFullName = admission.Ward.FullName;
		}

		internal void Synchronise(Presence locationConnection)
		{
			LocationContact = locationConnection.Contact;
			TimeSinceLastConnection = locationConnection.TimeSinceLastConnection;
			ShortTermWardNotificationsPending = locationConnection.ShortTermNotificationsPending;
			LongTermWardNotificationsPending = locationConnection.LongTermNotificationsPending;
		}

		private void DoOpenPopup()
		{
			IsPopupOpen = true;
		}
	}
}

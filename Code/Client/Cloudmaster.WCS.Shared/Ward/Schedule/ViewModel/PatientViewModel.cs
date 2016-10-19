using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using Cloudmaster.WCS.DataServicesInvoker.Types;
using GalaSoft.MvvmLight;
using WCS.Core;
using WCS.Shared.Location;

namespace WCS.Shared.Ward.Schedule
{
	public class PatientViewModel : ViewModelBase
	{
		private TopPatient _patient;
		private Order _order;
		protected LocationTrackingCoordinator _locationTrackingCoordinator;
		private RfidCardViewModel _rfidTracker;

		public PatientViewModel(TopPatient patient)
		{
			_patient = patient;
			_order = patient.Orders.First();

			LocationTrackingCoordinator = new LocationTrackingCoordinator();
			RfidTracker = new RfidCardViewModel(this);
		}

		#region IDispose

		private volatile bool _disposed = false;

		protected override void Dispose(bool disposing)
		{
			if (!this._disposed)
			{
				_disposed = true;

				if (_locationTrackingCoordinator != null)
				{
					_locationTrackingCoordinator.Dispose();
					_locationTrackingCoordinator = null;
				}
			}
		}
		#endregion

		public TopPatient Patient
		{
			get { return _patient; }
		}

		public RfidCardViewModel RfidTracker
		{
			get { return _rfidTracker; }
			set
			{
				_rfidTracker = value;
				this.DoRaisePropertyChanged(() => IPeopleNumber, RaisePropertyChanged);
			}
		}

		public Admission Admission
		{
			get { return _order.Admission; }
		}

		public string IPeopleNumber
		{
			get { return _order.Admission.Patient.IPeopleNumber ?? ""; }
			private set
			{
				if (_order.Admission.Patient.IPeopleNumber != value)
				{
					_order.Admission.Patient.IPeopleNumber = value;
					this.DoRaisePropertyChanged(() => IPeopleNumber, RaisePropertyChanged);
				}
			}
		}


		public string FamilyName
		{
			get { return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(_order.Admission.Patient.FamilyName.ToLower() ?? ""); }
			private set
			{
				if (_order.Admission.Patient.FamilyName != value)
				{
					_order.Admission.Patient.FamilyName = value;
					this.DoRaisePropertyChanged(() => FamilyName, RaisePropertyChanged);
				}
			}
		}

		public string GivenName
		{
			get { return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(_order.Admission.Patient.GivenName.ToLower() ?? ""); }
			set
			{
				if (_order.Admission.Patient.GivenName != value)
				{
					_order.Admission.Patient.GivenName = value;
					this.DoRaisePropertyChanged(() => GivenName, RaisePropertyChanged);
				}
			}
		}

		public string SearchString
		{
			get { return string.Format("{0} {1}", GivenName, FamilyName).Trim().ToUpper(); }
		}


		public string DepartmentCode
		{
			get { return _order.DepartmentCode ?? ""; }
			private set
			{
				if (_order.DepartmentCode != value)
				{
					_order.DepartmentCode = value;
					this.DoRaisePropertyChanged(() => DepartmentCode, RaisePropertyChanged);
				}
			}
		}


		public string DepartmentName
		{
			get { return _order.DepartmentName ?? ""; }
			private set
			{
				if (_order.DepartmentName != value)
				{
					_order.DepartmentName = value;
					this.DoRaisePropertyChanged(() => DepartmentName, RaisePropertyChanged);
				}
			}
		}

        public DateTime? OrderCreated
        {
            get
            {
                var update = _order.Updates.FirstOrDefault(u => u.Type == "Order Imported");

                if (update != null)
                {
                    return update.Created;
                }

                return null;
            }
            
        }

		public string LocationFullName
		{
			get { return _order.Admission.Location.FullName ?? ""; }
			set
			{
				if (_order.Admission.Location.FullName != value)
				{
					_order.Admission.Location.FullName = value;
					this.DoRaisePropertyChanged(() => LocationFullName, RaisePropertyChanged);
				}
			}
		}

		public string Location
		{
			get { return _order.Admission.Location.Name ?? ""; }
			set
			{
				if (_order.Admission.Location.Name != value)
				{
					_order.Admission.Location.Name = value;
					this.DoRaisePropertyChanged(() => Location, RaisePropertyChanged);
				}
			}
		}

		public string Room
		{
			get { return _order.Admission.Location.Room ?? ""; }
			set
			{
				if (_order.Admission.Location.Room != value)
				{
					_order.Admission.Location.Room = value;
					this.DoRaisePropertyChanged(() => Room, RaisePropertyChanged);
				}
			}
		}


		public string Bed
		{
			get { return _order.Admission.Location.Bed ?? ""; }
			private set
			{
				if (_order.Admission.Location.Bed != value)
				{
					_order.Admission.Location.Bed = value;
					this.DoRaisePropertyChanged(() => Bed, RaisePropertyChanged);
				}
			}
		}
		public int PatientId
		{
			get { return _order.Admission.Patient.PatientId; }
			set
			{
				if (_order.Admission.Patient.PatientId != value)
				{
					_order.Admission.Patient.PatientId = value;
					this.DoRaisePropertyChanged(() => PatientId, RaisePropertyChanged);
				}
			}
		}
		 
		public bool IsAssistanceRequired
		{
			get { return _order.Admission.Patient.IsAssistanceRequired; }
			set
			{
				if (_order.Admission.Patient.IsAssistanceRequired != value)
				{
					_order.Admission.Patient.IsAssistanceRequired = value;
					this.DoRaisePropertyChanged(() => IsAssistanceRequired, RaisePropertyChanged);
				}
			}
		}
		public string AssistanceDescription
		{
			get { return _order.Admission.Patient.AssistanceDescription; }
			set
			{
				if (_order.Admission.Patient.AssistanceDescription != value)
				{
					_order.Admission.Patient.AssistanceDescription = value;
					this.DoRaisePropertyChanged(() => AssistanceDescription, RaisePropertyChanged);
				}
			}
		}

		public MultiSelectAdmissionStatusFlag AdmissionStatusFlag
		{
			get { return _order.Admission.AdmissionStatusFlag; }
			set
			{
				if (_order.Admission.AdmissionStatusFlag != value)
				{
					_order.Admission.AdmissionStatusFlag = value;
					this.DoRaisePropertyChanged(() => AdmissionStatusFlag, RaisePropertyChanged);
				}
			}
		}

		public AdmissionType AdmissionType
		{
			get { return _order.Admission.Type; }
			private set
			{
				if (_order.Admission.Type != value)
				{
					_order.Admission.Type = value;
					this.DoRaisePropertyChanged(() => AdmissionType, RaisePropertyChanged);
				}
			}
		}

		/// <summary>
		/// This is a diabolical implementation of DateOfBirth. Massive inefficencies. Re-implement as a nullable datetime and write a converter for the UI
		/// </summary>
		public string DateOfBirth
		{
			get
			{
				return _order.Admission.Patient.DateOfBirth.HasValue
						? _order.Admission.Patient.DateOfBirth.Value.ToWcsDisplayDateFormat()
						: "";
			}
			private set
			{
				if (_order.Admission.Patient.DateOfBirth.HasValue && _order.Admission.Patient.DateOfBirth.Value.ToWcsDisplayDateFormat() != value)
				{
					_order.Admission.Patient.DateOfBirth = DateTime.Parse(value);
					this.DoRaisePropertyChanged(() => DateOfBirth, RaisePropertyChanged);
					this.DoRaisePropertyChanged(() => Age, RaisePropertyChanged);
				}
			}
		}

		public string Age
		{
			get
			{
				return (_order.Admission.Patient.DateOfBirth.HasValue
						? Math.Floor((DateTime.Today - _order.Admission.Patient.DateOfBirth.Value).Days / 365.0).ToString()
						: "Unknown");
			}
		}

		public bool MrsaRisk
		{
			get { return _order.Admission.CriticalCareIndicators.IsMrsaRisk; }
			private set
			{
				if (_order.Admission.CriticalCareIndicators.IsMrsaRisk != value)
				{
					_order.Admission.CriticalCareIndicators.IsMrsaRisk = value;
					this.DoRaisePropertyChanged(() => MrsaRisk, RaisePropertyChanged);
				}
			}
		}

		public bool FallRisk
		{
			get { return _order.Admission.CriticalCareIndicators.IsFallRisk; }
			private set
			{
				if (_order.Admission.CriticalCareIndicators.IsFallRisk != value)
				{
					_order.Admission.CriticalCareIndicators.IsFallRisk = value;
					this.DoRaisePropertyChanged(() => FallRisk, RaisePropertyChanged);
				}
			}
		}


		public bool LatexAllergy
		{
			get { return _order.Admission.CriticalCareIndicators.HasLatexAllergy; }
			private set
			{
				if (_order.Admission.CriticalCareIndicators.HasLatexAllergy != value)
				{
					_order.Admission.CriticalCareIndicators.HasLatexAllergy = value;
					this.DoRaisePropertyChanged(() => LatexAllergy, RaisePropertyChanged);
				}
			}
		}

		public bool RadiationRisk
		{
			get { return _order.Admission.CriticalCareIndicators.IsRadiationRisk; }
			private set
			{
				if (_order.Admission.CriticalCareIndicators.IsRadiationRisk != value)
				{
					_order.Admission.CriticalCareIndicators.IsRadiationRisk = value;
					this.DoRaisePropertyChanged(() => RadiationRisk, RaisePropertyChanged);
				}
			}
		}

		public LocationTrackingCoordinator LocationTrackingCoordinator
		{
			get { return _locationTrackingCoordinator; }
			private set { _locationTrackingCoordinator = value; }
		}

		internal void Synchronise(Order order)
		{
			PatientId = order.Admission.Patient.PatientId;
			IsAssistanceRequired = order.Admission.Patient.IsAssistanceRequired;
			AssistanceDescription = order.Admission.Patient.AssistanceDescription;
			AdmissionStatusFlag = order.Admission.AdmissionStatusFlag;
			AdmissionType = order.Admission.Type;
			FamilyName = order.Admission.Patient.FamilyName;
			IPeopleNumber = order.Admission.Patient.IPeopleNumber;
			GivenName = order.Admission.Patient.GivenName;
			DepartmentCode = order.DepartmentCode;
			DepartmentName = order.DepartmentName;
			Location = order.Admission.Location.Name;
			LocationFullName = order.Admission.Location.FullName;
			Room = order.Admission.Location.Room;
			Bed = order.Admission.Location.Bed;
			MrsaRisk = order.Admission.CriticalCareIndicators.IsMrsaRisk;
			FallRisk = order.Admission.CriticalCareIndicators.IsFallRisk;
			LatexAllergy = order.Admission.CriticalCareIndicators.HasLatexAllergy;
			RadiationRisk = order.Admission.CriticalCareIndicators.IsRadiationRisk;
			if (order.Admission.Patient.DateOfBirth.HasValue)
				DateOfBirth = order.Admission.Patient.DateOfBirth.Value.ToWcsDisplayDateFormat();
		}

		internal void Synchronise(IList<Detection> detections)
		{
			LocationTrackingCoordinator.Synchronise(detections);
		}

	}
}

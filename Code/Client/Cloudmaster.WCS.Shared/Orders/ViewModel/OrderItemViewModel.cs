using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudmaster.WCS.DataServicesInvoker;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using Cloudmaster.WCS.DataServicesInvoker.Types;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using WCS.Core;
using WCS.Shared.Notes;
using WCS.Shared.Schedule;
using WCS.Shared.Ward.Schedule;

namespace WCS.Shared.Orders
{
    public class OrderItemViewModel : ViewModelBase, IOrderItem
    {
        private Order _order;
        private InLineNotesViewModel _notes;
        private PatientViewModel _patient;


        public event Action<DateTime?> ProcedureTimeChanged;

        private DeviceInfoViewModel _departmentLocationPresence;
        private DeviceInfoViewModel _wardLocationPresence;

        public OrderItemViewModel(Order order, PatientViewModel patient, Action showNotesDelegate)
        {
            _order = order;

            _notes = new InLineNotesViewModel(order.Notes, showNotesDelegate);
            _patient = patient;

            DepartmentLocationPresence = new DeviceInfoViewModel { Location = order.DepartmentCode, LocationFullName = order.DepartmentName, LocationType = DeviceInfoViewModel.Mode.Department };
            WardLocationPresence = new DeviceInfoViewModel { Location = order.Admission.Location.Name, LocationFullName = order.Admission.Location.FullName, LocationType = DeviceInfoViewModel.Mode.Ward };

        }

        #region IOrderItem Members

        public int Id
        {
            get { return _order.OrderId; }
        }

        public OrderScheduleItemType AppointmentType
        {
            get { return OrderScheduleItemType.Order; }
        }

        public TimeSpan? StartTime
        {
            get
            {
                if (_order.ProcedureTime.HasValue)
                    return _order.ProcedureTime.Value.TimeOfDay;
                return new TimeSpan(4, 0, 0);
            }
            set
            {
				Debug.WriteLine(string.Format("Start Time: {0}",value));

                if (!value.HasValue || value < new TimeSpan(4, 0, 0))
                    value = new TimeSpan(4, 0, 0);

                var st = value.Value.RoundToNearest15Minutes();
                var dt = DateTime.Now;
                var proposedNewProcedureTime = new DateTime(dt.Year, dt.Month, dt.Day, st.Hours, st.Minutes, 0);
                
				if (_order.ProcedureTime == proposedNewProcedureTime) return;

                _order.ProcedureTime = proposedNewProcedureTime;


                this.DoRaisePropertyChanged(() => StartTime, RaisePropertyChanged);
                this.DoRaisePropertyChanged(() => ProcedureTime, RaisePropertyChanged);

                RaiseStartDateChanged();
            }
        }

     
        public void Synchronise(Order order)
        {
			if (order.ProcedureTime.HasValue && (StartTime != order.ProcedureTime.Value.TimeOfDay))
			{
				StartTime = order.ProcedureTime.Value.TimeOfDay;
			}
			CompletedTime = order.CompletedTime;
            Duration = order.Duration;
            AdmissionStatusFlag = order.Admission.AdmissionStatusFlag;
            OrderStatus = order.Status;
            ProcedureCode = order.ProcedureCode;
            ProcedureDescription = order.ProcedureDescription;
            OrderNumber = order.OrderNumber;
            OrderingDoctor = order.OrderingDoctor;
            WardCode = order.Admission.Location.Name;
            WardName = order.Admission.Location.FullName;
            IsAcknowledged = order.Acknowledged;
            ClinicalIndicators = order.ClinicalIndicators;

			this.DoRaisePropertyChanged(() => DateCreated, RaisePropertyChanged);	// this is now an infered property
            this.DoRaisePropertyChanged(() => ClinicalIndicators, RaisePropertyChanged);	// there is no setter. this can be cleaned

            _notes.Synchronise(order);
            _patient.Synchronise(order);

            _departmentLocationPresence.Location = order.DepartmentCode;
            _departmentLocationPresence.LocationFullName = order.DepartmentName;
            _departmentLocationPresence.LocationType = DeviceInfoViewModel.Mode.Department;
            _wardLocationPresence.Location = order.Admission.Location.Name;
            _wardLocationPresence.LocationFullName = order.Admission.Location.FullName;
            _wardLocationPresence.LocationType = DeviceInfoViewModel.Mode.Department;
        }

        public void Synchronise(Notification notification)
        {
            throw new NotImplementedException();
        }

        public void Synchronise(IList<Detection> detections)
        {
            _patient.Synchronise(detections);
        }


        public void HandleMinuteTimerTick()
        {
            this.DoRaisePropertyChanged(() => StartTime, RaisePropertyChanged);
        }

        public TimeSpan Duration
        {
            get { return _order.Duration; }
            private set
            {
                if (_order.Duration != value)
                {
                    _order.Duration = value;
                    this.DoRaisePropertyChanged(() => Duration, RaisePropertyChanged);
                }
            }
        }

        public TimeSpan? PriorTime
        {
            get { return null; }
        }


        public bool Equals(IOrderItem other)
        {
            if (other == null) return false;
            return (this.Id.Equals(other.Id) && this.AppointmentType.Equals(other.AppointmentType));
        }

        public override int GetHashCode()
        {
            return Id ^ (int)AppointmentType;
        }

        #endregion

        #region IDispose

        private bool _disposed = false;

        protected override void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (_notes != null)
                {
                    _notes.Dispose();
                    _notes = null;
                }
                _disposed = true;

            }


            base.Dispose(disposing);
        }
        #endregion

        public PatientViewModel Patient
        {
            get { return _patient; }
        }

        public bool IsAcknowledged
        {
            get { return _order.Acknowledged; }
            set
            {
                _order.Acknowledged = value;
                this.DoRaisePropertyChanged(() => IsAcknowledged, RaisePropertyChanged);
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

        public OrderStatus OrderStatus
        {
            get { return _order.Status; }
            set
            {
                if (_order.Status != value)
                {
                    _order.Status = value;
                    this.DoRaisePropertyChanged(() => OrderStatus, RaisePropertyChanged);
                }
            }
        }

        public string ProcedureCode
        {
            get { return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(_order.ProcedureCode); }
            private set
            {
                if (_order.ProcedureCode != value)
                {
                    _order.ProcedureCode = value;
                    this.DoRaisePropertyChanged(() => ProcedureCode, RaisePropertyChanged);
                }
            }
        }

        public string ProcedureDescription
        {
            get { return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(_order.ProcedureDescription.ToLower()); }
            private set
            {
                if (_order.ProcedureDescription != value)
                {
                    _order.ProcedureCode = value;
                    this.DoRaisePropertyChanged(() => ProcedureDescription, RaisePropertyChanged);
                }
            }
        }

        public string ClinicalIndicators
        {
            get { return _order.ClinicalIndicators; }
            set
            {
                _order.ClinicalIndicators = value;
                this.DoRaisePropertyChanged(() => ClinicalIndicators, RaisePropertyChanged);
            }
        }

        public string History
        {
            get { return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(_order.History.ToLower()); }
            set
            {
                _order.History = value;
                this.DoRaisePropertyChanged(() => History, RaisePropertyChanged);
            }
        }

        public string Diagnosis
        {
            get { return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(_order.Diagnosis.ToLower()); }
            set
            {
                _order.History = value;
                this.DoRaisePropertyChanged(() => Diagnosis, RaisePropertyChanged);
            }
        }

        public bool HasRequirements
        {
            get { return _order.RequiresSupervision || _order.RequiresMedicalRecords || _order.RequiresFootwear; }

        }

        public bool RequiresSupervision
        {
            get { return _order.RequiresSupervision; }
            set
            {
                _order.RequiresSupervision = value;
                this.DoRaisePropertyChanged(() => RequiresSupervision, RaisePropertyChanged);
            }
        }

        public bool RequiresFootwear
        {
            get { return _order.RequiresFootwear; }
            set
            {
                _order.RequiresFootwear = value;
                this.DoRaisePropertyChanged(() => RequiresFootwear, RaisePropertyChanged);
            }
        }

        public bool RequiresMedicalRecords
        {
            get { return _order.RequiresMedicalRecords; }
            set
            {
                _order.RequiresMedicalRecords = value;
                this.DoRaisePropertyChanged(() => RequiresMedicalRecords, RaisePropertyChanged);
            }
        }

        public bool IsUserModified { get; set; }

        public bool IsUserModifiedComplete { get; set; }

        public DateTime LastUserModified { get; set; }



        /// <summary>
        /// Gets the procedure time.
        /// </summary>
        /// <remarks>
        /// StartTime is the same as ProcedureTime
        /// </remarks>
        public DateTime? ProcedureTime
        {
            get
            {
                if (!_order.ProcedureTime.HasValue)
                    return null;
                if (_order.ProcedureTime.Value.TimeOfDay > new TimeSpan(0, 0, 0, 0))
                    return _order.ProcedureTime;
                else
                    return null;
            }
        }

		public DateTime? DateCreated
		{
			get
			{
				var importedUpdate = _order.Updates.FirstOrDefault(u => u.Type == "Order Imported");
				if (importedUpdate == null) return null;
				return importedUpdate.Created;
			}
		}

		public DateTime? CompletedTime
		{
			get { return _order.CompletedTime; }
			private set
			{
				if (_order.CompletedTime != value)
				{
					_order.CompletedTime = value;
					this.DoRaisePropertyChanged(() => CompletedTime, RaisePropertyChanged);
				}
			}
		}

        public string OrderNumber
        {
            get { return _order.OrderNumber; }
            private set
            {
                if (_order.OrderNumber != value)
                {
                    _order.OrderNumber = value;
                    this.DoRaisePropertyChanged(() => OrderNumber, RaisePropertyChanged);
                }
            }
        }

        public string WardCode
        {
            get { return _order.Admission.Location.Name; }
            private set
            {
                if (_order.Admission.Location.Name != value)
                {
                    _order.Admission.Location.Name = value;
                    this.DoRaisePropertyChanged(() => WardCode, RaisePropertyChanged);
                }
            }
        }

        public string WardName
        {
            get { return _order.Admission.Location.FullName; }
            private set
            {
                if (_order.Admission.Location.FullName != value)
                {
                    _order.Admission.Location.FullName = value;
                    this.DoRaisePropertyChanged(() => WardName, RaisePropertyChanged);
                }
            }
        }

        public string OrderingDoctor
        {
            get
            {
                if (_order.OrderingDoctor != null)
                {
                    return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(_order.OrderingDoctor.ToLower());
                }
                else
                {
                    return string.Empty;
                }
            }
            private set
            {
                if (_order.OrderingDoctor != value)
                {
                    _order.OrderNumber = value;
                    this.DoRaisePropertyChanged(() => OrderingDoctor, RaisePropertyChanged);
                }
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
        public DeviceInfoViewModel DepartmentLocationPresence
        {
            get { return _departmentLocationPresence; }
            private set
            {
                _departmentLocationPresence = value;
                this.DoRaisePropertyChanged(() => DepartmentLocationPresence, RaisePropertyChanged);
            }
        }

        /// <summary>
        /// Raises the ProcedureTimeChanged event
        /// </summary>
        public void RaiseStartDateChanged()
        {
            var ptc = ProcedureTimeChanged;
            if (ptc != null)
            {
                ptc(_order.ProcedureTime);
            }
        }

        private bool _isScheduled;
        public bool IsScheduled
        {
            get { return _isScheduled; }
            set
            {
                _isScheduled = value;
                this.DoRaisePropertyChanged(() => IsScheduled, RaisePropertyChanged);
            }
        }

        public InLineNotesViewModel Notes
        {
            get { return _notes; }
            set
            {
                _notes = value;
                this.DoRaisePropertyChanged(() => Notes, RaisePropertyChanged);
            }
        }


        public int GetFingerprint()
        {
            return _order.GetFingerprint();
        }


        public RelayCommand AcknowledgeCommand
        {
            get { return new RelayCommand(DoAcknowledgeCommand); }
        }

        private void DoAcknowledgeCommand()
        {
            IsAcknowledged = true;

            var invoker = WcsViewModel.MefContainer.GetExportedValue<IWcsAsyncInvoker>();
            if (invoker != null)
                invoker.AcknowledgeOrderAsync(_order.OrderId, Synchronise);
        }

    }
}

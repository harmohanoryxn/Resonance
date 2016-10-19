using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace WCS.Services.DataServices
{
	[Serializable]
	[DataContract(Namespace = "http://schemas.cloudmaster.ie/2012/01/WCS/1")]
	public class AuthenticationToken
	{
		[DataMember]
		public bool IsAuthenticated { get; set; }

		[DataMember]
		public string Message { get; set; }

		[DataMember]
		public int LockScreenTimeout { get; set; }
	}

	[Serializable]
	[DataContract(Namespace = "http://schemas.cloudmaster.ie/2012/01/WCS/1")]
	public class DeviceConfiguration
	{
		[DataMember]
		public string DeviceName { get; set; }

		[DataMember]
		public List<DeviceConfigurationInstance> Instances { get; set; }
	}

	[Serializable]
	[DataContract(Namespace = "http://schemas.cloudmaster.ie/2012/01/WCS/1")]
	public class PollingTimeouts
	{
		[DataMember]
		public int OrderTimeout { get; set; }

		[DataMember]
		public int CleaningBedDataTimeout { get; set; }

		[DataMember]
		public int RfidTimeout { get; set; }

		[DataMember]
		public int PresenceTimeout { get; set; }

		[DataMember]
		public int ConfigurationTimeout { get; set; }

		[DataMember]
		public int DischargeTimeout { get; set; }

		[DataMember]
		public int AdmissionsTimeout { get; set; } 
	}

	[Serializable]
	[DataContract(Namespace = "http://schemas.cloudmaster.ie/2012/01/WCS/1")]
	public class DeviceConfigurationInstance
	{
        [DataMember]
        public int ShortcutKey { get; set; }

        [DataMember]
        public string LocationName { get; set; }

        [DataMember]
        public string LocationCode { get; set; }

        [DataMember]
        public string WaitingRoomLocationCode { get; set; }

        [DataMember]
        public bool RequiresLoggingOn { get; set; }

        [DataMember]
        public DeviceConfigurationType Type { get; set; }

		[DataMember]
		public List<LocationSummary> VisibleLocations { get; set; }

        [DataMember]
        public PollingTimeouts PollingTimeouts { get; set; }
	}


	[Serializable]
	[DataContract(Namespace = "http://schemas.cloudmaster.ie/2012/01/WCS/1")]
	public class Presence
	{
		[DataMember]
		public int LocationId { get; set; }

		[DataMember]
		public string LocationCode { get; set; }

		[DataMember]
		public string Contact { get; set; }

		[DataMember]
		public TimeSpan? TimeSinceLastConnection { get; set; }

		[DataMember]
		public int ShortTermNotificationsPending { get; set; }

		[DataMember]
		public int LongTermNotificationsPending { get; set; }
	}

	[Serializable]
	[DataContract(Namespace = "http://schemas.cloudmaster.ie/2012/01/WCS/1")]
	public class LocationSummary : IEquatable<LocationSummary>
	{
		[DataMember]
		public int LocationId { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Code { get; set; }

        [DataMember]
        public string WaitingRoomCode { get; set; }

		public bool Equals(LocationSummary other)
		{
			return (this.Name == other.Name && this.Code == other.Code && this.LocationId == other.LocationId);
		}
	}


	[Serializable]
	[DataContract(Name = "AdmissionType", Namespace = "http://schemas.cloudmaster.ie/2012/01/WCS/1")]
	public enum AdmissionType
	{
        [EnumMember]
        Unknown = 0,
        [EnumMember]
		In,
		[EnumMember]
		Out,
		[EnumMember]
		Day
	}


	[Serializable]
	[DataContract(Name="AdmissionStatus", Namespace = "http://schemas.cloudmaster.ie/2012/01/WCS/1")]
	public enum AdmissionStatus
	{
        [EnumMember]
        Unknown = 0,
        [EnumMember]
		Registered = 1,
		[EnumMember]
		Admitted = 2,
		[EnumMember]
		Discharged = 4
	}

	[Flags, Serializable]
	[DataContract(Name = "OrderStatus", Namespace = "http://schemas.cloudmaster.ie/2012/01/WCS/1")]
	public enum OrderStatus
	{
		[EnumMember]
		InProgress = 1,
		[EnumMember]
		Completed =2,
		[EnumMember]
		Cancelled =4
	}

    [Serializable]
	[DataContract(Name = "DeviceConfigurationType", Namespace = "http://schemas.cloudmaster.ie/2012/01/WCS/1")]
	public enum DeviceConfigurationType
	{
		[EnumMember]
		Ward,
		[EnumMember]
		Department,
		[EnumMember]
		Cleaning,
		[EnumMember]
		Physio,
        [EnumMember]
        Admissions,
		[EnumMember]
		Discharge
	}

	[Serializable]
	[DataContract(Namespace = "http://schemas.cloudmaster.ie/2012/01/WCS/1")]
	public class Order
	{ 
		[DataMember]
		public int OrderId { get; set; }

		[DataMember]
		public Admission Admission { get; set; }

		[DataMember]
		public string DepartmentCode { get; set; }

		[DataMember]
		public string DepartmentName { get; set; }

		[DataMember]
		public string OrderNumber { get; set; }

		[DataMember]
		public string ProcedureCode { get; set; }

		[DataMember]
		public string ProcedureDescription { get; set; }

		[DataMember]
		public DateTime? ProcedureTime { get; set; }

		[DataMember]
		public TimeSpan Duration{ get; set; }

        [DataMember]
        public int? EstimatedProcedureDuration { get; set; }

		[DataMember]
		public OrderStatus Status { get; set; }

		[DataMember]
		public string ClinicalIndicators { get; set; }

        [DataMember]
        public bool IsHidden { get; set; }

        [DataMember]
		public bool Acknowledged { get; set; }
		 
		[DataMember]
		public string OrderingDoctor { get; set; }

        [DataMember]
        public string History { get; set; }

        [DataMember]
        public string Diagnosis { get; set; }

        [DataMember]
        public string CurrentCardiologist { get; set; }

        [DataMember]
        public bool RequiresSupervision { get; set; }

        [DataMember]
        public bool RequiresFootwear { get; set; }

        [DataMember]
        public bool RequiresMedicalRecords { get; set; }

		[DataMember]
		public DateTime? CompletedTime { get; set; }

		[DataMember]
		public IEnumerable<Note> Notes { get; set; }

		[DataMember]
		public IEnumerable<Notification> Notifications { get; set; }

		[DataMember]
		public IEnumerable<Update> Updates { get; set; }

        public override string ToString()
        {
            return String.Format("{0} {1} {2}", OrderNumber, DepartmentCode, Admission);
        }
	}

	[Serializable]
	[DataContract(Namespace = "http://schemas.cloudmaster.ie/2012/01/WCS/1")]
	public class Notification
	{
		[DataMember]
		public int NotificationId { get; set; }

		[DataMember]
		public int OrderId { get; set; }

		[DataMember]
		public NotificationType NotificationType { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public TimeSpan Duration { get; set; }
		 
		[DataMember]
		public TimeSpan PriorToProcedureTime { get; set; }

		[DataMember]
		public bool RequiresAcknowledgement { get; set; }

		[DataMember]
		public bool Acknowledged { get; set; }

		[DataMember]
		public DateTime? AcknowledgedTime { get; set; }

		[DataMember]
		public string AcknowledgedBy { get; set; }
	}


	[Serializable]
	[DataContract(Namespace = "http://schemas.cloudmaster.ie/2012/01/WCS/1")]
	public class Note
	{
		[DataMember]
		public int NoteId { get; set; }

		[DataMember]
		public int? OrderId { get; set; }

		[DataMember]
		public int? BedId { get; set; }

		[DataMember]
		public string Source { get; set; }

		[DataMember]
		public string NoteMessage { get; set; }

		[DataMember]
		public DateTime DateCreated { get; set; }
	}

	[Serializable]
	[DataContract(Namespace = "http://schemas.cloudmaster.ie/2012/01/WCS/1")]
	public class Update
	{
		[DataMember]
		public int UpdateId { get; set; }

		[DataMember]
		public int? OrderId { get; set; }

		[DataMember]
		public int? BedId { get; set; }

		[DataMember]
        public int? AdmissionId { get; set; }

		[DataMember]
		public string Type { get; set; }

		[DataMember]
		public string Source { get; set; }

		[DataMember]
		public string Value { get; set; }

		[DataMember]
		public DateTime Created { get; set; }
	}

	[Serializable]
	[DataContract(Namespace = "http://schemas.cloudmaster.ie/2012/01/WCS/1")]
	public class Admission
	{
		[DataMember]
		public int AdmissionId { get; set; }

        [DataMember]
        public string DisplayCode { get; set; }

        [DataMember]
		public Patient Patient { get; set; }

		[DataMember]
		public DateTime AdmitDateTime { get; set; }

        [DataMember]
        public DateTime? EstimatedDischargeDateTime { get; set; }

        [DataMember]
        public DateTime DischargeDateTime { get; set; }

        [DataMember]
		public Location Location { get; set; }

		[DataMember]
		public AdmissionType Type { get; set; }

		[DataMember]
		public AdmissionStatus Status { get; set; }

		[DataMember]
		public CriticalCareIndicators CriticalCareIndicators { get; set; }
		
		[DataMember]
		public string PrimaryDoctor { get; set; }

		[DataMember]
		public string AttendingDoctor { get; set; }

		[DataMember]
		public string AdmittingDoctor { get; set; }

        [DataMember]
        public IEnumerable<Update> Updates { get; set; }

        public override string ToString()
        {
            return String.Format("{0} {1}", DisplayCode, Patient);
        }
	}

	[Serializable]
	[DataContract(Namespace = "http://schemas.cloudmaster.ie/2012/01/WCS/1")]
	public class CriticalCareIndicators
	{
		[DataMember]
		public bool IsMrsaRisk { get; set; }

		[DataMember]
		public bool IsFallRisk { get; set; }

		[DataMember]
		public bool HasLatexAllergy { get; set; }

        [DataMember]
        public bool IsRadiationRisk { get; set; }
    }

	[Serializable]
	[DataContract(Namespace = "http://schemas.cloudmaster.ie/2012/01/WCS/1")]
	public class Location
	{
		[DataMember]
		public string FullName { get; set; }

        [DataMember]
        public bool IsEmergency { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Room { get; set; }

		[DataMember]
		public string Bed { get; set; }
	}

	[Serializable]
	[DataContract(Namespace = "http://schemas.cloudmaster.ie/2012/01/WCS/1")]
	public class Patient
	{
		[DataMember]
		public int PatientId { get; set; }

		[DataMember]
		public string IPeopleNumber { get; set; }

		[DataMember]
		public string GivenName { get; set; }

		[DataMember]
		public string FamilyName { get; set; }

		[DataMember]
		public DateTime? DateOfBirth { get; set; }

		[DataMember]
		public bool IsAssistanceRequired{ get; set; }

        [DataMember]
        public PatientSex Sex { get; set; }

        [DataMember]
        public string AssistanceDescription { get; set; }

        public override string ToString()
        {
            return String.Format("{0} {1} {2}", IPeopleNumber, GivenName, FamilyName);
        }
    }

    [Serializable]
    [DataContract(Name = "PatientSex", Namespace = "http://schemas.cloudmaster.ie/2012/01/WCS/1")]
    public enum PatientSex
    {
        [EnumMember]
        Male,
        [EnumMember]
        Female
    }

	[Serializable]
	[DataContract(Namespace = "http://schemas.cloudmaster.ie/2012/01/WCS/1")]
	public class Device
	{
		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string OS { get; set; }

		[DataMember]
		public string Location { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public string LastIPAddress { get; set; }

		[DataMember]
		public DateTime? LastConnectionDateTime { get; set; }
	}

	[Serializable]
	[DataContract(Namespace = "http://schemas.cloudmaster.ie/2012/01/WCS/1")]
	public class TrackingReport
	{
		[DataMember]
		public DateTime StartTime { get; set; }

		[DataMember]
		public int PeriodInMinutes { get; set; }

		[DataMember]
		public IEnumerable<TrackingReportInterval> Intervals { get; set; }
	}

	[Serializable]
	[DataContract(Namespace = "http://schemas.cloudmaster.ie/2012/01/WCS/1")]
	public class TrackingReportInterval
	{
		[DataMember]
		public DateTime Time { get; set; }

		[DataMember]
		public int Created { get; set; }

		[DataMember]
		public int Assigned { get; set; }

		[DataMember]
		public int Arrived { get; set; }

		[DataMember]
		public int Started { get; set; }

		[DataMember]
		public int Completed { get; set; }
	}

	[Serializable]
	[DataContract(Namespace = "http://schemas.cloudmaster.ie/2012/01/WCS/1")]
	public class Room
	{
        [DataMember]
        public int RoomId { get; set; }

        [DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Ward { get; set; }
	}

	[Serializable]
	[DataContract(Namespace = "http://schemas.cloudmaster.ie/2012/01/WCS/1")]
	public class Bed
	{
		[DataMember]
		public int BedId { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public Room Room { get; set; }

		[DataMember]
		public BedStatus CurrentStatus { get; set; }

        [DataMember]
        public DateTime? EstimatedDischargeDate { get; set; }

        [DataMember]
        public CriticalCareIndicators CriticalCareIndicators { get; set; }

        [DataMember]
		public Location Location { get; set; }

		[DataMember]
        public List<CleaningService> Services { get; set; }

		[DataMember]
		public CleaningService LatestService { get; set; }

		[DataMember]
        public List<Note> Notes { get; set; }

        [DataMember]
        public List<BedTime> AvailableTimes { get; set; }

        public override string ToString()
        {
            return String.Format("{0} {1} {2}", Room.Name, Name, CurrentStatus);
        }
	}

    [Serializable]
    [DataContract(Namespace = "http://schemas.cloudmaster.ie/2012/01/WCS/1")]
    public class BedDischargeData
    {
        [DataMember]
        public int BedId { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public Location Location { get; set; }

        [DataMember]
        public Room Room { get; set; }

        [DataMember]
        public Admission CurrentAdmission { get; set; }

        [DataMember]
        public DateTime? EstimatedDischargeDateLastUpdated { get; set; }

        public override string ToString()
        {
            return String.Format("{0} {1} {2}", Room.Name, Name, CurrentAdmission != null ? CurrentAdmission.DisplayCode : "Empty");
        }
    }

    [Serializable]
	[DataContract(Namespace = "http://schemas.cloudmaster.ie/2012/01/WCS/1")]
	public class BedTime
	{
		[DataMember]
		public int BedId { get; set; }

		[DataMember]
		public DateTime StartTime { get; set; }

		[DataMember]
		public DateTime EndTime { get; set; }

        [DataMember]
        public bool IsDueToDischarge { get; set; }
    }

	[Serializable]
	[DataContract(Namespace = "http://schemas.cloudmaster.ie/2012/01/WCS/1")]
	public class CleaningService
	{
		[DataMember]
		public int CleaningServiceId { get; set; }

		[DataMember]
		public DateTime? ServiceTime { get; set; }

		[DataMember]
		public BedCleaningEventType CleaningServiceType { get; set; }
	}


	[Serializable]
	[DataContract(Name = "BedStatus", Namespace = "http://schemas.cloudmaster.ie/2012/01/WCS/1")]
	public enum BedStatus
	{
		[EnumMember]
		Clean,
		[EnumMember]
		Dirty,
		[EnumMember]
		RequiresDeepClean
	}

    [Serializable]
    [DataContract(Name = "BedCleaningEventType", Namespace = "http://schemas.cloudmaster.ie/2012/01/WCS/1")]
    public enum BedCleaningEventType
    {
        [EnumMember]
        BedCleaned,
        [EnumMember]
        BedMarkedAsDirty
    }

    [CollectionDataContract(ItemName = "code")]
    public class LocationCodes : List<string> { }

    [CollectionDataContract(ItemName = "code")]
    public class PatientCodes : List<string> { }

	[Serializable]
	[DataContract(Namespace = "http://schemas.cloudmaster.ie/2012/01/WCS/1")]
	public class DetectionLocation
	{
		[DataMember]
		public int DetectorId { get; set; }

        [DataMember]
        public int LocationId { get; set; }

        [DataMember]
        public string LocationName { get; set; }

        [DataMember]
        public string LocationCode { get; set; }
	}

	[Serializable]
	[DataContract(Namespace = "http://schemas.cloudmaster.ie/2012/01/WCS/1")]
	public class Detection
	{
		[DataMember]
		public int DetectionId { get; set; }

		[DataMember]
		public int PatientId { get; set; }

		[DataMember]
        public DetectionDirection Direction { get; set; }

		[DataMember]
		public DateTime Timestamp { get; set; }

		[DataMember]
        public DetectionLocation DetectionLocation { get; set; }
	}

	[Serializable]
	[DataContract(Name = "DetectionDirection", Namespace = "http://schemas.cloudmaster.ie/2012/01/WCS/1")]
	public enum DetectionDirection
	{
		[EnumMember]
		In,
		[EnumMember]
		Out
	}

	[Serializable]
	[DataContract(Name = "NotificationType", Namespace = "http://schemas.cloudmaster.ie/2012/01/WCS/1")]
	public enum NotificationType
	{
		[EnumMember]
		Prep,
		[EnumMember]
		Physio
	}

    [Serializable]
    [DataContract(Namespace = "http://schemas.cloudmaster.ie/2012/01/WCS/1")]
    public class AdmissionsData
    {
        [DataMember]
        public IList<BedDischargeData> Beds { get; set; }

        [DataMember]
        public IList<Admission> Admissions { get; set; }
    }
}

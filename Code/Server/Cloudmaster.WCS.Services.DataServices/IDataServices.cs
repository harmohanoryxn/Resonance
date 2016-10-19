using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WCS.Services.DataServices
{
	[ServiceContract(Namespace = "http://schemas.cloudmaster.ie/2012/01/WCS/1")]
	public interface IDataServices
	{
		#region Security

		[OperationContract]
		AuthenticationToken Authenticate(string pin);

		[OperationContract]
		DateTime GetTimestamp();

		#endregion

		#region Configurations

		[OperationContract]
		DeviceConfiguration GetConfigurations(string device);

		[OperationContract]
        PollingTimeouts GetLatestPollingTimeouts(string device, int shortcutKeyNo);
		#endregion

		#region Devices

		[OperationContract]
        IEnumerable<Presence> GetPresenceData();

		#endregion

        #region Orders

        [OperationContract]
        IEnumerable<Order> GetOrders(DateTime fromDate, DateTime toDate, LocationCodes locations);

        [OperationContract]
        Order AcknowledgeOrder(int orderId);

        [OperationContract]
        Order HideOrder(int orderId);

        [OperationContract]
        Order UnhideOrder(int orderId);

        [OperationContract]
        Order UpdateProcedureTime(int orderId, DateTime procedureTime);

        [OperationContract]
        Order AddOrderNote(int orderId, string note);

        [OperationContract]
        Order DeleteOrderNote(int orderId, int noteId);

        #endregion

		#region Notifications

		[OperationContract]
		Order AcknowledgeNotification(int notificationId);

		#endregion

		#region Cleaning

		[OperationContract]
        IEnumerable<Bed> GetCleaningBedData(LocationCodes locations);

        [OperationContract]
        Bed MarkBedAsClean(int bedId);

        [OperationContract]
        Bed MarkBedAsDirty(int bedId);

        [OperationContract]
        Bed AddBedNote(int bedId, string note);

        [OperationContract]
        Bed DeleteBedNote(int bedId, int noteId);

		#endregion

		#region RFID

        [OperationContract]
        IList<Detection> GetEntireDetectionHistory(DateTime fromDate, DateTime toDate, PatientCodes patientCodes);

        [OperationContract]
        IList<Detection> GetLatestDetection(DateTime fromDate, DateTime toDate, PatientCodes patientCodes);

        [OperationContract]
        void RandomlyMovePatient();

		#endregion

		#region Discharges

		[OperationContract]
		IList<BedDischargeData> GetDischargesBedData(LocationCodes locations);

		[OperationContract]
		BedDischargeData UpdateEstimatedDischargeDate(int admissionId, DateTime estimatedDischargeDate);

		#endregion

        #region Admissions

        [OperationContract]
        AdmissionsData GetAdmissionsData(LocationCodes locations);

        #endregion
    }
}

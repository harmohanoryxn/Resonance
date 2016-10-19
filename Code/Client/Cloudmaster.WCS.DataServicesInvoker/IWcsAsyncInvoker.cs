using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using WCS.Core;

namespace Cloudmaster.WCS.DataServicesInvoker
{
	public interface IWcsAsyncInvoker
	{
        // Security
        void AuthenticateAsync(string pin, Action<AuthenticationToken> setResultsCallback);

        // Configurations
        void GetConfigurationsAsync(string device, Action<DeviceConfiguration> setResultsCallback);
		void GetLatestPollingTimeoutsAsync(int shortcut, Action<PollingTimeouts> setResultsCallback);
        
        // Devices
        void GetPresenceDataAsync(Action<IList<Presence>> setResultsCallback);

        // Orders
        void GetOrdersAsync(DateTime fromDate, DateTime toDate, LocationCodes locations, Action<IList<Order>> setResultsCallback);
		void AcknowledgeOrderAsync(int orderId, Action<Order> setResultsCallback);
        void HideOrderAsync(int orderId, Action<Order> setResultsCallback);
        void UnhideOrderAsync(int orderId, Action<Order> setResultsCallback);
        void UpdateProcedureTimeAsync(int id, TimeSpan procedureTime, Action<Order> setResultsCallback);
		void AddOrderNoteAsync(int orderId, string note, Action<Order> setResultsCallback);
		void DeleteOrderNoteAsync(int orderId, int noteId, Action<Order> setResultsCallback);

        // Notifications
        void AcknowledgeNotificationAsync(int notificationId, Action<Order> setResultsCallback);

        // Cleaning
        void GetCleaningBedDataAsync(LocationCodes locations, Action<IList<Bed>> setResultsCallback);
        void MarkBedAsCleanAsync(int bedId, Action<Bed> setResultsCallback);
        void MarkBedAsDirtyAsync(int bedId, Action<Bed> setResultsCallback);
        void AddBedNoteAsync(int orderId, string note, Action<Bed> setResultsCallback);
		void DeleteBedNoteAsync(int bedId, int noteId, Action<Bed> setResultsCallback);

		// Discharge
		void GetDischargesBedDataAsync(LocationCodes locations, Action<IList<BedDischargeData>> setResultsCallback);
		void UpdateEstimatedDischargeDateAsync(int admissionId, DateTime estimatedDischargeDate, Action<BedDischargeData> setResultsCallback);

		// Admissions
		void GetAdmissionDataAsync(LocationCodes locations, Action<AdmissionsData> setResultsCallback);
	
		// RFID
        void GetEntireDetectionHistoryAsync(DateTime fromDate, DateTime toDate, PatientCodes patientCodes, Action<IList<Detection>> setResultsCallback);
        void GetLatestDetectionAsync(DateTime fromDate, DateTime toDate, PatientCodes patientCodes, Action<IList<Detection>> setResultsCallback);
        void RandomlyMovePatientAsync();
	}
}

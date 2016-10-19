using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq; 
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using WCS.Core;
using WCS.Core.Composition;

namespace Cloudmaster.WCS.DataServicesInvoker
{
	/// <summary>
	/// Does all the interaction with the server
	/// </summary>
 	[Export(typeof(IWcsAsyncInvoker))]
	[PartCreationPolicy(CreationPolicy.Shared)]
	public class WcsAsyncInvoker : IWcsAsyncInvoker
	{
		public IWcsExceptionHandler ExceptionHandler { get; set; }
		public IWcsClientLogger ClientLogger { get; set; }
		private IDeviceIdentity Device { get; set; }

		[ImportingConstructorAttribute]
		public WcsAsyncInvoker(IWcsExceptionHandler exceptionHandler, IWcsClientLogger logger, [Import("IDeviceIdentity", typeof(IDeviceIdentity))]  IDeviceIdentity device)
		{
			ExceptionHandler = exceptionHandler;
			ClientLogger = logger;
			Device = device;
		}

		public void CheckServerIsAlive()
		{
			new AsyncInvoker(ExceptionHandler, ClientLogger).Execute("CheckServerIsAlive",
				() =>
				{
					using (var dataServices = new DataServiceClientWithDisposeFix(Device))
					{
						dataServices.GetTimestamp();
					}
				}, CheckServerIsAlive);
		}

		/// <summary>
		/// Gets the server timestamp
		/// </summary>
		/// <remarks>
		/// This CANNOT by async
		/// </remarks>
		/// <returns></returns>
		public DateTime GetTimestamp()
		{
			return new Invoker<DateTime>(ExceptionHandler, ClientLogger).Execute("GetTimestamp",
				() =>
				{
					using (var dataServices = new DataServiceClientWithDisposeFix(Device))
					{
						return dataServices.GetTimestamp();
					}
				}, CheckServerIsAlive);

		}

		public void AuthenticateAsync(string pin, Action<AuthenticationToken> setResultsCallback)
		{
			new AsyncInvoker<AuthenticationToken>(ExceptionHandler, ClientLogger).Execute("AuthenticateAsync",
				() =>
				{
					using (var dataServices = new DataServiceClientWithDisposeFix(Device))
					{
						return dataServices.Authenticate(pin);
					}
				}, setResultsCallback, CheckServerIsAlive);
		}

		public void GetConfigurationsAsync(string device, Action<DeviceConfiguration> setResultsCallback)
		{
			new AsyncInvoker<DeviceConfiguration>(ExceptionHandler, ClientLogger).Execute("GetConfigurationsAsync",
				() =>
				{
					using (var dataServices = new DataServiceClientWithDisposeFix(Device))
					{
						return dataServices.GetConfigurations(device);
					}
				}, setResultsCallback, CheckServerIsAlive);
		}

		public void GetLatestPollingTimeoutsAsync(int shortcut, Action<PollingTimeouts> setResultsCallback)
		{
			new AsyncInvoker<PollingTimeouts>(ExceptionHandler, ClientLogger).Execute("GetLatestPollingTimeoutsAsync",
				() =>
				{
					using (var dataServices = new DataServiceClientWithDisposeFix(Device))
					{
						return dataServices.GetLatestPollingTimeouts(Device.DeviceName, shortcut);
					}
				}, setResultsCallback, CheckServerIsAlive);
		}


        public void GetPresenceDataAsync(Action<IList<Presence>> setResultsCallback)
		{
            new AsyncInvoker<IList<Presence>>(ExceptionHandler, ClientLogger).Execute("GetPresenceDataAsync",
				() =>
				{
					using (var dataServices = new DataServiceClientWithDisposeFix(Device))
					{
                        return dataServices.GetPresenceData();
					}
				}, setResultsCallback, CheckServerIsAlive);
		}

		public void GetOrdersAsync(DateTime fromDate, DateTime toDate, LocationCodes locations, Action<IList<Order>> setResultsCallback)
		{
			new AsyncInvoker<IList<Order>>(ExceptionHandler, ClientLogger).Execute("GetOrdersAsync",
				() =>
				{
					using (var dataServices = new DataServiceClientWithDisposeFix(Device))
					{
                        return dataServices.GetOrders(fromDate, toDate, locations);
					}
				}, setResultsCallback, CheckServerIsAlive);
		}

		public void AcknowledgeOrderAsync(int orderId, Action<Order> setResultsCallback)
		{
			new AsyncInvoker<Order>(ExceptionHandler, ClientLogger).Execute("AcknowledgeOrderAsync",
				() =>
				{
					using (var dataServices = new DataServiceClientWithDisposeFix(Device))
					{
						return dataServices.AcknowledgeOrder(orderId);
					}
				}, setResultsCallback, CheckServerIsAlive);
		}
	

		public void UpdateProcedureTimeAsync(int id, TimeSpan procedureTime, Action<Order> setResultsCallback)
		{
			var dt = DateTime.Today;
			UpdateProcedureTimeAsync(id, new DateTime(dt.Year, dt.Month, dt.Day, procedureTime.Hours, procedureTime.Minutes, 0), setResultsCallback);

		}


		public void UpdateProcedureTimeAsync(int id, DateTime procedureTime, Action<Order> setResultsCallback)
		{
			Sound.UpdateSent.Play();

			new AsyncInvoker<Order>(ExceptionHandler, ClientLogger).Execute("UpdateProcedureTimeAsync",
				() =>
				{
					using (var dataServices = new DataServiceClientWithDisposeFix(Device))
					{
						return dataServices.UpdateProcedureTime(id, procedureTime);
					}
				}, setResultsCallback, CheckServerIsAlive);
		}

		public void HideOrderAsync(int orderId, Action<Order> setResultsCallback)
		{
			new AsyncInvoker<Order>(ExceptionHandler, ClientLogger).Execute("HideOrderAsync",
				() =>
				{
					using (var dataServices = new DataServiceClientWithDisposeFix(Device))
					{
						return dataServices.HideOrder(orderId);
					}
				}, setResultsCallback, CheckServerIsAlive);
		}

		public void UnhideOrderAsync(int orderId, Action<Order> setResultsCallback)
		{
			new AsyncInvoker<Order>(ExceptionHandler, ClientLogger).Execute("UnhideOrderAsync",
				() =>
				{
					using (var dataServices = new DataServiceClientWithDisposeFix(Device))
					{
						return dataServices.UnhideOrder(orderId);
					}
				}, setResultsCallback, CheckServerIsAlive);
		}
		
		public void AddOrderNoteAsync(int orderId, string note, Action<Order> setResultsCallback)
		{
			Sound.UpdateSent.Play();

			new AsyncInvoker<Order>(ExceptionHandler, ClientLogger).Execute("AddOrderNoteAsync",
				() =>
				{
					using (var dataServices = new DataServiceClientWithDisposeFix(Device))
					{
						return dataServices.AddOrderNote(orderId, note);
					}
				}, setResultsCallback, CheckServerIsAlive);
		}

		public void AddBedNoteAsync(int orderId, string note, Action<Bed> setResultsCallback)
		{
			Sound.UpdateSent.Play();

			new AsyncInvoker<Bed>(ExceptionHandler, ClientLogger).Execute("AddBedNoteAsync",
				() =>
				{
					using (var dataServices = new DataServiceClientWithDisposeFix(Device))
					{
						return dataServices.AddBedNote(orderId, note);
					}
				}, setResultsCallback, CheckServerIsAlive);
		}

		public void DeleteOrderNoteAsync(int orderId, int noteId, Action<Order> setResultsCallback)
		{
            new AsyncInvoker<Order>(ExceptionHandler, ClientLogger).Execute("DeleteOrderNoteAsync",
				() =>
				{
					using (var dataServices = new DataServiceClientWithDisposeFix(Device))
					{
						return dataServices.DeleteOrderNote(orderId, noteId);
					}
				}, setResultsCallback, CheckServerIsAlive);
		}

		public void DeleteBedNoteAsync(int bedId, int noteId, Action<Bed> setResultsCallback)
		{
			new AsyncInvoker<Bed>(ExceptionHandler, ClientLogger).Execute("DeleteBedNoteAsync",
				() =>
				{
					using (var dataServices = new DataServiceClientWithDisposeFix(Device))
					{
						return dataServices.DeleteBedNote(bedId, noteId);
					}
				}, setResultsCallback, CheckServerIsAlive);
		}

		public void AcknowledgeNotificationAsync(int notificationId, Action<Order> setResultsCallback)
		{
			Sound.UpdateSent.Play();

			new AsyncInvoker<Order>(ExceptionHandler, ClientLogger).Execute("AcknowledgeNotificationAsync",
				() =>
				{
					using (var dataServices = new DataServiceClientWithDisposeFix(Device))
					{
						return dataServices.AcknowledgeNotification(notificationId);
					}
				}, setResultsCallback, CheckServerIsAlive);

		}

        public void GetCleaningBedDataAsync(LocationCodes locations, Action<IList<Bed>> setResultsCallback)
		{
            new AsyncInvoker<IList<Bed>>(ExceptionHandler, ClientLogger).Execute("GetCleaningBedDataAsync",
				() =>
				{
					using (var dataServices = new DataServiceClientWithDisposeFix(Device))
					{
                        return dataServices.GetCleaningBedData(locations);
					}
				}, setResultsCallback, CheckServerIsAlive);

		}

        public void MarkBedAsCleanAsync(int bedId, Action<Bed> setResultsCallback)
		{
			new AsyncInvoker<Bed>(ExceptionHandler, ClientLogger).Execute("MarkBedAsCleanAsync",
				() =>
				{
					using (var dataServices = new DataServiceClientWithDisposeFix(Device))
					{
						return dataServices.MarkBedAsClean(bedId);
					}
				}, setResultsCallback, CheckServerIsAlive);

		}

        public void MarkBedAsDirtyAsync(int bedId, Action<Bed> setResultsCallback)
		{
			new AsyncInvoker<Bed>(ExceptionHandler, ClientLogger).Execute("MarkBedAsDirtyAsync",
				() =>
				{
					using (var dataServices = new DataServiceClientWithDisposeFix(Device))
					{
						return dataServices.MarkBedAsDirty(bedId);
					}
				}, setResultsCallback, CheckServerIsAlive);

		}

        public void GetLatestDetectionAsync(DateTime fromDate, DateTime toDate, PatientCodes patientCodes, Action<IList<Detection>> setResultsCallback)
        {
            new AsyncInvoker<IList<Detection>>(ExceptionHandler, ClientLogger).Execute("GetLatestDetectionAsync",
                () =>
                {
                    using (var dataServices = new DataServiceClientWithDisposeFix(Device))
                    {
                        return dataServices.GetLatestDetection(fromDate, toDate, patientCodes);
                    }
                }, setResultsCallback, CheckServerIsAlive);
        }

        public void GetEntireDetectionHistoryAsync(DateTime fromDate, DateTime toDate, PatientCodes patientCodes, Action<IList<Detection>> setResultsCallback)
        {
            new AsyncInvoker<IList<Detection>>(ExceptionHandler, ClientLogger).Execute("GetEntireDetectionHistoryAsync",
                () =>
                {
                    using (var dataServices = new DataServiceClientWithDisposeFix(Device))
                    {
                        return dataServices.GetEntireDetectionHistory(fromDate, toDate, patientCodes);
                    }
                }, setResultsCallback, CheckServerIsAlive);
        }

		public void GetDischargesBedDataAsync(LocationCodes locations, Action<IList<BedDischargeData>> setResultsCallback)
		{
			new AsyncInvoker<IList<BedDischargeData>>(ExceptionHandler, ClientLogger).Execute("GetDischargesBedDataAsync",
				() =>
				{
					using (var dataServices = new DataServiceClientWithDisposeFix(Device))
					{
						return dataServices.GetDischargesBedData(locations);
					}
				}, setResultsCallback, CheckServerIsAlive);

		}

		public void UpdateEstimatedDischargeDateAsync(int admissionId, DateTime estimatedDischargeDate, Action<BedDischargeData> setResultsCallback)
		{
			new AsyncInvoker<BedDischargeData>(ExceptionHandler, ClientLogger).Execute("UpdateEstimatedDischargeDateAsync",
				() =>
				{
					using (var dataServices = new DataServiceClientWithDisposeFix(Device))
					{
						return dataServices.UpdateEstimatedDischargeDate(admissionId, estimatedDischargeDate);
					}
				}, setResultsCallback, CheckServerIsAlive);

		}

		public void RandomlyMovePatientAsync()
		{
            new AsyncInvoker(ExceptionHandler, ClientLogger).Execute("RandomlyMovePatientAsync",
				() =>
				{
					using (var dataServices = new DataServiceClientWithDisposeFix(Device))
					{
						dataServices.RandomlyMovePatient();
					}
				}, CheckServerIsAlive);
		}


		public void GetAdmissionDataAsync(LocationCodes locations, Action<AdmissionsData> setResultsCallback)
		{
			new AsyncInvoker<AdmissionsData>(ExceptionHandler, ClientLogger).Execute("GetAdmissionDataAsync",
				() =>
				{
					using (var dataServices = new DataServiceClientWithDisposeFix(Device))
					{
						return dataServices.GetAdmissionsData(locations);
					}
				}, setResultsCallback, CheckServerIsAlive);
		}
		 
	}
}
 
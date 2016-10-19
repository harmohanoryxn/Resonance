using System;
using System.Collections.Generic;
using System.ServiceModel;
using WCS.Core;
using WCS.Core.Instrumentation;
using WCS.Services.DataServices.Data;

namespace WCS.Services.DataServices
{
	[MessageLoggingBehavior]
	public class DataServices : IDataServices
	{
		private Logger _logger;
		private ServerFacade _wcs;
		private object _debugLock = new object();

		public DataServices()
		{
			_logger = new Logger("DataServices", true);
			_wcs = new ServerFacade();
		}

		#region Security

		public AuthenticationToken Authenticate(string pin)
		{
			try
			{
				string deviceName = OperationContext.Current.GetDeviceName();

				if (string.IsNullOrEmpty(deviceName))
					return new AuthenticationToken() { IsAuthenticated = false, LockScreenTimeout = 0, Message = string.Format("Device not recognised") };

				_logger.InfoFormat("{0} attempts for {0} to login", pin, deviceName);

				var authenticationToken = _wcs.Authenticate(deviceName, pin, DateTime.Now);
				if (!authenticationToken.IsAuthenticated)
				{
					_logger.WarnFormat("{0} failed to login", pin);

					authenticationToken.Message = string.Format("Logged failed for {0} at {1}", deviceName, DateTime.Now.ToWcsTimeFormat());
				}

				return authenticationToken;
			}
			catch (Exception ex)
			{
				_logger.Error(ex.BuildExceptionInfo());
				throw;
			}
		}

        public DateTime GetTimestamp()
        {
            return DateTime.Now;
        }

		#endregion

		#region Configurations

		public DeviceConfiguration GetConfigurations(string device)
		{
			try
			{
				return _wcs.GetConfigurations(device);
			}
			catch (Exception ex)
			{
				_logger.Error(ex.BuildExceptionInfo());
				throw;
			}
		}

        public PollingTimeouts GetLatestPollingTimeouts(string device, int shortcutKeyNo)
		{
			try
			{
                return _wcs.GetLatestPollingTimeouts(device, shortcutKeyNo);
			}
			catch (Exception ex)
			{
				_logger.Error(ex.BuildExceptionInfo());
				throw;
			}
		}

        #endregion

		#region Orders

        public IEnumerable<Order> GetOrders(DateTime fromDate, DateTime toDate, LocationCodes locations)
        {
            try
            {
                return _wcs.GetOrders(fromDate, toDate, locations, DateTime.Now);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.BuildExceptionInfo());
                throw;
            }
        }

		public Order AcknowledgeOrder(int orderId)
		{
			try
			{
				string deviceName = OperationContext.Current.GetDeviceName();

                return _wcs.AcknowledgeOrder(orderId, DateTime.Now, deviceName);
			 	}
			catch (Exception ex)
			{
				_logger.Error(ex.BuildExceptionInfo());
				throw;
			}
		}

        public Order HideOrder(int orderId)
        {
            try
            {
                string deviceName = OperationContext.Current.GetDeviceName();

                return _wcs.HideOrder(orderId, DateTime.Now, deviceName);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.BuildExceptionInfo());
                throw;
            }
        }

        public Order UnhideOrder(int orderId)
        {
            try
            {
                string deviceName = OperationContext.Current.GetDeviceName();

                return _wcs.UnhideOrder(orderId, DateTime.Now, deviceName);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.BuildExceptionInfo());
                throw;
            }
        }

		#endregion
		 
		#region Tracking

		public Order UpdateProcedureTime(int orderId, DateTime procedureTime)
		{
			try
			{
				string deviceName = OperationContext.Current.GetDeviceName();

                return _wcs.UpdateProcedureTime(orderId, procedureTime, DateTime.Now, deviceName);
			}
			catch (Exception ex)
			{
				_logger.Error(ex.BuildExceptionInfo());
				throw;
			}
		}

		#endregion

		#region Notes

		public Order AddOrderNote(int orderId, string note)
		{
			try
			{
				string deviceName = OperationContext.Current.GetDeviceName();

                return _wcs.AddOrderNote(orderId, note, DateTime.Now, deviceName);
			}
			catch (Exception ex)
			{
				_logger.Error(ex.BuildExceptionInfo());
				throw;
			}
		}

		public Bed AddBedNote(int bedId, string note)
		{
			try
			{
				string deviceName = OperationContext.Current.GetDeviceName();

                return _wcs.AddBedNote(bedId, note, DateTime.Now, deviceName);
			 		}
			catch (Exception ex)
			{
				_logger.Error(ex.BuildExceptionInfo());
				throw;
			}
		}

		public Order DeleteOrderNote(int orderId, int noteId)
		{
			try
			{
				string deviceName = OperationContext.Current.GetDeviceName();

                return _wcs.DeleteOrderNote(noteId, DateTime.Now, deviceName);
			 	}
			catch (Exception ex)
			{
				_logger.Error(ex.BuildExceptionInfo());
				throw;
			}
		}

		public Bed DeleteBedNote(int bedId, int noteId)
		{
			try
			{
				string deviceName = OperationContext.Current.GetDeviceName();

                return _wcs.DeleteBedNote(noteId, DateTime.Now, deviceName);
		 		}
			catch (Exception ex)
			{
				_logger.Error(ex.BuildExceptionInfo());
				throw;
			}
		}

		#endregion

		#region Devices

        public IEnumerable<Presence> GetPresenceData()
        {
            try
            {
                return _wcs.GetPresenceData(DateTime.Now);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.BuildExceptionInfo());
                throw;
            }
        }

		#endregion

		#region Notifications

		public Order AcknowledgeNotification(int notificationId)
		{
			try
			{
				string deviceName = OperationContext.Current.GetDeviceName();

                return _wcs.AcknowledgeNotification(notificationId, DateTime.Now, deviceName);
		 		}
			catch (Exception ex)
			{
				_logger.Error(ex.BuildExceptionInfo());
				throw;
			}
		}

		#endregion

		#region Cleaning

        public IEnumerable<Bed> GetCleaningBedData(LocationCodes locations)
		{
			try
			{
                return _wcs.GetCleaningBedData(locations, DateTime.Now);
			}
			catch (Exception ex)
			{
				_logger.Error(ex.BuildExceptionInfo());
				throw;
			}
		}

        public Bed MarkBedAsClean(int bedId)
		{
			try
			{
				string deviceName = OperationContext.Current.GetDeviceName();
                return _wcs.MarkBedAsClean(bedId, DateTime.Now, deviceName);
			}
			catch (Exception ex)
			{
				_logger.Error(ex.BuildExceptionInfo());
				throw;
			}
		}

        public Bed MarkBedAsDirty(int bedId)
		{
			try
			{
				string deviceName = OperationContext.Current.GetDeviceName();
                return _wcs.MarkBedAsDirty(bedId, DateTime.Now, deviceName);
			}
			catch (Exception ex)
			{
				_logger.Error(ex.BuildExceptionInfo());
				throw;
			}
		}
 
		#endregion

		#region RFID

        public IList<Detection> GetEntireDetectionHistory(DateTime fromDate, DateTime toDate, PatientCodes patientCodes)
		{
			try
			{
                return _wcs.GetEntireDetectionHistory(fromDate, toDate, patientCodes);
			}
			catch (Exception ex)
			{
				_logger.Error(ex.BuildExceptionInfo());
				throw;
			}
		}

        public IList<Detection> GetLatestDetection(DateTime fromDate, DateTime toDate, PatientCodes patientCodes)
		{
			try
			{
                return _wcs.GetLatestDetection(fromDate, toDate, patientCodes);
			}
			catch (Exception ex)
			{
				_logger.Error(ex.BuildExceptionInfo());
				throw;
			}
		}

		public void RandomlyMovePatient()
		{
			try
			{
				  _wcs.RandomlyMovePatient(DateTime.Now);
			}
			catch (Exception ex)
			{
				_logger.Error(ex.BuildExceptionInfo());
				throw;
			}
		}
		#endregion

        #region Discharges

        public IList<BedDischargeData> GetDischargesBedData(LocationCodes locations)
        {
            try
            {
                return _wcs.GetDischargesBedData(locations, DateTime.Now);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.BuildExceptionInfo());
                throw;
            }
        }

        public BedDischargeData UpdateEstimatedDischargeDate(int admissionId, DateTime estimatedDischargeDate)
        {
            try
            {
                string deviceName = OperationContext.Current.GetDeviceName();
                return _wcs.UpdateEstimatedDischargeDate(admissionId, estimatedDischargeDate, DateTime.Now, deviceName);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.BuildExceptionInfo());
                throw;
            }
        }

        #endregion

        #region Admissions

        public AdmissionsData GetAdmissionsData(LocationCodes locations)
        {
            try
            {
                string deviceName = OperationContext.Current.GetDeviceName();
                return _wcs.GetAdmissionsData(locations, DateTime.Now);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.BuildExceptionInfo());
                throw;
            }
        }

        #endregion
    }
}

using System;
using System.ServiceModel;
using WCS.Core;
using WCS.Core.Instrumentation;
using WCS.Services.DataServices.Data;

namespace WCS.Services.DataServices
{
    public class ExternalServices : IExternalServices
    {
        private Logger _logger;
        private ServerFacade _wcs;

        public ExternalServices()
        {
            _logger = new Logger("ExternalServices",true);
            _wcs = new ServerFacade();
        }

        #region RFID

        public Detection InsertDetection(string trackableIdSource, string trackableId, string locationSource, string location, DetectionDirection direction, DateTime timestamp)
        {
            try
            {
                return _wcs.InsertDetection(trackableIdSource, trackableId, locationSource, location, direction, timestamp.ToLocalTime());
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

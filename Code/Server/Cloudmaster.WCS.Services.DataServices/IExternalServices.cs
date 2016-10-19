using System;
using System.ServiceModel;

namespace WCS.Services.DataServices
{
    [ServiceContract(Namespace = "http://schemas.cloudmaster.ie/2012/01/WCS/1")]
    public interface IExternalServices
    {
        [OperationContract]
        Detection InsertDetection(string trackableIdSource, string trackableId, string locationSource, string location, DetectionDirection direction, DateTime timestamp);
    }
}

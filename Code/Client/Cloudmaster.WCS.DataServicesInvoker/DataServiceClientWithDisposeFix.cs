using System;
using System.ServiceModel;
using WCS.Core.Composition;

namespace Cloudmaster.WCS.DataServicesInvoker
{
    public class DataServiceClientWithDisposeFix : DataServices.DataServicesClient, IDisposable
    {
		private IDeviceIdentity Device { get; set; }

		public DataServiceClientWithDisposeFix(IDeviceIdentity device)
		{
			Device = device;
		
			var behavior = Endpoint.Behaviors.Find<AddWcsHeaderBehavior>();

			if (behavior == null)
			{
				Endpoint.Behaviors.Add(new AddWcsHeaderBehavior(Device));
			}
		}
        #region IDisposable Members

        void IDisposable.Dispose()
        {
			if (this.State == CommunicationState.Faulted)
			{
				this.Abort();
			}
			else if (this.State != CommunicationState.Closed)
			{
				this.Close();
			}
        }

        #endregion
    }
}

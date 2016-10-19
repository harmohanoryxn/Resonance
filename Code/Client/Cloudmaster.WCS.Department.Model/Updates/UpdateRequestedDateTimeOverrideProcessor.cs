using System;
using System.ComponentModel;
using Cloudmaster.WCS.Classes;
using Cloudmaster.WCS.IO;
using Cloudmaster.WCS.Processing;

namespace Cloudmaster.WCS.Department.Processing
{
    public class UpdateRequestedDateTimeOverrideProcessor : ProcessorBaseClass
    {
        Order orderBeingUpdated;

        public void ExecuteInBackground(Order order)
        {
            if (!order.IsUpdatingServer)
            {
                orderBeingUpdated = order;

                orderBeingUpdated.IsUpdatingServer = true;

                UpdateRequestedDateTimeOverrideArguements arguements = new UpdateRequestedDateTimeOverrideArguements()
                {
                    OrderNumber = order.PlaceOrderId,
                    RequestedDateTimeOverride = order.Metadata.RequestedDateTimeOverride,
                    Notes = order.Metadata.Notes,
                    IAdmissionsProviderConnectionString = BaseProcessorViewModel.AdmissionsProviderConnectionString,
                };

                backgroundWorker.RunWorkerAsync(arguements);
            }
        }

        protected override void OnDoWork(DoWorkEventArgs e)
        {
            e.Result = new ProcessingResults() { FatalErrorOccured = false };

            UpdateRequestedDateTimeOverrideArguements arguements = (UpdateRequestedDateTimeOverrideArguements)e.Argument;

            try
            {
                IAdmisssionManager admisssionManager = InformationProviders.GetAdmissionsManager(arguements.IAdmissionsProviderConnectionString);

                admisssionManager.UpdateRequestedDateTimeOverride(arguements.OrderNumber, arguements.RequestedDateTimeOverride, arguements.Notes);
            }
            catch (Exception ex)
            {
                e.Result = new ProcessingResults() { FatalErrorOccured = true };
            }
        }

        protected override void OnRunWorkerCompleted(RunWorkerCompletedEventArgs e)
        {
            ProcessingResults results = (ProcessingResults) e.Result;

            orderBeingUpdated.IsUpdatingServer = false;

            if (!results.FatalErrorOccured)
            {
                orderBeingUpdated.IsUserModified = false;
                orderBeingUpdated.IsUserModifiedComplete = false;
            }
            else
            {
                orderBeingUpdated.HasErrorOccuredOnServerUpdate = true;
            }

            DepartmentProcessor.Instance.RefreshOrders();
        }
    }
}

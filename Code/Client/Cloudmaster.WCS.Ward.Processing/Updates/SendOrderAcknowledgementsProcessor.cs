using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Windows;
using Cloudmaster.WCS.Classes;
using Cloudmaster.WCS;
using System.Net;
using Cloudmaster.WCS.IO;
using Cloudmaster.WCS.Processing;

namespace Cloudmaster.WCS.Ward.Processing
{
    public class SendOrderAcknowledgementsProcessor : ProcessorBaseClass
    {
        public void ExecuteInBackground(string orderNumber, bool isFastingAcknowledged, bool isPrepWorkAcknowledged, bool isExamAcknowledged, bool isInjectionAcknowledged)
        {
            SendOrderAcknowledgementsArguements arguements = new SendOrderAcknowledgementsArguements()
            {
                OrderNumber = orderNumber,
                IsFastingAcknowledged = isFastingAcknowledged,
                IsPrepWorkAcknowledged = isPrepWorkAcknowledged,
                IsExamAcknowledged = isExamAcknowledged,
                IAdmissionsProviderConnectionString = WardProcessorViewModel.AdmissionsProviderConnectionString,
                IsInjectionAcknowledged = isInjectionAcknowledged
                };

            backgroundWorker.RunWorkerAsync(arguements);
        }

        protected override void OnDoWork(DoWorkEventArgs e)
        {
            e.Result = new ProcessingResults() { FatalErrorOccured = false };

            SendOrderAcknowledgementsArguements arguements = (SendOrderAcknowledgementsArguements) e.Argument;

            try
            {
                IAdmisssionManager admisssionManager = InformationProviders.GetAdmissionsManager(arguements.IAdmissionsProviderConnectionString);

                admisssionManager.SendOrderAcknowledgements(arguements.OrderNumber, arguements.IsFastingAcknowledged, arguements.IsPrepWorkAcknowledged, arguements.IsExamAcknowledged, arguements.IsInjectionAcknowledged);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                e.Result = new ProcessingResults() { FatalErrorOccured = true };
            }
        }

        protected override void OnRunWorkerCompleted(RunWorkerCompletedEventArgs e)
        {
            ProcessingResults results = (ProcessingResults) e.Result;

            if (results.FatalErrorOccured)
            {
                // this.HasErrors = true;
            }

            WardProcessorViewModel.Instance.GetIguanaWardFeedsProcessor.ExecuteInBackround();
        }
    }
}

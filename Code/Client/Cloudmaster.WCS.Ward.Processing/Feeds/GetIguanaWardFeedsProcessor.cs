using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Cloudmaster.WCS.Classes;
using Cloudmaster.WCS.IO;
using Cloudmaster.WCS.Processing;
using Cloudmaster.WCS.Ward.Processing;
using Cloudmaster.WCS.Ward.Model;
using System.Windows.Input;

namespace Cloudmaster.WCS.Ward.Feeds
{
    public class GetIguanaWardFeedsProcessor : ProcessorBaseClass
    {
        private string WardIdentifier { get; set; }

        public GetIguanaWardFeedsProcessor(string wardIdentifier)
        {
            WardIdentifier = wardIdentifier;
        }

        public void ExecuteInBackround()
        {
            InitializeBackgroundWorker();

            GetIguanaWardFeedsArguements arguements = new GetIguanaWardFeedsArguements()
            {
                IAdmissionsProviderConnectionString = BaseProcessorViewModel.AdmissionsProviderConnectionString,
                WardIdentifier = WardIdentifier
                    };


            backgroundWorker.RunWorkerAsync(arguements);
        }

        protected override void OnDoWork(DoWorkEventArgs e)
        {
            GetIguanaWardFeedsArguements arguements = (GetIguanaWardFeedsArguements) e.Argument;

            ServerInformation serverInformation = new ServerInformation();

            ProcessingResults results = new ProcessingResults() { FatalErrorOccured = false };

            try
            {
                IAdmisssionManager admisssionManager = InformationProviders.GetAdmissionsManager(arguements.IAdmissionsProviderConnectionString);

                Floor ward = new Floor(Guid.NewGuid());

                IList<Order> ordersFeed = admisssionManager.GetOrdersForWardToday(arguements.WardIdentifier);

                backgroundWorker.ReportProgress(40);

                IList<OrderMetadata> orderMetadataFeed = admisssionManager.GetOrdersMetadataForWardToday(ward);

                backgroundWorker.ReportProgress(80);

                serverInformation.Orders = ordersFeed.ToObservableCollection();
                serverInformation.OrderMetadatas = orderMetadataFeed.ToObservableCollection();

                string temperaryFilename = Path.GetTempFileName();

                XmlTypeSerializer<ServerInformation>.SerializeAndOverwriteFile(serverInformation, temperaryFilename);

                results.Filename = temperaryFilename;

                backgroundWorker.ReportProgress(100);
            }
            catch (Exception ex)
            {
                results.FatalErrorOccured = true;

                ReportError(ex, 100, "Admissions Feed");
            }

            serverInformation = null;

            e.Result = results;
        }

        protected override void OnRunWorkerCompleted(RunWorkerCompletedEventArgs e)
        {
            ProcessingResults results = (ProcessingResults) e.Result;

            IsWorking = false;

            if ((!results.FatalErrorOccured) && (results.Filename != null))
            {
                string filename = results.Filename;

                ServerInformation serverInformation = XmlTypeSerializer<ServerInformation>.Deserialize(filename);

                CleanUpTemporaryFile(filename);

                WardModel.Instance.Feeds.Orders.Synchronise(serverInformation.Orders);
                WardModel.Instance.Feeds.Orders.SyncronizeMetadata(serverInformation.OrderMetadatas);

                serverInformation = null;

                WardModel.Instance.Alerts.UpdateAlerts();

                WardModel.Instance.WardLabels.UpdateOrderStatus();

                CommandManager.InvalidateRequerySuggested();

                // Update Local Storage

                //SaveXml(serverInformation, "Temp", "server.iguana.xml");
            }

            DisposeOfBackgroundWorker();
        }
    }
}

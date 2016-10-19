using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Cloudmaster.WCS.Classes;
using Cloudmaster.WCS.IO;
using Cloudmaster.WCS.Processing;
using Cloudmaster.WCS.Department.Model;

namespace Cloudmaster.WCS.Department.Processing.Feeds
{
    public class OriginalGetIguanaFeedsProcessor : ProcessorBaseClass
    {
        public void ExecuteInBackround()
        {
            GetIguanaFeedsProcessorArguements arguements = new GetIguanaFeedsProcessorArguements()
            {
                IFormManagerConnectionString = BaseProcessorViewModel.FormManagerConnectionString,
                ITaskManagerConnectionString = BaseProcessorViewModel.TaskManagerConnectionString,
                IRoomManagerConnectionString = BaseProcessorViewModel.RoomManagerConnectionString,
                IAdmissionsProviderConnectionString = BaseProcessorViewModel.AdmissionsProviderConnectionString,
                IFileManagerImagesConnectionString = BaseProcessorViewModel.FileManagerImagesConnectionString
                    };

            if (!IsWorking)
            {
                IsWorking = true;

                backgroundWorker.RunWorkerAsync(arguements);
            }
        }

        protected override void OnDoWork(DoWorkEventArgs e)
        {
            ProcessorArguements arguements = (ProcessorArguements) e.Argument;

            ServerInformation serverInformation = new ServerInformation();

            ProcessingResults results = new ProcessingResults() { FatalErrorOccured = false };

            IAdmisssionManager admisssionManager;

            try
            {
                admisssionManager = InformationProviders.GetAdmissionsManager(arguements.IAdmissionsProviderConnectionString);

                Floor ward = new Floor(Guid.NewGuid());

                IList<Order> ordersFeed = admisssionManager.GetOrdersForToday();

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
            admisssionManager = null;

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
                
                DepartmentModel.Instance.Feeds.Orders.Syncronize(serverInformation.Orders);
                DepartmentModel.Instance.Feeds.Orders.SyncronizeMetadata(serverInformation.OrderMetadatas);

                DepartmentModel.Instance.Alerts.UpdateAlerts();
                DepartmentModel.Instance.DepartmentLabels.UpdateLabels();

                DepartmentModel.Instance.DepartmentLabels.UpdateOrderStatus();

                // Update Local Storage

                SaveXml(serverInformation, "Temp", "server.iguana.xml");
            }
        }

        
    }
}

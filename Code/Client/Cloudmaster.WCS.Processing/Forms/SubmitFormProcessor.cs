using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.IO;
using Cloudmaster.WCS.IO;
using Cloudmaster.WCS.IO;
using Cloudmaster.WCS.Office.Excel;
using System.Xml;
using System.Windows;
using Cloudmaster.WCS.Classes;
using Cloudmaster.WCS;
using Cloudmaster.WCS.Office.SharePoint._2010;
using Cloudmaster.WCS.Office.Word;
using Cloudmaster.WCS.Model.LocalStroage;
using Cloudmaster.WCS.Processing;
using Cloudmaster.WCS.Model;

namespace Cloudmaster.WCS.Forms.Processing
{
    public class SubmitFormProcessor : ProcessorBaseClass
    {
        protected FormInstance formBeingProcessed;

        public void Submit(FormInstance form)
        {
            formBeingProcessed = form;

            formBeingProcessed.OutboxStatus = OutboxStatus.Sending;

            string temporaryFilename = Path.GetTempFileName();

            XmlTypeSerializer<FormInstance>.SerializeAndOverwriteFile(formBeingProcessed, temporaryFilename);

            SubmitFormArguements arguements = new SubmitFormArguements() {
                FormXmlFilename = temporaryFilename,
                IFormManagerConnectionString = BaseProcessorViewModel.FormManagerConnectionString,
                ITaskManagerConnectionString = BaseProcessorViewModel.TaskManagerConnectionString,
                IRoomManagerConnectionString = BaseProcessorViewModel.RoomManagerConnectionString,
                IAdmissionsProviderConnectionString = BaseProcessorViewModel.AdmissionsProviderConnectionString,
                IFileManagerImagesConnectionString = BaseProcessorViewModel.FileManagerImagesConnectionString
                };

            backgroundWorker.RunWorkerAsync(arguements);
        }

        protected override void OnDoWork(DoWorkEventArgs e)
        {
            ProcessingResults results = new ProcessingResults() { FatalErrorOccured = false };

            string temporaryFilename = Path.GetTempFileName();

            SubmitFormArguements arguements = (SubmitFormArguements)e.Argument;

            try
            {
                FormInstance form = XmlTypeSerializer<FormInstance>.Deserialize(arguements.FormXmlFilename);

                DoUpload(form, arguements, results);

                XmlTypeSerializer<FormInstance>.SerializeAndOverwriteFile(form, temporaryFilename);

                results.Filename = temporaryFilename;
            }
            catch (Exception ex)
            {
                results.FatalErrorOccured = true;
            }

            e.Result = results;
        }

        private void DoUpload(FormInstance form, SubmitFormArguements arguements, ProcessingResults results)
        {
            try
            {
                UploadImages(form, arguements.IFileManagerImagesConnectionString);
            }
            catch (Exception ex)
            {
                results.FatalErrorOccured = true;

                ReportError(ex, 30, "Images");

                return;
            }

            try
            {
                CreateTasks(form, arguements.ITaskManagerConnectionString, form.Metadata.EmployeeNo);
            }
            catch (Exception ex)
            {
                results.FatalErrorOccured = true;

                ReportError(ex, 60, "Tasks");

                return;
            }

            try
            {
                UploadReport(form, arguements.IFormManagerConnectionString);
            }
            catch (Exception ex)
            {
                results.FatalErrorOccured = true;

                ReportError(ex, 60, "Tasks");

                return;
            }
        }

        private void UploadImages(FormInstance form, string connectionString)
        {
            IFileManager<string> fileManager = InformationProviders.GetFileManager(connectionString);

            foreach (Section section in form.Sections)
            {
                foreach (Check check in section.Checks)
                {
                    foreach (RelatedFile imageFile in check.UserImages)
                    {
                        string destinationFilename = string.Format("{0}.png", DateTime.Now.ToFileTime());

                        string uri = InformationProviders.GetConnectionStringValue(connectionString, "Uri");
                        string listName = InformationProviders.GetConnectionStringValue(connectionString, "ListName");

                        string destinationRelativeUrl = string.Format(@"/sites/SiteManagement/{0}/{1}", listName, destinationFilename);

                        string destinationUrl = string.Format(@"{0}{1}/{2}", uri, listName, destinationFilename);

                        fileManager.UploadFile(imageFile.LocalFilename, destinationRelativeUrl);

                        imageFile.StorageFilename = destinationUrl;
                    }
                }
            }
        }

        private void CreateTasks(FormInstance form, string ITaskManagerConnectionString, string employeeNo)
        {
            ITaskManager<string> taskManager = InformationProviders.GetTaskManager(ITaskManagerConnectionString);

            foreach (Section section in form.Sections)
            {
                foreach (Check check in section.Checks)
                {
                    if (check.Result.CompareTo("False") == 0)
                    {
                        if (check.Target == "Engineering")
                        {
                            Task task = PopulateTaskFields(form, taskManager, check);

                            check.TaskId = taskManager.CreateTask(task, employeeNo);
                        }
                    }
                }
            }
        }

        private static Task PopulateTaskFields(FormInstance form, ITaskManager<string> taskManager, Check check)
        {
            Task task = new Task(Guid.NewGuid());

            task.EntityId = form.Metadata.EntityId;
            task.AssetNumber = check.AssetNumber;
            task.Description = taskManager.CreateTaskDescription(form, check);

            if (check.CWorksId != string.Empty)
            {
                task.CWorksLocationId = check.CWorksId;
            }
            else
            {
                task.CWorksLocationId = "0079";
            }

            task.Location = "MTWARD";
            task.Room = form.Metadata.Room;
            task.Bed = form.Metadata.Bed;

            return task;
        }

        private static void UploadReport(FormInstance form, string connectionString)
        {
            IFormManager formManager = new SharePointServices(connectionString);

            WordFormExporter exporter = new WordFormExporter();

            string localFilename = Path.GetTempFileName();

            exporter.Export(localFilename, form);

            formManager.StoreFormInstance(form.Metadata, localFilename);
        }


        protected override void OnRunWorkerCompleted(RunWorkerCompletedEventArgs e)
        {
            ProcessingResults results = (ProcessingResults) e.Result;

            formBeingProcessed.OutboxStatus = OutboxStatus.Completed;

            if ((e.Cancelled) | (results.FatalErrorOccured))
            {
                formBeingProcessed.Status = OutboxStatus.Error;
            }

            FormManager.Instance.Outbox.RefreshOutboxLabel();
        }
    }
}

using System;
using System.Collections.Generic;
using Cloudmaster.WCS.Classes;
using System.IO;
using Cloudmaster.WCS.IO;

namespace Cloudmaster.WCS.IO
{
    public class FileSystemServices : IFormManager, IFileManager<string>, IAdmisssionManager 
    {
        public string Uri { set; get; }

        public string ListName { set; get; }

        public FileSystemServices(string connectionString)
        {
            Dictionary <string, string> conenctionStringValues = ExternalServicesBase.ParseConnectionString(connectionString);

            ListName = conenctionStringValues["ListName"];

            Uri = conenctionStringValues["Uri"];
        }

        #region ITaskManager

        public string CreateTask(Task task)
        {
            List<Task> results = new List<Task>();

            string id = DateTime.Now.ToFileTime().ToString();

            string filename = string.Format("{0}/{1}.{2}", Uri, id, "tsx");

            while (File.Exists(filename))
            {
                filename = string.Format("{0}.{1}", DateTime.Now.ToFileTime());
            }

            XmlTypeSerializer<Task>.SerializeAndOverwriteFile(task, filename);

            return id;
        }

        public Task GetTasksById(string id)
        {
            Task task;

            string filename = string.Format("{0}/{1}.tsx", Uri, id);

            XmlTypeSerializer<Task>.TryParseFile(id, out task);

            return task;
        }

        public IList<Task> GetOpenTasks()
        {
            IList<Task> tasks = new List<Task>();

            string[] files = Directory.GetFiles(Uri);

            foreach (string file in files)
            {
                Task task;

                XmlTypeSerializer<Task>.TryParseFile(file, out task);

                if (task != null)
                {
                    tasks.Add(task);
                }
            }

            return tasks;
        }

        public string CreateTaskDescription(FormInstance form, Check failedCheck)
        {
            return string.Empty;
        }

        #endregion

        #region IFileManager

        public void UploadFile(string source, string destination)
        {
            File.Copy(source, destination);
        }

        #endregion

        #region IFormManager

        public IList<FormDefinition> GetFormDefinitions()
        {
            return FormRepository.GetFormDefinitions();
        }

        public IList<FormMetadata> GetFormInstancesMetadata()
        {
            string metadataPath = Path.Combine(Uri, ListName);

            IList<FormMetadata> formMetadatas = new List <FormMetadata>();

            string[] files = Directory.GetFiles(Uri);

            foreach (string file in files)
            {
                FormMetadata formMetaData;

                XmlTypeSerializer <FormMetadata>.TryParseFile(file, out formMetaData);
                
                if (formMetaData != null)
                {
                    formMetadatas.Add(formMetaData);
                }
            }

            return formMetadatas;
        }

        

        public FormMetadata StoreFormInstance(FormMetadata formMetadata, string filename)
        {
            string metadataPath = Path.Combine (Uri, ListName);

            if (!Directory.Exists(metadataPath))
            {
                Directory.CreateDirectory(metadataPath);

                File.SetAttributes(metadataPath, FileAttributes.Hidden);
            }

            string destinationFilename = string.Format (@"{0}\{1} ({2}).{3}", Uri, formMetadata.Name, DateTime.Now.ToFileTime(), formMetadata.OutputType);
            
            string metadataDestinationFilename = string.Format (@"{0}\{1} ({2}).{3}", metadataPath, formMetadata.Name, DateTime.Now.ToFileTime(), "fmx");

            File.Copy(filename, destinationFilename);

            XmlTypeSerializer<FormMetadata>.SerializeAndOverwriteFile(formMetadata, metadataDestinationFilename);

            return formMetadata;
        }

        #endregion

        #region IAdmisssionManager

        public IList<Order> GetOrdersForToday()
        {
            List<Order> result = new List<Order>();

            string[] files = Directory.GetFiles(Uri);

            foreach (string file in files)
            {
                Order order;

                XmlTypeSerializer<Order>.TryParseFile(file, out order);

                if (order != null)
                {
                    result.Add(order);
                }
            }

            return result;
        }

        public IList<Order> GetOrdersForWardToday(string wardIdentifer)
        {
            List<Order> result = new List<Order>();

            string[] files = Directory.GetFiles(Uri);

            foreach (string file in files)
            {
                Order order;

                XmlTypeSerializer<Order>.TryParseFile(file, out order);

                if (order != null)
                {
                    result.Add(order);
                }
            }

            return result;
        }

        public IList<OrderMetadata> GetOrdersMetadataForWardToday(Floor floor)
        {
            List<OrderMetadata> result = new List<OrderMetadata>();

            string metadataUri = string.Format (@"{0}\Metadata\", Uri);

            string[] files = Directory.GetFiles(metadataUri);

            foreach (string file in files)
            {
                OrderMetadata orderMetadata;

                XmlTypeSerializer<OrderMetadata>.TryParseFile(file, out orderMetadata);

                if (orderMetadata != null)
                {
                    result.Add(orderMetadata);
                }
            }

            return result;
        }


        public void SendOrderAcknowledgements(string orderNumber, bool isFastingAcknowledged, bool isPrepWorkAcknowledged, bool isExamAcknowledged, bool isInjectionAcknowledged)
        {
            string metadataFilename = string.Format(@"{0}\Metadata\{1}.xml", Uri, orderNumber);

            OrderMetadata orderMetadata;

            XmlTypeSerializer<OrderMetadata>.TryParseFile(metadataFilename, out orderMetadata);

            if (orderMetadata != null)
            {
                orderMetadata.IsExamAcknowledged = isExamAcknowledged;
                orderMetadata.IsPrepWorkAcknowledged = isPrepWorkAcknowledged;
                orderMetadata.IsExamAcknowledged = isExamAcknowledged;
                orderMetadata.IsInjectionAcknowledged = isInjectionAcknowledged;
            }

            XmlTypeSerializer<OrderMetadata>.SerializeAndOverwriteFile(orderMetadata, metadataFilename);
        }

        public void UpdateRequestedDateTimeOverride(string orderNumber, DateTime? startTime, string notes)
        {
            string metadataFilename = string.Format(@"{0}\Metadata\{1}.xml", Uri, orderNumber);

            OrderMetadata orderMetadata;

            XmlTypeSerializer<OrderMetadata>.TryParseFile(metadataFilename, out orderMetadata);

            if (orderMetadata != null)
            {
                orderMetadata.RequestedDateTimeOverride = startTime;
                orderMetadata.Notes = notes;
                orderMetadata.LastRequestedDateTimeOverrideModified = DateTime.Now;
            }

            XmlTypeSerializer<OrderMetadata>.SerializeAndOverwriteFile(orderMetadata, metadataFilename);
        }

        #endregion
    }
}

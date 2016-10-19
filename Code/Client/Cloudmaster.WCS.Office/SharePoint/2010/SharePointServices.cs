using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Cloudmaster.WCS.IO;
using Microsoft.SharePoint;
using Cloudmaster.WCS.Classes;
using Microsoft.SharePoint.Client;
using System.Net;
using System.Text.RegularExpressions;

namespace Cloudmaster.WCS.Office.SharePoint._2010
{
    public class SharePointServices : ITaskManager<string>, IFileManager <string>, IFormManager
    {
        ClientContext clientContext;

        public string SiteUri { set; get; }

        public string ListName { set; get; }

        public SharePointServices() { }

        public SharePointServices (string connectionString)
        {
            Dictionary <string, string> conenctionStringValues = ExternalServicesBase.ParseConnectionString(connectionString);

            ListName = conenctionStringValues["ListName"];

            SiteUri = conenctionStringValues["Uri"];

            string integratedSecurity = conenctionStringValues["Integrated Security"];

            clientContext = new ClientContext(SiteUri);

            if (integratedSecurity.CompareTo("False") == 0)
            {
                string domain = conenctionStringValues["Domain"];
                string username = conenctionStringValues["Username"];
                string password = conenctionStringValues["Password"];

                clientContext.Credentials = new NetworkCredential(username, password, domain);
            }
        }

        #region IFileManager

        public void UploadFile(string sourceFilename, string destinationUrl)
        {
            FileStream filestream = new FileStream(sourceFilename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            Microsoft.SharePoint.Client.File.SaveBinaryDirect(clientContext, destinationUrl, filestream, true);
        }

        #endregion

        #region ITaskManager

        public IList<Task> GetOpenTasks()
        {
            List<Task> results = new List<Task>();

            List list = clientContext.Web.Lists.GetByTitle("Tasks");

            CamlQuery camlQuery = new CamlQuery();
            camlQuery.ViewXml = "<View/>";

            ListItemCollection listItems = list.GetItems(camlQuery);

            clientContext.Load(list);
            clientContext.Load(listItems);
            clientContext.ExecuteQuery();

            foreach (ListItem listItem in listItems)
            {
                Task task = new Task(Guid.NewGuid());

                task.Name = TryGetStringValue(listItem, "Title");
                
                task.EntityId = TryGetGuidValue(listItem, "EntityId");
                task.Priority = TryGetStringValue(listItem, "Priority");
                task.Status = TryGetStringValue(listItem, "Status");
                task.Room = TryGetStringValue(listItem, "Room");

                string body = TryGetStringValue(listItem, "Body");

                Regex regex = new Regex(@"^<div><b>Description</b></div><div>(?<description>[^<]+)");

                Match match = regex.Match(body);

                if (match.Success)
                {
                    task.Description = match.Groups["description"].Value.Trim();
                }

                string id = TryGetStringValue(listItem, "ID");

                if (id != string.Empty)
                {
                    string url = string.Format(@"{0}{1}/AllItems.aspx?FilterField1=ID&FilterValue1={2}", SiteUri, ListName, id);

                    task.Url = url;
                }

                results.Add(task);
            }

            return results;
        }

        public string CreateTask(Task task, string employeeNo)
        {
            List<Task> results = new List<Task>();

            List list = clientContext.Web.Lists.GetByTitle("Tasks");
            clientContext.ExecuteQuery();

            ListItemCreationInformation itemCreateInfo = new ListItemCreationInformation();
            
            ListItem listItem = list.AddItem(itemCreateInfo);

            listItem["Body"] = task.Description;
            listItem["EntityId"] = task.EntityId;
            listItem["Room"] = task.Room;
            //listItem["Department"] = task.Location;

            listItem.Update();

            clientContext.ExecuteQuery();

            string taskNumber = string.Format("WR1{0:D5}", listItem.Id);

            listItem["Title"] = taskNumber;

            listItem.Update();

            clientContext.ExecuteQuery();

            return taskNumber;

        }

        public string CreateTaskDescription(FormInstance form, Check check)
        {
            string result;

            string imagesInformation = "<div><b>Images</b></div>";

            foreach (RelatedFile image in check.UserImages)
            {
                string imageFilename = string.Format("{0}.png", DateTime.Now.ToFileTime());

                string imageInfomration = string.Format(@"<div><a href=""{0}"">{1}</a></div>", image.StorageFilename, imageFilename);

                imagesInformation += imageInfomration;
            }

            result = string.Format("<div><b>Description</b></div><div>{0}</div><div>&nbsp;</div>{1}</p>", check.Comments, imagesInformation);

            return result;
        }

        public Task GetTasksById(string id)
        {
            return new Task(Guid.NewGuid());
        }

        #endregion

        #region IFormManager

        public IList<FormDefinition> GetFormDefinitions()
        {
            return FormRepository.GetFormDefinitions();
        }

        public IList<FormMetadata> GetFormInstancesMetadata()
        {
            List<FormMetadata> results = new List<FormMetadata>();

            List list = clientContext.Web.Lists.GetByTitle("Completed Forms");

            CamlQuery camlQuery = new CamlQuery();
            camlQuery.ViewXml = "<View/>";

            ListItemCollection listItems = list.GetItems(camlQuery);

            clientContext.Load(listItems);
            clientContext.ExecuteQuery();

            foreach (ListItem listItem in listItems)
            {
                FormMetadata metadata = new FormMetadata();
                
                metadata.EntityId = TryGetGuidValue (listItem, "EntityId");
                metadata.FormDefinitionId = TryGetGuidValue(listItem, "FormDefinitionId");
                metadata.DateCompleted = DateTime.Parse(listItem["Created"].ToString ());

                results.Add(metadata);
            }
            
            return results;
        }

        public FormMetadata StoreFormInstance(FormMetadata metadata, string filename)
        {
            byte[] bytes = System.IO.File.ReadAllBytes(filename);

            List documentsList = clientContext.Web.Lists.GetByTitle("Completed Forms");

            var fileCreationInformation = new FileCreationInformation();

            fileCreationInformation.Content = bytes;
            fileCreationInformation.Overwrite = true;

            fileCreationInformation.Url = string.Format (@"{0}{1}\{2} ({3}).{4}", SiteUri, ListName, metadata.Name, DateTime.Now.ToFileTime(), metadata.OutputType);
               
            Microsoft.SharePoint.Client.File uploadFile = documentsList.RootFolder.Files.Add(fileCreationInformation);

            uploadFile.ListItemAllFields["EntityId"] = metadata.EntityId;
            uploadFile.ListItemAllFields["FormDefinitionId"] = metadata.FormDefinitionId;
            uploadFile.ListItemAllFields["Room"] = metadata.Room;
            //uploadFile.ListItemAllFields["Department"] = metadata.CWorksLocationId;

            uploadFile.ListItemAllFields.Update();

            clientContext.ExecuteQuery();

            return metadata;
        }

        #endregion

         
        private string TryGetStringValue(ListItem listItem, string fieldName)
        {
            string result = string.Empty;

            object field = listItem[fieldName];

            if (field != null)
            {
                result = field.ToString();
            }

            return result;
        }

        private Guid TryGetGuidValue(ListItem listItem, string fieldName)
        {
            Guid result = new Guid();

            object field = listItem[fieldName];

            if (field != null)
            {
                Guid.TryParse(field.ToString(), out result);
            }

            return result;
        }
    }
}

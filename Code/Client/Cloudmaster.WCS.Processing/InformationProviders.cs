using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cloudmaster.WCS;
using Cloudmaster.WCS.IO;
using Cloudmaster.WCS.IO;
using Cloudmaster.WCS.Office.SharePoint._2010; 


namespace Cloudmaster.WCS.Processing
{
    public class InformationProviders
    {
        public static IAdmisssionManager GetAdmissionsManager(string connectionString)
        {
            IAdmisssionManager result;

            Dictionary<string, string> connectionStringValues = ExternalServicesBase.ParseConnectionString(connectionString);

            string provider = connectionStringValues["Provider"];

            switch (provider)
            {
                case "Iguana":

                    result = new IguanaServices(connectionString);

                    break;

                case "FileSystem":

                    result = new FileSystemServices(connectionString);

                    break;

                default:

                    throw new Exception(string.Format("Data provider {0} not supported", provider));
            }

            return result;
        }

        public static string GetConnectionStringValue(string connectionString, string name)
        {
            Dictionary<string, string> connectionStringValues = ExternalServicesBase.ParseConnectionString(connectionString);

            string result = connectionStringValues[name];

            return result;
        }

        public static IFileManager<string> GetFileManager(string connectionString)
        {
            IFileManager<string> result;

            Dictionary<string, string> connectionStringValues = ExternalServicesBase.ParseConnectionString(connectionString);

            string provider = connectionStringValues["Provider"];

            switch (provider)
            {
                case "FileSystem":

                    result = new FileSystemServices(connectionString);

                    break;

                case "SharePoint":

                    result = new SharePointServices(connectionString);

                    break;

                default:

                    throw new Exception(string.Format("Data provider {0} not supported", provider));
            }

            return result;
        }

        public static ITaskManager<string> GetTaskManager(string connectionString)
        {
            ITaskManager<string> result;

            Dictionary<string, string> connectionStringValues = ExternalServicesBase.ParseConnectionString(connectionString);

            string provider = connectionStringValues["Provider"];

            switch (provider)
            {
                case "SharePoint":

                    result = new SharePointServices(connectionString);

                    break;

                case "CWorks":

                    result = new CWorksServices(connectionString);

                    break;

                default:

                    throw new Exception(string.Format("Data provider {0} not supported", provider));
            }

            return result;
        }
    }
}

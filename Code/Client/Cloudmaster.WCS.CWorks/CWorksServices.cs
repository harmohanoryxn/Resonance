using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cloudmaster.WCS.CWorks.Client;
using Cloudmaster.WCS;
using Cloudmaster.WCS.Classes;
using System.Text.RegularExpressions;
using System.Data.Services.Client;
using Cloudmaster.WCS.IO;

namespace Cloudmaster.WCS.CWorks
{
	/// <summary>
	/// Connect to the CWorks WCF Service and provides ITaskManager ability to the data that comes from the service
	/// </summary>
    public class CWorksServices : ExternalServicesBase, ITaskManager<string>
    {
        private CWorksConnectionSettings services;

        public CWorksServices(string connectionString)
        {
            Dictionary<string, string> connectionStringValues = ExternalServicesBase.ParseConnectionString(connectionString);

            services = new CWorksConnectionSettings(new Uri(connectionStringValues["Uri"]));
        }

        public string CreateTaskDescription(FormInstance form, Check check)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("Check: {0}", check.Description);
            sb.AppendLine();
            sb.AppendFormat("Comments: {0}", check.Comments);
            sb.AppendLine();

            foreach (RelatedFile image in check.UserImages)
            {
                sb.AppendLine();
                sb.AppendFormat("Image: {0}", image.StorageFilename);
            }

            return sb.ToString();
        }

        public string CreateTask(Task task, string employeeNo)
        {
            /*
            DataServiceQuery<string> result = services.CreateQuery<string>("CreateWorkRequest").AddQueryOption("description", task.Description)
                .AddQueryOption ("requestsPin", "1046")
                .AddQueryOption("locationNo", "0079")
                .AddQueryOption("deptNo", "MTWARD");

            return result.First();
        }
            */


            int workRequestCount = services.worequests.Count();

            worequest workRequest = new worequest();

            string workRequestNumber = string.Format("CENTRAL07{0:D6}", workRequestCount);

            workRequest.ProblemDesc = task.Description;

            workRequest.CreatedBy = employeeNo;
            workRequest.EmployeeNo = employeeNo;
            workRequest.RequestStatus = 1;
            workRequest.ReceivedDate = DateTime.Now;
            workRequest.WorkType = 3;
            workRequest.WorkPriority = 1;
            workRequest.AssetNo = string.Empty;
            workRequest.SiteCode = "CENTRAL";
            workRequest.DepartmentNo = "MTWARD";

            workRequest.RequestNo = workRequestNumber;

            services.AddObject("worequests", workRequest);

            services.SaveChanges(System.Data.Services.Client.SaveChangesOptions.Batch);

            return workRequestNumber;
        }


        public Task GetTasksById(string workRequestNumber)
        {
            CWorksTask task = null;

            worequest workRequest = services.worequests.Where(w => w.RequestNo == workRequestNumber).First();

            if (workRequest != null)
            {
                task = new CWorksTask(Guid.NewGuid());

                task.Description = workRequest.ProblemDesc;
                task.Status = workRequest.RequestStatus.ToString();
                task.AssetNumber = workRequest.AssetNo;
            }

            return task;
        }

        public IList<Task> GetOpenTasks()
        {
            List<Task> tasks = new List<Task>();

            var results = services.worequests.Where(w => w.RequestStatus == 1 | w.RequestStatus == 2);

            foreach(var workRequest in results)
            {
                Task task = new Task (Guid.NewGuid());

                task.Name = workRequest.RequestNo;
                task.Description = workRequest.ProblemDesc;

                if (workRequest.RequestStatus == 1)
                {
                    task.Status = "Open";
                }
                else if (workRequest.RequestStatus == 2)
                {
                    task.Status = "WO raised";
                }

                task.AssetNumber = workRequest.AssetNo;
                task.Location = workRequest.LocationNo;

                Regex regex = new Regex("^Room: (?<roomName>.+)");

                Match match = regex.Match(task.Description);

                if (match.Success)
                {
                    task.Room = match.Groups["roomName"].Value.Trim();
                }

                tasks.Add(task);
            }

            return tasks;
        }
    }
}

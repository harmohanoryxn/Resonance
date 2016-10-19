using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WCS.Services.DataServices.Data;
using WCS.Services.DataServices;

namespace WCS.Services.Test
{
    [TestClass]
    public class DemoData
    {
        ServerFacade server = new ServerFacade();

        LocationCodes ctLocationCode = new LocationCodes() { "CT" };
        LocationCodes usLocationCode = new LocationCodes() { "US" };

        private const string CLIENT_NAME = "WCS-DEMO01";

        [TestMethod]
        public void UpdateORD002ProcedureTime()
        {
            var ctOrders = server.GetOrders(DateTime.Today, DateTime.Today.AddDays(1.0), ctLocationCode,
                                            DateTime.Today.AddHours(12));

            var ordn002 = ctOrders.First(o => o.OrderNumber == "ORDN002");

            Assert.IsFalse(ordn002.IsHidden);

            ordn002 = server.UpdateProcedureTime(ordn002.OrderId, DateTime.Now.AddHours(1.5), DateTime.Now, CLIENT_NAME);
        }

        [TestMethod]
        public void UpdateORD020ProcedureTime()
        {
            var ctOrders = server.GetOrders(DateTime.Today, DateTime.Today.AddDays(1.0), usLocationCode,
                                            DateTime.Today.AddHours(12));

            var ordn020 = ctOrders.First(o => o.OrderNumber == "ORDN020");
            
            Assert.IsFalse(ordn020.IsHidden);

            ordn020 = server.UpdateProcedureTime(ordn020.OrderId, DateTime.Now.AddHours(3), DateTime.Now, CLIENT_NAME);
        }

        [TestMethod]
        public void MarkBedsAsCleanDemoData()
        {
            var ward1Beds = server.GetCleaningBedData(new LocationCodes() { "WARD1" }, DateTime.Today.AddHours(8));

            // Mark beds 1 and 3 in each room as clean, except for those one bed rooms and room 107 which is used in other tests
            var bedsToMarkClean = from b in ward1Beds where
                                      (new string[] {"1", "3"}).Contains(b.Name)
                                      && !(new string[] { "102", "104", "106", "107", "108" }).Contains(b.Location.Room) 
                                  select b;

            foreach (var b in bedsToMarkClean)
            {

                server.MarkBedAsClean(b.BedId, DateTime.Now, CLIENT_NAME);
            }
        }

        
        [TestMethod]
        public void InsertRfidDetectionsThatShouldHaveHappenedByNow()
        {
            // Get all orders
            var allOrders = server.GetOrders(DateTime.Today, DateTime.Today.AddDays(1.0), new LocationCodes(), DateTime.Today.AddHours(12));

            // Remove orders we don't want detections for here
            var ordersToInsertDetectionsFor = from o in allOrders where !(new string[] { "ORDN001", "ORDN002", "ORDN043" }).Contains(o.OrderNumber) select o;

            foreach (var o in ordersToInsertDetectionsFor)
            {
                string patientCode = o.Admission.Patient.IPeopleNumber;

                string wardRfidCode = o.Admission.Location.Name + "_RFID1";
                string deptRfidCode = o.DepartmentCode + "_RFID1";

                // Insert a trail of detections leaving 15 minutes before appointment, arriving 10 before, leaving 10 min after appointment, arriving 15 min after
                if (o.ProcedureTime.HasValue && o.ProcedureTime != DateTime.Today)
                {
                    DateTime pt = o.ProcedureTime.Value;
                    DateTime leaveWard = pt.AddMinutes(-15);
                    DateTime arriveDept = pt.AddMinutes(-10);
                    DateTime leaveDept = pt.AddMinutes(o.Duration.Minutes + 10);
                    DateTime arriveWard = pt.AddMinutes(o.Duration.Minutes + 15);

                    if (leaveWard <= DateTime.Now) server.InsertDetection("HIS", patientCode, "RFID", wardRfidCode, DetectionDirection.Out, leaveWard);
                    if (arriveDept <= DateTime.Now) server.InsertDetection("HIS", patientCode, "RFID", deptRfidCode, DetectionDirection.In, arriveDept);
                    if (leaveDept <= DateTime.Now) server.InsertDetection("HIS", patientCode, "RFID", deptRfidCode, DetectionDirection.Out, leaveDept);
                    if (arriveWard <= DateTime.Now) server.InsertDetection("HIS", patientCode, "RFID", wardRfidCode, DetectionDirection.In, arriveWard);
                }
            }
        }

        /*
        [TestMethod]
        public void SetORDN008ToHappenSoonForDemo()
        {
            string patientCode = "PAT008";

            // Get all orders
            var allOrders = server.GetOrders(DateTime.Today, DateTime.Today.AddDays(1.0), new LocationCodes() { "CT" }, DateTime.Today.AddHours(12));

            var ordn008 = (from o in allOrders where (new string[] { "ORDN008" }).Contains(o.OrderNumber) select o).First();

            ordn008 = server.UpdateProcedureTime(ordn008.OrderId, DateTime.Now.AddMinutes(5), DateTime.Now, CLIENT_NAME);

            server.AcknowledgeNotification(ordn008.Notifications.First().NotificationId, DateTime.Now, CLIENT_NAME);

            server.InsertDetection("HIS", patientCode, "RFID", ordn008.Admission.Location.Name + "_RFID1", DetectionDirection.Out, DateTime.Now.AddMinutes(-10));
            server.InsertDetection("HIS", patientCode, "RFID", ordn008.DepartmentCode + "_RFID1", DetectionDirection.In, DateTime.Now.AddMinutes(-5));
        }
         * */
    }
}

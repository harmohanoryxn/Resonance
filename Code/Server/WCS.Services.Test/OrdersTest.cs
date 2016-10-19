using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WCS.Services.DataServices.Data;
using WCS.Services.DataServices;
using EF = WCS.Data.EF;

namespace WCS.Services.Test
{
    [TestClass]
    public class OrdersTest
    {
        ServerFacade server = new ServerFacade();
        private const string CLIENT_NAME = "WCS-DEMO01";
        LocationCodes allLocationCodes = new LocationCodes() { };
        LocationCodes ctLocationCode = new LocationCodes() { "CT" };
        LocationCodes mriLocationCode = new LocationCodes() { "MRI" };

        [TestMethod]
        public void GetOrdersForAllLocationsTest()
        {
            var orders = server.GetOrders(DateTime.Today, DateTime.Today.AddDays(1.0), allLocationCodes, DateTime.Today.AddHours(12));
            Assert.AreEqual(orders.Count, 45);
        }

        [TestMethod]
        public void GetOrdersForOneDepartmentTest()
        {
            var oneDepartment = new LocationCodes() { "VASC" };

            var orders = server.GetOrders(DateTime.Today, DateTime.Today.AddDays(1.0), oneDepartment, DateTime.Today.AddHours(12));
            Assert.AreEqual(1, orders.Where(o => o.DepartmentCode == "VASC").Count());
        }

        [TestMethod]
        public void GetOrdersForOneWardTest()
        {
            var oneWard = new LocationCodes() { "WARD1" };

            var orders = server.GetOrders(DateTime.Today, DateTime.Today.AddDays(1.0), oneWard, DateTime.Today.AddHours(12));

            Assert.AreEqual(orders.Count, 15);
        }

        [TestMethod]
        public void LookAtOrderInDetailTest()
        {
            // We will test that all properties of ORD001 that are in the StagingData_Test.xls spreadsheet are populated correctly
            var ctOrders = server.GetOrders(DateTime.Today, DateTime.Today.AddDays(1.0), mriLocationCode, DateTime.Today.AddHours(12));
            var ord001 = ctOrders.First(o => o.OrderNumber == "ORDN001");

            Assert.AreEqual(ord001.OrderNumber, "ORDN001");
            Assert.AreEqual(ord001.DepartmentCode, "MRI");
            Assert.AreEqual(ord001.ProcedureCode, "ABD");
            Assert.AreEqual(ord001.ProcedureDescription, "Abdomen");
            Assert.AreEqual(ord001.ProcedureTime, DateTime.Today.AddHours(14));
            Assert.AreEqual(ord001.Status, OrderStatus.InProgress);
            Assert.AreEqual(ord001.ClinicalIndicators, "Stomach pains");
            Assert.IsNull(ord001.CompletedTime);
            Assert.AreEqual(ord001.EstimatedProcedureDuration, 60);
            Assert.AreEqual(ord001.IsHidden, false);

            // Admission
            Assert.AreEqual(ord001.Admission.Type, AdmissionType.In);
            Assert.AreEqual(ord001.Admission.Status, AdmissionStatus.Admitted);
            Assert.AreEqual(ord001.Admission.AdmitDateTime, DateTime.Today.AddHours(11).AddMinutes(13));
            Assert.AreEqual(ord001.Admission.DischargeDateTime, DateTime.Today.AddHours(17).AddMinutes(30));
            Assert.AreEqual(ord001.Admission.AdmittingDoctor, "MRS. JULIE LYNCH");
            Assert.AreEqual(ord001.Admission.AttendingDoctor, "Mr. David Burnett");
            Assert.AreEqual(ord001.Admission.PrimaryDoctor, "mr. joe mcinty");

            // Admission.CriticalCareIndicators
            Assert.AreEqual(ord001.Admission.CriticalCareIndicators.IsMrsaRisk, true);
            Assert.AreEqual(ord001.Admission.CriticalCareIndicators.IsFallRisk, true);
            Assert.AreEqual(ord001.Admission.CriticalCareIndicators.HasLatexAllergy, true);
            Assert.AreEqual(ord001.Admission.CriticalCareIndicators.IsRadiationRisk, false);

            // Admission.Location
            Assert.AreEqual(ord001.Admission.Location.Name, "WARD1");
            Assert.AreEqual(ord001.Admission.Location.Room, "101");
            Assert.AreEqual(ord001.Admission.Location.Bed, "1");

            // Admission.Patient
            Assert.AreEqual(ord001.Admission.Patient.GivenName, "Christine");
            Assert.AreEqual(ord001.Admission.Patient.FamilyName, "Davis");
            Assert.AreEqual(ord001.Admission.Patient.IPeopleNumber, "PAT001");
            Assert.AreEqual(ord001.Admission.Patient.DateOfBirth, new DateTime(1961, 4, 20));
            Assert.AreEqual(ord001.Admission.Patient.IsAssistanceRequired, false);
            Assert.AreEqual(ord001.Admission.Patient.AssistanceDescription, String.Empty);
        }

        [TestMethod]
        public void CheckEmergencyRoomOrderTest()
        {
            var ctOrders = server.GetOrders(DateTime.Today, DateTime.Today.AddDays(1.0), ctLocationCode, DateTime.Today.AddHours(12));

            var emergencyOrder = ctOrders.First(o => o.OrderNumber == "ORDN023");

            Assert.IsTrue(emergencyOrder.Admission.Location.IsEmergency);
        }

        [TestMethod]
        public void HideUnhideOrderTest()
        {
            var ctOrders = server.GetOrders(DateTime.Today, DateTime.Today.AddDays(1.0), ctLocationCode, DateTime.Today.AddHours(12));

            var ordn002 = ctOrders.First(o => o.OrderNumber == "ORDN002");

            Assert.IsFalse(ordn002.IsHidden);

            ordn002 = server.HideOrder(ordn002.OrderId, DateTime.Today.AddHours(8), CLIENT_NAME);

            Assert.IsTrue(ordn002.IsHidden);
            ordn002 = server.UnhideOrder(ordn002.OrderId, DateTime.Today.AddHours(8).AddMinutes(5), CLIENT_NAME);
            Assert.IsFalse(ordn002.IsHidden);
        }

        [TestMethod]
        public void CheckOrdersForPatientWithTwoAdmissionsTest()
        {
            var orders = server.GetOrders(DateTime.Today, DateTime.Today.AddDays(1.0), new LocationCodes() { "CT", "TH1" }, DateTime.Today.AddHours(12));

            var ordn007 = orders.First(o => o.OrderNumber == "ORDN007");
            var ordn043 = orders.First(o => o.OrderNumber == "ORDN043");

            Assert.AreEqual(ordn007.Admission.Type, AdmissionType.In);
            Assert.AreEqual(ordn043.Admission.Type, AdmissionType.In);
        }

        [TestMethod]
        public void AcknowledgeNotificationTest()
        {
            var ctOrders = server.GetOrders(DateTime.Today, DateTime.Today.AddDays(1.0), ctLocationCode, DateTime.Today.AddHours(12));

            // Get an order needing acknowledgement
            var unAckOrder = ctOrders.First(not => not.Notifications.Any(n => !n.Acknowledged));
            string unAckOrderOrderNo = unAckOrder.OrderNumber;
            var notificationId = unAckOrder.Notifications.First().NotificationId;

            // Acknowledge it
            var ackOrder = server.AcknowledgeNotification(notificationId, DateTime.Now, CLIENT_NAME);

            var ackNotification = ackOrder.Notifications.First(n => n.NotificationId == notificationId);

            Assert.AreEqual(ackNotification.Acknowledged, true);

        }

        [TestMethod]
        public void AddDeleteNoteTest()
        {
            var ctOrders = server.GetOrders(DateTime.Today, DateTime.Today.AddDays(1.0), ctLocationCode, DateTime.Today.AddHours(12));
            var ord002 = ctOrders.First(o => o.OrderNumber == "ORDN002");

            int noteCountBefore = ord002.Notes.Count();
            
            // Add a note
            ord002 = server.AddOrderNote(ord002.OrderId, "test add", DateTime.Today.AddHours(12), CLIENT_NAME);

            Assert.AreEqual(ord002.Notes.Count(), noteCountBefore + 1);

            // Delete the note
            noteCountBefore = ord002.Notes.Count();
            ord002 = server.DeleteOrderNote(ord002.Notes.Last().NoteId, DateTime.Today.AddHours(14), CLIENT_NAME);

            Assert.AreEqual(ord002.Notes.Count(), noteCountBefore - 1);
        }

        [TestMethod]
        [ExpectedException(typeof(AccessViolationException))]
        public void DeleteNoteFailTest()
        {
            var ctOrders = server.GetOrders(DateTime.Today, DateTime.Today.AddDays(1.0), ctLocationCode, DateTime.Now);
            var ord002 = ctOrders.First(o => o.OrderNumber == "ORDN002");

            ord002 = server.AddOrderNote(ord002.OrderId, "Pateint Feeling Better Today", DateTime.Now.AddHours(-1), CLIENT_NAME);

            server.DeleteOrderNote(ord002.Notes.Last().NoteId, DateTime.Today.AddHours(-1), "NOT_" + CLIENT_NAME);
        }

        [TestMethod]
        public void UpdateProcedureTimeTest()
        {
            var ctOrders = server.GetOrders(DateTime.Today, DateTime.Today.AddDays(1.0), ctLocationCode, DateTime.Today.AddHours(12));
            var ord004 = ctOrders.First(o => o.OrderNumber == "ORDN004");

            DateTime newProcedureTime = DateTime.Today.AddHours(12);
            DateTime timestamp = DateTime.Today.AddHours(8);

            ord004 = server.UpdateProcedureTime(ord004.OrderId, newProcedureTime, timestamp, CLIENT_NAME);

            Assert.AreEqual(newProcedureTime, ord004.ProcedureTime);

            Assert.AreEqual(EF.WCSUpdateTypes.ProcedureTimeUpdated, ord004.Updates.First(u => u.Created == timestamp).Type);
        }

        [TestMethod]
        public void RelatedOrdersTest()
        {
            // Even though we query for CT orders we should also get back PAT002s ORDN043 order for TH1 as it is related
            var ctOrders = server.GetOrders(DateTime.Today, DateTime.Today.AddDays(1.0), ctLocationCode, DateTime.Today.AddHours(12));

            var ordn002 = ctOrders.FirstOrDefault(o => o.OrderNumber == "ORDN002");
            Assert.IsNotNull(ordn002);
            
            var ordn043 = ctOrders.FirstOrDefault(o => o.OrderNumber == "ORDN043");
            Assert.IsNotNull(ordn043);
        }

        [TestMethod]
        public void CheckPatientRadiationRiskTest()
        {
            // We will consider ORDN011/ADM011
            DateTime timestamp = DateTime.Today.AddHours(8);
            var mriOrders = server.GetOrders(DateTime.Today, DateTime.Today.AddDays(1.0), mriLocationCode, timestamp);
            var ordn011 = mriOrders.FirstOrDefault(o => o.OrderNumber == "ORDN011");

            // At 8am the patient is not a radiation risk
            Assert.IsFalse(ordn011.Admission.CriticalCareIndicators.IsRadiationRisk);

            // The procedure time is 2pm and the prep with radiation risk would be performed at 1am
            timestamp = DateTime.Today.AddHours(13);
            mriOrders = server.GetOrders(DateTime.Today, DateTime.Today.AddDays(1.0), mriLocationCode, timestamp);
            ordn011 = mriOrders.FirstOrDefault(o => o.OrderNumber == "ORDN011");
            Assert.IsTrue(ordn011.Admission.CriticalCareIndicators.IsRadiationRisk);

            // Under normal cases the patient would no longer be a radiation risk at 4pm, first check they are still a risk at 3.59pm
            timestamp = DateTime.Today.AddHours(15).AddMinutes(59);
            mriOrders = server.GetOrders(DateTime.Today, DateTime.Today.AddDays(1.0), mriLocationCode, timestamp);
            ordn011 = mriOrders.FirstOrDefault(o => o.OrderNumber == "ORDN011");
            Assert.IsTrue(ordn011.Admission.CriticalCareIndicators.IsRadiationRisk);

            // Now check at 4pm
            timestamp = DateTime.Today.AddHours(16);
            mriOrders = server.GetOrders(DateTime.Today, DateTime.Today.AddDays(1.0), mriLocationCode, timestamp);
            ordn011 = mriOrders.FirstOrDefault(o => o.OrderNumber == "ORDN011");
            Assert.IsFalse(ordn011.Admission.CriticalCareIndicators.IsRadiationRisk);

            // Now acknowledge the notification 15 minutes late at 1.15pm
            var contrastNotification = ordn011.Notifications.First();
            timestamp = DateTime.Today.AddHours(13).AddMinutes(15);
            ordn011 = server.AcknowledgeNotification(contrastNotification.NotificationId, timestamp, CLIENT_NAME);

            // Because the notification was late this means the radiation risk extends 
            timestamp = DateTime.Today.AddHours(16);
            mriOrders = server.GetOrders(DateTime.Today, DateTime.Today.AddDays(1.0), mriLocationCode, timestamp);
            ordn011 = mriOrders.FirstOrDefault(o => o.OrderNumber == "ORDN011");
            Assert.IsTrue(ordn011.Admission.CriticalCareIndicators.IsRadiationRisk);

            // But at 4.15pm they are no longer a risk again
            timestamp = DateTime.Today.AddHours(16).AddMinutes(15);
            mriOrders = server.GetOrders(DateTime.Today, DateTime.Today.AddDays(1.0), mriLocationCode, timestamp);
            ordn011 = mriOrders.FirstOrDefault(o => o.OrderNumber == "ORDN011");
            Assert.IsFalse(ordn011.Admission.CriticalCareIndicators.IsRadiationRisk);
        }
    
    
    }
}

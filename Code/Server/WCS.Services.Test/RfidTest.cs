using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WCS.Services.DataServices;
using WCS.Services.DataServices.Data;

namespace WCS.Services.Test
{
    [TestClass]
    public class RfidTest
    {
        ServerFacade server = new ServerFacade();
        private const string CLIENT_NAME = "WCS-DEMO01";

        [TestMethod]
        public void PAT001RfidWaitingRoomTest()
        {
            DateTime startTest = DateTime.Now;

            // Find the patient through it's order ORDN001
            var ctOrders = server.GetOrders(DateTime.Today, DateTime.Today.AddDays(1.0), new LocationCodes() { "CT" }, DateTime.Today.AddHours(12));
            var ord002 = ctOrders.First(o => o.OrderNumber == "ORDN002");

            string patientCode = ord002.Admission.Patient.IPeopleNumber;

            Assert.AreEqual(patientCode, "PAT002");

            // Insert a trail of detections in waiting areas and locations
            var detection = server.InsertDetection("HIS", patientCode, "RFID", "WARD1_RFID1", DetectionDirection.Out, startTest.AddMilliseconds(10));
            detection = server.InsertDetection("HIS", patientCode, "RFID", "RADIOLOGY_RFID1", DetectionDirection.In, startTest.AddMilliseconds(20));

            // Insert a trail of detections in locations

            var pcs = new PatientCodes() { patientCode };
            var allDetections = server.GetEntireDetectionHistory(startTest, startTest.AddMilliseconds(30), pcs);
            
            Assert.AreEqual(2, allDetections.Count());

            var latestDetection = server.GetLatestDetection(DateTime.Today, DateTime.Today.AddDays(1), pcs).First(d => d.PatientId == ord002.Admission.Patient.PatientId);
            Assert.AreEqual("RADIOLOGY", latestDetection.DetectionLocation.LocationCode);
        }

        [TestMethod]
        public void PAT001RfidTest()
        {
            // Find the patient through it's order ORDN001
            var mriOrders = server.GetOrders(DateTime.Today, DateTime.Today.AddDays(1.0), new LocationCodes() { "MRI" }, DateTime.Today.AddHours(12));
            var ord001 = mriOrders.First(o => o.OrderNumber == "ORDN001");

            string patientCode = ord001.Admission.Patient.IPeopleNumber;

            ord001 = server.UpdateProcedureTime(ord001.OrderId, DateTime.Now.AddHours(-0.5), DateTime.Now, CLIENT_NAME);

            var procedureTime = ord001.ProcedureTime.Value;

            // Insert a trail of detections in waiting areas and locations
            var detection = server.InsertDetection("HIS", patientCode, "RFID", "WARD1_RFID1", DetectionDirection.In, DateTime.Today.AddHours(8));
            detection = server.InsertDetection("HIS", patientCode, "RFID", "WARD1_RFID1", DetectionDirection.Out, procedureTime.AddMinutes(-60));
            detection = server.InsertDetection("HIS", patientCode, "RFID", "RADIOLOGY_RFID1", DetectionDirection.In, procedureTime.AddMinutes(-59));
            detection = server.InsertDetection("HIS", patientCode, "RFID", "RADIOLOGY_RFID1", DetectionDirection.Out, procedureTime.AddMinutes(-30));
            detection = server.InsertDetection("HIS", patientCode, "RFID", "MRI_RFID1", DetectionDirection.In, procedureTime.AddMinutes(-29));
            detection = server.InsertDetection("HIS", patientCode, "RFID", "MRI_RFID1", DetectionDirection.Out, procedureTime.AddMinutes(15));
            detection = server.InsertDetection("HIS", patientCode, "RFID", "WARD1_RFID1", DetectionDirection.In, procedureTime.AddMinutes(20));

            // Insert a trail of detections in locations

            var pcs = new PatientCodes() { patientCode };
            var allDetections = server.GetEntireDetectionHistory(DateTime.Today, DateTime.Today.AddDays(1), pcs);
            //Assert.AreEqual(6, allDetections.Count());

            var latestDetection = server.GetLatestDetection(DateTime.Today, DateTime.Today.AddDays(1), pcs).First(d => d.PatientId == ord001.Admission.Patient.PatientId);
            Assert.AreEqual("RADIOLOGY", latestDetection.DetectionLocation.LocationCode);
        }
    }
}

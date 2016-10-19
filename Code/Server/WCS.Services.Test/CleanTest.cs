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
    public class CleanTest
    {
        ServerFacade server = new ServerFacade();
        private const string CLIENT_NAME = "WCS-DEMO01";
        LocationCodes ward1 = new LocationCodes() { "WARD1" };
        LocationCodes ward3 = new LocationCodes() { "WARD3" };

        [TestMethod]
        public void PerformanceTest()
        {
            DateTime startTime = DateTime.Now;

            var cleaningData = server.GetCleaningBedData(ward1, DateTime.Today.AddHours(8));

            DateTime end = DateTime.Now;

            TimeSpan duration = end.Subtract(startTime);

            bool isLongerThanFiveSeconds = (duration > TimeSpan.FromSeconds(5));

            Assert.IsFalse(isLongerThanFiveSeconds, string.Format("Get Cleaning Bed Data took {0} seconds to complete", duration));
        }

        [TestMethod]
        public void MarkBedAsCleanDirtyTest()
        {
            var ward1Beds = server.GetCleaningBedData(ward1, DateTime.Today.AddHours(8));
            var room107bed1 = (from b in ward1Beds where b.Room.Ward == "WARD1" && b.Room.Name == "107" && b.Name == "1" select b).First();

            room107bed1 = server.MarkBedAsClean(room107bed1.BedId, DateTime.Today.AddSeconds(10), CLIENT_NAME);

            Assert.AreEqual(room107bed1.BedId, room107bed1.BedId);
            Assert.AreEqual(room107bed1.CurrentStatus, BedStatus.Clean);

            room107bed1 = server.MarkBedAsDirty(room107bed1.BedId, DateTime.Today.AddSeconds(20), CLIENT_NAME);
            Assert.AreEqual(room107bed1.CurrentStatus, BedStatus.Dirty);
        }

        [TestMethod]
        public void CheckEstimatedDischargeDateTest()
        {
            var ward1BedCleaningData = server.GetCleaningBedData(ward1, DateTime.Today.AddSeconds(10));
            var room107bed1CleaningData = (from b in ward1BedCleaningData where b.Room.Ward == "WARD1" && b.Room.Name == "107" && b.Name == "1" select b).First();

            // Should be no estimated discharge date
            Assert.IsNull(room107bed1CleaningData.EstimatedDischargeDate);

            var ward1DischargesData = server.GetDischargesBedData(ward1, DateTime.Today.AddSeconds(10));
            var room107bed1DischargesData = (from b in ward1DischargesData where b.Room.Ward == "WARD1" && b.Room.Name == "107" && b.Name == "1" select b).First();

            // Update the estimated discharge date to 8pm
            room107bed1DischargesData = server.UpdateEstimatedDischargeDate(room107bed1DischargesData.CurrentAdmission.AdmissionId, DateTime.Today.AddSeconds(20), DateTime.Today.AddHours(9), CLIENT_NAME);

            // Retrieve cleaning data again and check estimated discharge date is updated
            ward1BedCleaningData = server.GetCleaningBedData(ward1, DateTime.Today.AddSeconds(10));
            room107bed1CleaningData = (from b in ward1BedCleaningData where b.Room.Ward == "WARD1" && b.Room.Name == "107" && b.Name == "1" select b).First();
            Assert.AreEqual(DateTime.Today.AddHours(20), room107bed1CleaningData.EstimatedDischargeDate);
        }

        [TestMethod]
        public void GetAllFreeBedTimesForDateTest()
        {
            // First assign a procedure time to ORD002 so it will appear in the available beds
            var ctOrders = server.GetOrders(DateTime.Today, DateTime.Today.AddDays(1.0), new LocationCodes() { "CT" }, DateTime.Today.AddHours(12));
            var ord002 = ctOrders.First(o => o.OrderNumber == "ORDN002");

            server.UpdateProcedureTime(ord002.OrderId, DateTime.Today.AddSeconds(15), DateTime.Today.AddHours(16), CLIENT_NAME);

            var ward1Beds = server.GetCleaningBedData(ward1, DateTime.Today.AddSeconds(8));

            // Find bed id
            var bedWithKnownOrder = (from b in ward1Beds where b.Room.Ward == "WARD1" && b.Room.Name == "109" && b.Name == "1" select b).First();

            // There should be 4 records for this bed (1 before admission, 2 orders linked to the admission, 1 after discharge)
            Assert.AreEqual(4, bedWithKnownOrder.AvailableTimes.Count());

            // Get the record for the procedure that starts at 3pm
            var freeTimeForTheTestBed = bedWithKnownOrder.AvailableTimes.First(b => b.StartTime == DateTime.Now.Date.AddHours(15));

            Assert.IsNotNull(freeTimeForTheTestBed);
            Assert.AreEqual(freeTimeForTheTestBed.EndTime, DateTime.Now.Date.AddSeconds(16));
        }

        [TestMethod]
        public void BedsWithNoAdmissionShouldBeAvailableAllDayTest()
        {
            var ward1Beds = server.GetCleaningBedData(ward1, DateTime.Today.AddHours(8));

            var bedWithNoAdmission = (from b in ward1Beds where b.Room.Name == "115" && b.Name == "1" select b).First();

            // Should be available from 8am to 8pm as there is no admission today
            Assert.AreEqual(DateTime.Today.AddHours(8), bedWithNoAdmission.AvailableTimes.First().StartTime);
            Assert.AreEqual(DateTime.Today.AddHours(20), bedWithNoAdmission.AvailableTimes.First().EndTime);
        }

        [TestMethod]
        public void BedWithAdmissionYesterdayRequiresDeepCleanTest()
        {
            var ward3Beds = server.GetCleaningBedData(ward3, DateTime.Today.AddSeconds(8));

            string roomName = "301";
            string bedName = "4";
            var bedWithAdmissionYesterday = (from b in ward3Beds where b.Room.Name == roomName && b.Name == bedName select b).First();

            Assert.AreEqual(BedStatus.RequiresDeepClean, bedWithAdmissionYesterday.CurrentStatus);

            // Add a cleaning event yesterday, should still require a deep clean
            server.MarkBedAsClean(bedWithAdmissionYesterday.BedId, DateTime.Today.AddDays(-1).AddHours(17), CLIENT_NAME);

            ward3Beds = server.GetCleaningBedData(ward3, DateTime.Today.AddHours(8));
            bedWithAdmissionYesterday = (from b in ward3Beds where b.Room.Name == roomName && b.Name == bedName select b).First();

            Assert.AreEqual(BedStatus.RequiresDeepClean, bedWithAdmissionYesterday.CurrentStatus);

            // Add a deep clean event this morning, should now be clean
            server.MarkBedAsClean(bedWithAdmissionYesterday.BedId, DateTime.Today.AddSeconds(8), CLIENT_NAME);

            ward3Beds = server.GetCleaningBedData(ward3, DateTime.Today.AddSeconds(8));
            bedWithAdmissionYesterday = (from b in ward3Beds where b.Room.Name == roomName && b.Name == bedName select b).First();

            Assert.AreEqual(BedStatus.Clean, bedWithAdmissionYesterday.CurrentStatus);
        }

        [TestMethod]
        public void BedWithAdmissionArrivingAfterStartOfDayShouldHaveAvailabilityBeforeThat()
        {
            var ward3Beds = server.GetCleaningBedData(ward3, DateTime.Today.AddHours(8));

            string roomName = "302";
            string bedName = "4";
            var bedWithAdmissionAfterStartOfDay = (from b in ward3Beds where b.Room.Name == roomName && b.Name == bedName select b).First();

            // Should be available from the start of the cleaning day at 8am until 9am when admitted
            var availabilityPeriodStartingAtEight = bedWithAdmissionAfterStartOfDay.AvailableTimes.FirstOrDefault(b => b.StartTime == DateTime.Now.Date.AddHours(8));

            Assert.IsNotNull(availabilityPeriodStartingAtEight);
            Assert.AreEqual(DateTime.Today.AddHours(9), availabilityPeriodStartingAtEight.EndTime);
            Assert.IsFalse(availabilityPeriodStartingAtEight.IsDueToDischarge);
        }

        [TestMethod]
        public void BedWithDischargeTodayShouldHaveAvailabilityAfterThat()
        {
            var ward3Beds = server.GetCleaningBedData(ward3, DateTime.Today.AddHours(8));

            string roomName = "302";
            string bedName = "4";
            var bedWithDischargeToday = (from b in ward3Beds where b.Room.Name == roomName && b.Name == bedName select b).First();

            // Should be available from the discharge at midday to the end of the cleaning day at 8pm
            var availabilityPeriodStartingAtTwelve = bedWithDischargeToday.AvailableTimes.FirstOrDefault(b => b.StartTime == DateTime.Today.AddHours(12));

            Assert.IsNotNull(availabilityPeriodStartingAtTwelve);
            Assert.AreEqual(DateTime.Today.AddHours(20), availabilityPeriodStartingAtTwelve.EndTime);
            Assert.IsTrue(availabilityPeriodStartingAtTwelve.IsDueToDischarge);
        }

        [TestMethod]
        public void BedWithTwoAdmissionsTodayShouldHaveAvailabilityBetweenThem()
        {
            var ward3Beds = server.GetCleaningBedData(ward3, DateTime.Today.AddHours(8));

            string roomName = "303";
            string bedName = "1";
            var bedWithTwoAdmissionsToday = (from b in ward3Beds where b.Room.Name == roomName && b.Name == bedName select b).First();

            // Should be a gap for cleaning between the 2 admissions
            var firstPeriodThatStartsAtMidday = bedWithTwoAdmissionsToday.AvailableTimes.First(b => b.StartTime == DateTime.Now.Date.AddHours(12));
            Assert.AreEqual(DateTime.Today.AddHours(14), firstPeriodThatStartsAtMidday.EndTime);
            Assert.IsTrue(firstPeriodThatStartsAtMidday.IsDueToDischarge);

            // Should be a gap for cleaning after the 2nd admission
            var secondPeriodThatStartsAtFivePm = bedWithTwoAdmissionsToday.AvailableTimes.First(b => b.StartTime == DateTime.Now.Date.AddHours(17));
            Assert.AreEqual(DateTime.Today.AddHours(20), secondPeriodThatStartsAtFivePm.EndTime);
            Assert.IsTrue(secondPeriodThatStartsAtFivePm.IsDueToDischarge);
        }

        [TestMethod]
        public void BedWithMrsaPatientAdmittedThisMorningShouldHavePositiveMrsaStatus()
        {
            var ward3Beds = server.GetCleaningBedData(ward3, DateTime.Today.AddHours(8));

            string roomName = "302";
            string bedName = "4";
            var bedWithMrsaPatientAdmittedThisMorning = (from b in ward3Beds where b.Room.Name == roomName && b.Name == bedName select b).First();

            Assert.IsTrue(bedWithMrsaPatientAdmittedThisMorning.CriticalCareIndicators.IsMrsaRisk);
        }

        [TestMethod]
        public void AddDeleteNoteTest()
        {
            var ward1Beds = server.GetCleaningBedData(ward1, DateTime.Today.AddHours(8));
            var bed109 = (from b in ward1Beds where b.Room.Ward == "WARD1" && b.Room.Name == "109" && b.Name == "1" select b).First();

            int noteCountBefore = bed109.Notes.Count();

            // Add a note
            bed109 = server.AddBedNote(bed109.BedId, "test add", DateTime.Today.AddHours(12), CLIENT_NAME);

            Assert.AreEqual(bed109.Notes.Count(), noteCountBefore + 1);

            // Delete the note
            noteCountBefore = bed109.Notes.Count();
            bed109 = server.DeleteBedNote(bed109.Notes.Last().NoteId, DateTime.Today.AddHours(14), CLIENT_NAME);

            Assert.AreEqual(bed109.Notes.Count(), noteCountBefore - 1);
        }

        [TestMethod]
        public void CheckBedWeKnowIsARadiationRiskTest()
        {
            // ADM011 has a procedure at 2pm that will make them a radiation risk at this time, this should be shown in the cleaning data also.
            var ward1Beds = server.GetCleaningBedData(ward1, DateTime.Today.AddHours(14));
            var ward1room112bed1 = (from b in ward1Beds where b.Room.Ward == "WARD1" && b.Room.Name == "112" && b.Name == "1" select b).First();

            Assert.IsTrue(ward1room112bed1.CriticalCareIndicators.IsRadiationRisk);

            // And test that the next bed is not a radiation risk
            var ward1room112bed2 = (from b in ward1Beds where b.Room.Ward == "WARD1" && b.Room.Name == "112" && b.Name == "2" select b).First();

            Assert.IsFalse(ward1room112bed2.CriticalCareIndicators.IsRadiationRisk);
        }
    }
}

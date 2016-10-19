using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WCS.Services.DataServices.Data;
using System.Linq;
using WCS.Services.DataServices;
using WCS.Data.EF;

namespace WCS.Services.Test
{
    [TestClass]
    public class DischargesTest
    {
        ServerFacade server = new ServerFacade();
        private const string CLIENT_NAME = "WCS-DEMO01";
        LocationCodes ward3 = new LocationCodes() { "WARD3" };

        [TestMethod]
        public void GetAllGetDischargesBedDataTest()
        {
            // Get the discharge data at 12am
            var allWardsDischargesBedData = server.GetDischargesBedData(new LocationCodes(), DateTime.Today.AddHours(12));

            // There should be a record for every bed
            Assert.AreEqual(62, allWardsDischargesBedData.Count);
        }

        [TestMethod]
        public void GetAllFreeBedTimesForDateTest()
        {
            // Get the discharge data at 8am
            var ward3DischargesBedData = server.GetDischargesBedData(ward3, DateTime.Today.AddHours(8));

            var bedWithKnownAdmissions = (from b in ward3DischargesBedData where b.Room.Ward == "WARD3" && b.Room.Name == "303" && b.Name == "1" select b).First();

            // At 8am this bed would have no admission
            Assert.IsNull(bedWithKnownAdmissions.CurrentAdmission);

            // Get the discharge data at 10am
            ward3DischargesBedData = server.GetDischargesBedData(ward3, DateTime.Today.AddHours(9));
            bedWithKnownAdmissions = (from b in ward3DischargesBedData where b.Room.Ward == "WARD3" && b.Room.Name == "303" && b.Name == "1" select b).First();

            // Now there should be an admission ADM062
            Assert.AreEqual("ADM062", bedWithKnownAdmissions.CurrentAdmission.DisplayCode);

            // Get the discharge data at 12am
            ward3DischargesBedData = server.GetDischargesBedData(ward3, DateTime.Today.AddHours(12));
            bedWithKnownAdmissions = (from b in ward3DischargesBedData where b.Room.Ward == "WARD3" && b.Room.Name == "303" && b.Name == "1" select b).First();

            // At 12am this bed would have no admission again
            Assert.IsNull(bedWithKnownAdmissions.CurrentAdmission);

            // Get the discharge data at 2pm
            ward3DischargesBedData = server.GetDischargesBedData(ward3, DateTime.Today.AddHours(14));
            bedWithKnownAdmissions = (from b in ward3DischargesBedData where b.Room.Ward == "WARD3" && b.Room.Name == "303" && b.Name == "1" select b).First();

            // Now there should be an admission ADM063
            Assert.AreEqual("ADM063", bedWithKnownAdmissions.CurrentAdmission.DisplayCode);

            // Get the discharge data at 4.30pm
            ward3DischargesBedData = server.GetDischargesBedData(ward3, DateTime.Today.AddHours(16).AddMinutes(30));
            bedWithKnownAdmissions = (from b in ward3DischargesBedData where b.Room.Ward == "WARD3" && b.Room.Name == "303" && b.Name == "1" select b).First();

            // Should still be an admission ADM063
            Assert.AreEqual("ADM063", bedWithKnownAdmissions.CurrentAdmission.DisplayCode);

            int adm063Id = bedWithKnownAdmissions.CurrentAdmission.AdmissionId;

            // Update the estimated discharge to 4pm at 3pm
            bedWithKnownAdmissions = server.UpdateEstimatedDischargeDate(adm063Id, DateTime.Today.AddHours(16), DateTime.Today.AddHours(15), CLIENT_NAME);

            // Because the update was performed at 3pm the admission should still be there
            Assert.AreEqual("ADM063", bedWithKnownAdmissions.CurrentAdmission.DisplayCode);

            // Now get the discharge data at 4.30pm again
            ward3DischargesBedData = server.GetDischargesBedData(ward3, DateTime.Today.AddHours(16).AddMinutes(30));
            bedWithKnownAdmissions = (from b in ward3DischargesBedData where b.Room.Ward == "WARD3" && b.Room.Name == "303" && b.Name == "1" select b).First();

            // Should no longer have an admission
            Assert.IsNull(bedWithKnownAdmissions.CurrentAdmission);
        }

        [TestMethod]
        public void UpdatingEstimatedDischargeDateShouldCreateAnUpdateRecordTest()
        {
            // Get the discharge data at 12am
            var allWardsDischargesBedData = server.GetDischargesBedData(new LocationCodes(), DateTime.Today.AddHours(12));

            var ward1room101bed1 = (from b in allWardsDischargesBedData where b.Room.Ward == "WARD1" && b.Room.Name == "101" && b.Name == "1" select b).First();

            var adm001 = ward1room101bed1.CurrentAdmission;

            int countBefore = adm001.Updates.Count();

            // Update the estimated discharge to 5.15pm at 12am
            ward1room101bed1 = server.UpdateEstimatedDischargeDate(adm001.AdmissionId, DateTime.Today.AddHours(17).AddMinutes(15), DateTime.Today.AddHours(12), CLIENT_NAME);

            Assert.AreEqual(countBefore + 1, ward1room101bed1.CurrentAdmission.Updates.Count());

            var updateOfInterest = ward1room101bed1.CurrentAdmission.Updates.Last();

            Assert.AreEqual(WCSUpdateTypes.EstimatedDischargeDateUpdated, updateOfInterest.Type);

            Assert.AreEqual(DateTime.Today.AddHours(12), ward1room101bed1.EstimatedDischargeDateLastUpdated);
        }
    }
}

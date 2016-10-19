using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WCS.Services.DataServices.Data;
using System.Linq;
using WCS.Services.DataServices;
using WCS.Data.EF;

namespace WCS.Services.Test
{
    [TestClass]
    public class AdmissionsTest
    {
        ServerFacade server = new ServerFacade();

        LocationCodes admissionsLocations = new LocationCodes() { "ADM1", "ADM2" };

        [TestMethod]
        public void GetAdmissionsDataTest()
        {
            // Get the admissions data at 12am
            var admissionsData = server.GetAdmissionsData(admissionsLocations, DateTime.Today.AddHours(12));

            // There should be a record for every bed
            Assert.AreEqual(62, admissionsData.Beds.Count);

            // Check the admissions count is correct
            Assert.AreEqual(19, admissionsData.Admissions.Where(a => a.Location.Name == "ADM1").Count());
            Assert.AreEqual(2, admissionsData.Admissions.Where(a => a.Location.Name == "ADM2").Count());
        }
    }
}

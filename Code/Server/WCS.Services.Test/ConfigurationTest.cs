using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WCS.Services.DataServices;
using WCS.Services.DataServices.Data;

namespace WCS.Services.Test
{
    [TestClass]
    public class ConfigurationTest
    {
        private const string CLIENT_NAME = "WCS-DEMO01";
        ServerFacade server = new ServerFacade();

        [TestMethod]
        public void GetConfigurationsTest()
        {
            var conf = server.GetConfigurations(CLIENT_NAME);

            Assert.IsNotNull(conf);
            Assert.AreEqual(conf.DeviceName, CLIENT_NAME);
            Assert.AreEqual(conf.Instances.Count, 8);

            var firstCleaningConfiguration = (from i in conf.Instances where i.Type == DeviceConfigurationType.Cleaning select i).First();

            Assert.AreEqual("WARD1", firstCleaningConfiguration.LocationCode);
            Assert.IsNotNull((from vl in firstCleaningConfiguration.VisibleLocations where vl.Code == "WARD1" select vl).SingleOrDefault());
            Assert.IsNotNull((from vl in firstCleaningConfiguration.VisibleLocations where vl.Code == "WARD2" select vl).SingleOrDefault());
            Assert.IsNotNull((from vl in firstCleaningConfiguration.VisibleLocations where vl.Code == "WARD3" select vl).SingleOrDefault());
        }

        [TestMethod]
        public void GetLatestPollingTimeoutsTest()
        {
            PollingTimeouts pts = new DataServices.DataServices().GetLatestPollingTimeouts(CLIENT_NAME, 1);

            Assert.IsNotNull(pts);
            Assert.AreEqual(pts.CleaningBedDataTimeout, 30);
            Assert.AreEqual(pts.OrderTimeout, 10);
            Assert.AreEqual(pts.PresenceTimeout, 30);
            Assert.AreEqual(pts.RfidTimeout, 30);
            Assert.AreEqual(pts.ConfigurationTimeout, 60);
            Assert.AreEqual(pts.DischargeTimeout, 30);
            Assert.AreEqual(pts.AdmissionsTimeout, 30);
        }
    }
}

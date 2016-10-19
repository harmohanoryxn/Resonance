using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WCS.Services.DataServices.Data;
using WCS.Core;

namespace WCS.Services.Test
{
    [TestClass]
    public class DevicesTest
    {
        private const string CLIENT_NAME = "WCS-DEMO01";
        ServerFacade server = new ServerFacade();

        [TestMethod]
        public void GetPresenceDataTest()
        {
            var connections = server.GetPresenceData(DateTime.Now).ToList();

            Assert.IsTrue(connections.Any());
        }
    }
}

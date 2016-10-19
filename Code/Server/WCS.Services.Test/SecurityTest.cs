using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WCS.Services.DataServices.Data;

namespace WCS.Services.Test
{
    [TestClass]
    public class SecurityTest
    {
        ServerFacade server = new ServerFacade();
        private const string CLIENT_NAME = "WCS-DEMO01";

        [TestMethod]
        public void AuthenticateSuccessTest()
        {
            var auth = server.Authenticate(CLIENT_NAME, "0000", DateTime.Now);

            Assert.IsTrue(auth.IsAuthenticated);
        }

        [TestMethod]
        public void AuthenticateFailTest()
        {
            var auth = server.Authenticate(CLIENT_NAME, "5555", DateTime.Now);

            Assert.IsFalse(auth.IsAuthenticated);
        }

    }
}

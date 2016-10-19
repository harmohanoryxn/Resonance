using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cloudmaster.WCS.WebApps.Cleaning;
using Cloudmaster.WCS.WebApps.Cleaning.Controllers;

namespace Cloudmaster.WCS.WebApps.Cleaning.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestMethod]
        public void Overview()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.Overview() as ViewResult;

            // Assert
            Assert.AreEqual("Modify this template to jump-start your ASP.NET MVC application.", result.ViewBag.Message);
        }

        [TestMethod]
        public void Details()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.Details(1) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Summary()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.Summary() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }
    }
}

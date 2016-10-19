using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WCS.Shared.Converters;

namespace WCS.Services.Test.Convertors
{
    [TestClass]
    public class NotificationConverterTest
    {
        [TestMethod]
        public void NotificationTypeToVisibilityConverterTest()
        {
            var converter = new NotificationTypeToVisibilityConverter();

            var shouldBeCollapsed = converter.Convert(new object[] { NotificationType.Prep, NotificationType.Physio }, typeof(Visibility), string.Empty, null);

            Assert.AreEqual(shouldBeCollapsed, Visibility.Collapsed);

            var shouldBeVisible = converter.Convert(new object[] { NotificationType.Physio, NotificationType.Physio }, typeof(Visibility), string.Empty, null);

            Assert.AreEqual(shouldBeVisible, Visibility.Visible);
        }
    }
}

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Media;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using Cloudmaster.WCS.DataServicesInvoker.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using WCS.Shared.Schedule;

namespace WCS.Services.Test
{
    [TestClass]
    public class LocationClockViewModelTest
    {


        [TestMethod]
        public void SyncronizeTest()
        {
            var locationClockViewModel = new LocationClockViewModel("ACC1");

            var detection = new Detection();

            var location = new Location();


            

           // locationClockViewModel.CurrentLocation = "ACC2";

            //locationClockViewModel.SetLatestDetection(detection);

            //Assert.IsTrue(locationClockViewModel.IsInCurrentLocation);

        }

    }
}

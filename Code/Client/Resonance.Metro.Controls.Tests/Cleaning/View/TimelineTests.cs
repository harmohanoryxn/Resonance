using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Resonance.Metro.Controls.Cleaning.View;

namespace Resonance.Metro.Controls.Tests.Cleaning.View
{
    [TestClass]
    public class TimelineTests
    {
        private double defaultWidth = 2400.00;

        [TestMethod]
        public void GetOneHourWidthIn24HoursTest()
        {
            var width = TimelineControl.GetOneHourWidth(defaultWidth, 0, 24);

            Assert.AreEqual(width, 100.00);
        }

        [TestMethod]
        public void GetOneHourWidthIn12HoursTest()
        {
            var width = TimelineControl.GetOneHourWidth(defaultWidth, 0, 12);

            Assert.AreEqual(width, 200.00);
        }

        [TestMethod]
        public void GetOneHourWidthIn8HoursTest()
        {
            var width = TimelineControl.GetOneHourWidth(defaultWidth, 8, 16);

            Assert.AreEqual(width, 300.00);
        }

        [TestMethod]
        public void GetOneHourWidthZeoWidthTest()
        {
            var width = TimelineControl.GetOneHourWidth(0, 8, 16);

            Assert.AreEqual(width, 0);
        }

        [TestMethod]
        public void GetOneHourWidthNotEqualTest()
        {
            var width = TimelineControl.GetOneHourWidth(defaultWidth, 0, 6);

            Assert.AreNotEqual(width, 100.00);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetOneHourWidthInvalidStartHoursArguementTest()
        {
            TimelineControl.GetOneHourWidth(defaultWidth, 36, 16);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetOneHourWidthInvalidEndHoursArguementTest()
        {
            TimelineControl.GetOneHourWidth(defaultWidth, 12, 54);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetOneHourWidthInvalidStartHoursessThanEndHoursArguementTest()
        {
            TimelineControl.GetOneHourWidth(defaultWidth, 12, 6);
        }
    }
}

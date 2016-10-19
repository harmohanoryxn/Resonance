using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Cloudmaster.WCS.DataServicesInvoker.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WCS.Services.Test
{
    [TestClass]
    public class ClientAdmissionStatusTests
    {
        [TestMethod]
        public void IsSetTest()
        {
            var selectedStatuses = MultiSelectAdmissionStatusFlag.Admitted;

            Assert.IsTrue(selectedStatuses.IsSet(MultiSelectAdmissionStatusFlag.Admitted));
            Assert.IsFalse(selectedStatuses.IsSet(MultiSelectAdmissionStatusFlag.Discharged));
            Assert.IsFalse(selectedStatuses.IsSet(MultiSelectAdmissionStatusFlag.Registered));
            
            selectedStatuses = (MultiSelectAdmissionStatusFlag.Admitted | MultiSelectAdmissionStatusFlag.Registered);

            Assert.IsTrue(selectedStatuses.IsSet(MultiSelectAdmissionStatusFlag.Admitted));
            Assert.IsFalse(selectedStatuses.IsSet(MultiSelectAdmissionStatusFlag.Discharged));
            Assert.IsTrue(selectedStatuses.IsSet(MultiSelectAdmissionStatusFlag.Registered));

            selectedStatuses = (MultiSelectAdmissionStatusFlag.Admitted | MultiSelectAdmissionStatusFlag.Registered | MultiSelectAdmissionStatusFlag.Discharged);

            Assert.IsTrue(selectedStatuses.IsSet(MultiSelectAdmissionStatusFlag.Admitted));
            Assert.IsTrue(selectedStatuses.IsSet(MultiSelectAdmissionStatusFlag.Discharged));
            Assert.IsTrue(selectedStatuses.IsSet(MultiSelectAdmissionStatusFlag.Registered));

            selectedStatuses = (MultiSelectAdmissionStatusFlag.Registered | MultiSelectAdmissionStatusFlag.Discharged);

            Assert.IsFalse(selectedStatuses.IsSet(MultiSelectAdmissionStatusFlag.Admitted));
            Assert.IsTrue(selectedStatuses.IsSet(MultiSelectAdmissionStatusFlag.Discharged));
            Assert.IsTrue(selectedStatuses.IsSet(MultiSelectAdmissionStatusFlag.Registered));
        }

        [TestMethod]
        public void IsSetMultipleTest()
        {
            var selectedStatuses = (MultiSelectAdmissionStatusFlag.Admitted | MultiSelectAdmissionStatusFlag.Discharged);

            Assert.IsTrue(selectedStatuses.IsSet(MultiSelectAdmissionStatusFlag.Admitted));
            Assert.IsTrue(selectedStatuses.IsSet(MultiSelectAdmissionStatusFlag.Discharged));
            Assert.IsFalse(selectedStatuses.IsSet(MultiSelectAdmissionStatusFlag.Registered));

            // Test Multiple 

            Assert.IsTrue(selectedStatuses.IsSet(MultiSelectAdmissionStatusFlag.Admitted | MultiSelectAdmissionStatusFlag.Registered));
            Assert.IsTrue(selectedStatuses.IsSet(MultiSelectAdmissionStatusFlag.Admitted | MultiSelectAdmissionStatusFlag.Discharged));
            Assert.IsTrue(selectedStatuses.IsSet(MultiSelectAdmissionStatusFlag.Discharged | MultiSelectAdmissionStatusFlag.Registered));
        }

        [TestMethod]
        public void ToggleTest()
        {
            var selectedStatuses = MultiSelectAdmissionStatusFlag.Admitted;

            Assert.IsTrue(selectedStatuses.IsSet(MultiSelectAdmissionStatusFlag.Admitted));
            Assert.IsFalse(selectedStatuses.IsSet(MultiSelectAdmissionStatusFlag.Discharged));
            Assert.IsFalse(selectedStatuses.IsSet(MultiSelectAdmissionStatusFlag.Registered));

            // Method is called as below so that property in view is updated and triggers refresh in DepartmentScheduleViewModel

            selectedStatuses = selectedStatuses.Toggle(MultiSelectAdmissionStatusFlag.Registered);

            Assert.IsTrue(selectedStatuses.IsSet(MultiSelectAdmissionStatusFlag.Admitted));
            Assert.IsFalse(selectedStatuses.IsSet(MultiSelectAdmissionStatusFlag.Discharged));
            Assert.IsTrue(selectedStatuses.IsSet(MultiSelectAdmissionStatusFlag.Registered));

            selectedStatuses = selectedStatuses.Toggle(MultiSelectAdmissionStatusFlag.Discharged);

            Assert.IsTrue(selectedStatuses.IsSet(MultiSelectAdmissionStatusFlag.Admitted));
            Assert.IsTrue(selectedStatuses.IsSet(MultiSelectAdmissionStatusFlag.Discharged));
            Assert.IsTrue(selectedStatuses.IsSet(MultiSelectAdmissionStatusFlag.Registered));

            selectedStatuses = selectedStatuses.Toggle(MultiSelectAdmissionStatusFlag.Admitted);

            Assert.IsFalse(selectedStatuses.IsSet(MultiSelectAdmissionStatusFlag.Admitted));
            Assert.IsTrue(selectedStatuses.IsSet(MultiSelectAdmissionStatusFlag.Discharged));
            Assert.IsTrue(selectedStatuses.IsSet(MultiSelectAdmissionStatusFlag.Registered));
        }

        [TestMethod]
        public void ClearTest()
        {
            var selectedStatuses = (MultiSelectAdmissionStatusFlag.Admitted | MultiSelectAdmissionStatusFlag.Registered | MultiSelectAdmissionStatusFlag.Discharged);
             
            Assert.IsTrue(selectedStatuses.IsSet(MultiSelectAdmissionStatusFlag.Admitted));
            Assert.IsTrue(selectedStatuses.IsSet(MultiSelectAdmissionStatusFlag.Discharged));
            Assert.IsTrue(selectedStatuses.IsSet(MultiSelectAdmissionStatusFlag.Registered));

            // Method is called as below so that property in view is updated and triggers refresh in DepartmentScheduleViewModel

            selectedStatuses = selectedStatuses.Clear(MultiSelectAdmissionStatusFlag.Registered);

            Assert.IsTrue(selectedStatuses.IsSet(MultiSelectAdmissionStatusFlag.Admitted));
            Assert.IsTrue(selectedStatuses.IsSet(MultiSelectAdmissionStatusFlag.Discharged));
            Assert.IsFalse(selectedStatuses.IsSet(MultiSelectAdmissionStatusFlag.Registered));

            selectedStatuses = selectedStatuses.Clear(MultiSelectAdmissionStatusFlag.Discharged);

            Assert.IsTrue(selectedStatuses.IsSet(MultiSelectAdmissionStatusFlag.Admitted));
            Assert.IsFalse(selectedStatuses.IsSet(MultiSelectAdmissionStatusFlag.Discharged));
            Assert.IsFalse(selectedStatuses.IsSet(MultiSelectAdmissionStatusFlag.Registered));

            selectedStatuses = selectedStatuses.Clear(MultiSelectAdmissionStatusFlag.Admitted);

            Assert.IsFalse(selectedStatuses.IsSet(MultiSelectAdmissionStatusFlag.Admitted));
            Assert.IsFalse(selectedStatuses.IsSet(MultiSelectAdmissionStatusFlag.Discharged));
            Assert.IsFalse(selectedStatuses.IsSet(MultiSelectAdmissionStatusFlag.Registered));
        }
    }
}

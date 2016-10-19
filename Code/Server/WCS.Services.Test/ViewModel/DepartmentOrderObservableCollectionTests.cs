using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using Cloudmaster.WCS.DataServicesInvoker.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WCS.Shared.Department;
using WCS.Shared.Orders;
using WCS.Shared.Ward.Schedule;
using WCS.Shared.Schedule;

namespace WCS.Services.Test
{
    [TestClass]
    public class DepartmentOrderObservableCollectionTests
    {
        [TestMethod]
        public void FilterByLocationTest()
        {
            var patient = OrderViewModelTest.CreateDefaultPatient();
            var admission = OrderViewModelTest.CreateDefaultAdmission(patient);

            var order = OrderViewModelTest.CreateDefaultOrder(admission);
            var orders = new List<Order>(1) { order };

            var topPatient = new TopPatient(patient, admission, orders);

            var patientViewModel = new PatientViewModel(topPatient);

            var collection = new DepartmentOrderObservableCollection(TransformFunction)
            {
                PatientAdmissionType = MultiSelectAdmissionTypeFlag.In,
                OrderStatusFilter = OrderStatus.InProgress,
                AdmissionStatusFlagFilter = MultiSelectAdmissionStatusFlag.Admitted,
                                     Location = "US" };

            collection.Synchronise(orders, patientViewModel);

            Assert.AreEqual(collection.Count, 1);

            collection.Location = "CT";

            Assert.AreEqual(collection.Count, 0);
        }


        [TestMethod]
        public void FilterByAdmissionStatusTest()
        {
            // Test for in-patients with in progress orders

            var patient = OrderViewModelTest.CreateDefaultPatient();
            var admission = OrderViewModelTest.CreateDefaultAdmission(patient);

            FilterByAdmissionStatus(patient, admission, MultiSelectAdmissionTypeFlag.In, OrderStatus.InProgress);

            // Test for in-patients with completed orders

            patient = OrderViewModelTest.CreateDefaultPatient();
            admission = OrderViewModelTest.CreateDefaultAdmission(patient);

            FilterByAdmissionStatus(patient, admission, MultiSelectAdmissionTypeFlag.In, OrderStatus.Completed);

            // Test for in-patients with cancelled orders

            patient = OrderViewModelTest.CreateDefaultPatient();
            admission = OrderViewModelTest.CreateDefaultAdmission(patient);

            FilterByAdmissionStatus(patient, admission, MultiSelectAdmissionTypeFlag.In, OrderStatus.Cancelled);

            // Out-patients

            // Test for Out-patients with in progress orders

            patient = OrderViewModelTest.CreateDefaultPatient();
            admission = OrderViewModelTest.CreateDefaultAdmission(patient);
            admission.Type = AdmissionType.Out;

            FilterByAdmissionStatus(patient, admission, MultiSelectAdmissionTypeFlag.Out, OrderStatus.InProgress);

            // Test for Out-patients with completed orders

            patient = OrderViewModelTest.CreateDefaultPatient();
            admission = OrderViewModelTest.CreateDefaultAdmission(patient);
            admission.Type = AdmissionType.Out;

            FilterByAdmissionStatus(patient, admission, MultiSelectAdmissionTypeFlag.Out, OrderStatus.Completed);

            // Test for Out-patients with cancelled orders

            patient = OrderViewModelTest.CreateDefaultPatient();
            admission = OrderViewModelTest.CreateDefaultAdmission(patient);
            admission.Type = AdmissionType.Out;

            FilterByAdmissionStatus(patient, admission, MultiSelectAdmissionTypeFlag.Out, OrderStatus.Cancelled);

            // Day-patients

            // Test for Day-patients with in progress orders

            patient = OrderViewModelTest.CreateDefaultPatient();
            admission = OrderViewModelTest.CreateDefaultAdmission(patient);
            admission.Type = AdmissionType.Day;

            FilterByAdmissionStatus(patient, admission, MultiSelectAdmissionTypeFlag.Day, OrderStatus.InProgress);

            // Test for Day-patients with completed orders

            patient = OrderViewModelTest.CreateDefaultPatient();
            admission = OrderViewModelTest.CreateDefaultAdmission(patient);
            admission.Type = AdmissionType.Day;

            FilterByAdmissionStatus(patient, admission, MultiSelectAdmissionTypeFlag.Day, OrderStatus.Completed);

            // Test for Day-patients with cancelled orders

            patient = OrderViewModelTest.CreateDefaultPatient();
            admission = OrderViewModelTest.CreateDefaultAdmission(patient);
            admission.Type = AdmissionType.Day;

            FilterByAdmissionStatus(patient, admission, MultiSelectAdmissionTypeFlag.Day, OrderStatus.Cancelled);
        }

        public void FilterByAdmissionStatus(Patient patient, Admission admission, MultiSelectAdmissionTypeFlag type, OrderStatus status)
        {
            var order = OrderViewModelTest.CreateDefaultOrder(admission);
            var orders = new List<Order>(1) {order};

            var topPatient = new TopPatient(patient, admission, orders);

            var patientViewModel = new PatientViewModel(topPatient);

            var collection = new DepartmentOrderObservableCollection(TransformFunction)
                                 {PatientAdmissionType = type, OrderStatusFilter = status};

            // Test Registered

            admission.Status = AdmissionStatus.Registered;
            order = OrderViewModelTest.CreateDefaultOrder(admission);
            order.Status = status;
            orders = new List<Order>(1) { order };

            collection.Synchronise(orders, patientViewModel);

            collection.AdmissionStatusFlagFilter = MultiSelectAdmissionStatusFlag.Admitted;
            Assert.AreEqual(collection.Count, 0);

            collection.AdmissionStatusFlagFilter = MultiSelectAdmissionStatusFlag.Discharged;
            Assert.AreEqual(collection.Count, 0);

            collection.AdmissionStatusFlagFilter = MultiSelectAdmissionStatusFlag.Registered;
            Assert.AreEqual(collection.Count, 1);

            collection.AdmissionStatusFlagFilter = MultiSelectAdmissionStatusFlag.Unknown;
            Assert.AreEqual(collection.Count, 0);

            // Test Admitted

            admission.Status = AdmissionStatus.Admitted;
            order = OrderViewModelTest.CreateDefaultOrder(admission);
            order.Status = status;
            orders = new List<Order>(1) { order };

            collection.Synchronise(orders, patientViewModel);

            collection.AdmissionStatusFlagFilter = MultiSelectAdmissionStatusFlag.Admitted;
            Assert.AreEqual(collection.Count, 1);

            collection.AdmissionStatusFlagFilter = MultiSelectAdmissionStatusFlag.Discharged;
            Assert.AreEqual(collection.Count, 0);

            collection.AdmissionStatusFlagFilter = MultiSelectAdmissionStatusFlag.Registered;
            Assert.AreEqual(collection.Count, 0);

            collection.AdmissionStatusFlagFilter = MultiSelectAdmissionStatusFlag.Unknown;
            Assert.AreEqual(collection.Count, 0);

            // Test Discharged

            admission.Status = AdmissionStatus.Discharged;
            order = OrderViewModelTest.CreateDefaultOrder(admission);
            order.Status = status;
            orders = new List<Order>(1) { order };

            collection.Synchronise(orders, patientViewModel);

            collection.AdmissionStatusFlagFilter = MultiSelectAdmissionStatusFlag.Admitted;
            Assert.AreEqual(collection.Count, 0);

            collection.AdmissionStatusFlagFilter = MultiSelectAdmissionStatusFlag.Discharged;
            Assert.AreEqual(collection.Count, 1);

            collection.AdmissionStatusFlagFilter = MultiSelectAdmissionStatusFlag.Registered;
            Assert.AreEqual(collection.Count, 0);

            collection.AdmissionStatusFlagFilter = MultiSelectAdmissionStatusFlag.Unknown;
            Assert.AreEqual(collection.Count, 0);
        }

        [TestMethod]
        public void FilterByAdmissionTypeTest()
        {
            var patient = OrderViewModelTest.CreateDefaultPatient();
            var admission = OrderViewModelTest.CreateDefaultAdmission(patient);

            var order = OrderViewModelTest.CreateDefaultOrder(admission);
            var orders = new List<Order>(1) { order };

            var topPatient = new TopPatient(patient, admission, orders);

            var patientViewModel = new PatientViewModel(topPatient);

            var collection = new DepartmentOrderObservableCollection(TransformFunction);

            // Test In

            admission.Type = AdmissionType.In;
            order = OrderViewModelTest.CreateDefaultOrder(admission);
            orders = new List<Order>(1) { order };

            collection.Synchronise(orders, patientViewModel);

            collection.PatientAdmissionType = MultiSelectAdmissionTypeFlag.In;
            Assert.AreEqual(collection.Count, 1);

            collection.PatientAdmissionType = MultiSelectAdmissionTypeFlag.Out;
            Assert.AreEqual(collection.Count, 0);

            collection.PatientAdmissionType = MultiSelectAdmissionTypeFlag.Day;
            Assert.AreEqual(collection.Count, 0);

            collection.PatientAdmissionType = MultiSelectAdmissionTypeFlag.Unknown;
            Assert.AreEqual(collection.Count, 0);

            collection.PatientAdmissionType = MultiSelectAdmissionTypeFlag.All;
            Assert.AreEqual(collection.Count, 1);

            // Test Out

            admission.Type = AdmissionType.Out;
            order = OrderViewModelTest.CreateDefaultOrder(admission);
            orders = new List<Order>(1) { order };

            collection.Synchronise(orders, patientViewModel);

            collection.PatientAdmissionType = MultiSelectAdmissionTypeFlag.In;
            Assert.AreEqual(collection.Count, 0);

            collection.PatientAdmissionType = MultiSelectAdmissionTypeFlag.Out;
            Assert.AreEqual(collection.Count, 1);

            collection.PatientAdmissionType = MultiSelectAdmissionTypeFlag.Day;
            Assert.AreEqual(collection.Count, 0);

            collection.PatientAdmissionType = MultiSelectAdmissionTypeFlag.Unknown;
            Assert.AreEqual(collection.Count, 0);

            collection.PatientAdmissionType = MultiSelectAdmissionTypeFlag.All;
            Assert.AreEqual(collection.Count, 1);

            // Test Day

            admission.Type = AdmissionType.Day;
            order = OrderViewModelTest.CreateDefaultOrder(admission);
            orders = new List<Order>(1) { order };

            collection.Synchronise(orders, patientViewModel);

            collection.PatientAdmissionType = MultiSelectAdmissionTypeFlag.In;
            Assert.AreEqual(collection.Count, 0);

            collection.PatientAdmissionType = MultiSelectAdmissionTypeFlag.Out;
            Assert.AreEqual(collection.Count, 0);

            collection.PatientAdmissionType = MultiSelectAdmissionTypeFlag.Day;
            Assert.AreEqual(collection.Count, 1);

            collection.PatientAdmissionType = MultiSelectAdmissionTypeFlag.Unknown;
            Assert.AreEqual(collection.Count, 0);

            collection.PatientAdmissionType = MultiSelectAdmissionTypeFlag.All;
            Assert.AreEqual(collection.Count, 1);
        }


        [TestMethod]
        public void FilterByOrderStatusTest()
        {
            FilterByOrderStatusTest(false);
        }

        [TestMethod]
        public void FilterByOrderStatusWithHiddenFlagTest()
        {
            FilterByOrderStatusTest(true);
        }

        [TestMethod]
        public void FilterByOrderStatusTest(bool isHidden)
        {
            // In-patients

            // Test for admitted in-patients

            var patient = OrderViewModelTest.CreateDefaultPatient();
            var admission = OrderViewModelTest.CreateDefaultAdmission(patient);
            admission.Type = AdmissionType.In;
            admission.Status = AdmissionStatus.Admitted;

            FilterByOrderStatus(patient, admission, MultiSelectAdmissionStatusFlag.Admitted, MultiSelectAdmissionTypeFlag.In, isHidden);

            // Test for registered in-patients

            patient = OrderViewModelTest.CreateDefaultPatient();
            admission = OrderViewModelTest.CreateDefaultAdmission(patient);
            admission.Type = AdmissionType.In;
            admission.Status = AdmissionStatus.Registered;

            FilterByOrderStatus(patient, admission, MultiSelectAdmissionStatusFlag.Registered, MultiSelectAdmissionTypeFlag.In, isHidden);

            // Test for discharged in-patients

            patient = OrderViewModelTest.CreateDefaultPatient();
            admission = OrderViewModelTest.CreateDefaultAdmission(patient);
            admission.Type = AdmissionType.In;
            admission.Status = AdmissionStatus.Discharged;

            FilterByOrderStatus(patient, admission, MultiSelectAdmissionStatusFlag.Discharged, MultiSelectAdmissionTypeFlag.In, isHidden);

            // Out-patients

            // Test for admitted out-patients

            patient = OrderViewModelTest.CreateDefaultPatient();
            admission = OrderViewModelTest.CreateDefaultAdmission(patient);
            admission.Type = AdmissionType.Out;
            admission.Status = AdmissionStatus.Admitted;

            FilterByOrderStatus(patient, admission, MultiSelectAdmissionStatusFlag.Admitted, MultiSelectAdmissionTypeFlag.Out, isHidden);

            // Test for registered out-patients

            patient = OrderViewModelTest.CreateDefaultPatient();
            admission = OrderViewModelTest.CreateDefaultAdmission(patient);
            admission.Type = AdmissionType.Out;
            admission.Status = AdmissionStatus.Registered;

            FilterByOrderStatus(patient, admission, MultiSelectAdmissionStatusFlag.Registered, MultiSelectAdmissionTypeFlag.Out, isHidden);

            // Test for discharged out-patients

            patient = OrderViewModelTest.CreateDefaultPatient();
            admission = OrderViewModelTest.CreateDefaultAdmission(patient);
            admission.Type = AdmissionType.Out;
            admission.Status = AdmissionStatus.Discharged;

            FilterByOrderStatus(patient, admission, MultiSelectAdmissionStatusFlag.Discharged, MultiSelectAdmissionTypeFlag.Out, isHidden);

            // Day-patients

            // Test for admitted day-patients

            patient = OrderViewModelTest.CreateDefaultPatient();
            admission = OrderViewModelTest.CreateDefaultAdmission(patient);
            admission.Type = AdmissionType.Day;
            admission.Status = AdmissionStatus.Admitted;

            FilterByOrderStatus(patient, admission, MultiSelectAdmissionStatusFlag.Admitted, MultiSelectAdmissionTypeFlag.Day, isHidden);

            // Test for registered day-patients

            patient = OrderViewModelTest.CreateDefaultPatient();
            admission = OrderViewModelTest.CreateDefaultAdmission(patient);
            admission.Type = AdmissionType.Day;
            admission.Status = AdmissionStatus.Registered;

            FilterByOrderStatus(patient, admission, MultiSelectAdmissionStatusFlag.Registered, MultiSelectAdmissionTypeFlag.Day, isHidden);

            // Test for discharged day-patients

            patient = OrderViewModelTest.CreateDefaultPatient();
            admission = OrderViewModelTest.CreateDefaultAdmission(patient);
            admission.Type = AdmissionType.Day;
            admission.Status = AdmissionStatus.Discharged;

            FilterByOrderStatus(patient, admission, MultiSelectAdmissionStatusFlag.Discharged, MultiSelectAdmissionTypeFlag.Day, isHidden);
        }

        public void FilterByOrderStatus(Patient patient, Admission admission, MultiSelectAdmissionStatusFlag admissionStatusFlag, MultiSelectAdmissionTypeFlag type, bool isHidden)
        {
            var order = OrderViewModelTest.CreateDefaultOrder(admission);
            var orders = new List<Order>(1) { order };

            var topPatient = new TopPatient(patient, admission, orders);

            var patientViewModel = new PatientViewModel(topPatient);

            var collection = new DepartmentOrderObservableCollection(TransformFunction);

            // Set Admission Status and Admission Type Filters

            collection.PatientAdmissionType = type;
            collection.AdmissionStatusFlagFilter = admissionStatusFlag;
            collection.ShowHiddenOverride = isHidden;

            // Test In Progress

            order = OrderViewModelTest.CreateDefaultOrder(admission);
            order.Status = OrderStatus.InProgress;
            order.IsHidden = isHidden;
            orders = new List<Order>(1) { order };

            collection.Synchronise(orders, patientViewModel);

            collection.OrderStatusFilter = OrderStatus.InProgress;
            Assert.AreEqual(collection.Count, 1);

            collection.OrderStatusFilter = OrderStatus.Completed;

            if (isHidden)
            {
                // Order status should be ignored

                Assert.AreEqual(collection.Count, 1);
            }
            else
            {
                Assert.AreEqual(collection.Count, 0);
            }

            collection.OrderStatusFilter = OrderStatus.Cancelled;

            if (isHidden)
            {
                // Order status should be ignored

                Assert.AreEqual(collection.Count, 1);
            }
            else
            {
                Assert.AreEqual(collection.Count, 0);
            }

            // Test Completed

            order = OrderViewModelTest.CreateDefaultOrder(admission);
            order.Status = OrderStatus.Completed;
            order.IsHidden = isHidden;
            orders = new List<Order>(1) { order };

            collection.Synchronise(orders, patientViewModel);

            collection.OrderStatusFilter = OrderStatus.InProgress;

            if (isHidden)
            {
                // Order status should be ignored

                Assert.AreEqual(collection.Count, 1);
            }
            else
            {
                Assert.AreEqual(collection.Count, 0);
            }

            collection.OrderStatusFilter = OrderStatus.Completed;
            Assert.AreEqual(collection.Count, 1);

            collection.OrderStatusFilter = OrderStatus.Cancelled;

            if (isHidden)
            {
                // Order status should be ignored

                Assert.AreEqual(collection.Count, 1);
            }
            else
            {
                Assert.AreEqual(collection.Count, 0);
            }

            // Test Cancelled

            order = OrderViewModelTest.CreateDefaultOrder(admission);
            order.Status = OrderStatus.Cancelled;
            order.IsHidden = isHidden;
            orders = new List<Order>(1) { order };

            collection.Synchronise(orders, patientViewModel);

            collection.OrderStatusFilter = OrderStatus.InProgress;

            if (isHidden)
            {
                // Order status should be ignored

                Assert.AreEqual(collection.Count, 1);
            }
            else
            {
                Assert.AreEqual(collection.Count, 0);
            }

            collection.OrderStatusFilter = OrderStatus.Completed;

            if (isHidden)
            {
                // Order status should be ignored

                Assert.AreEqual(collection.Count, 1);
            }
            else
            {
                Assert.AreEqual(collection.Count, 0);
            }

            collection.OrderStatusFilter = OrderStatus.Cancelled;
            Assert.AreEqual(collection.Count, 1);
        }

        #region Callbacks/Transforms

        private OrderViewModel TransformFunction(Order order, PatientViewModel patientViewModel)
        {
            return new OrderViewModel(order, patientViewModel, PopupOrderCallback, DismissPopupCallback,
                                      LocatePatientCallback, GetDefaultLocationCallback, ToggleNotesCallback,
                                      ToggleHistoryCallback, ToggleInfoCardCallback,
                                      null,
                                      TimelineItemType.NoteIn | TimelineItemType.NoteOut |
                                      TimelineItemType.ProcedureTimeUpdated | TimelineItemType.OrderAssigned |
                                      TimelineItemType.OrderCompleted | TimelineItemType.NotificationAcknowlegement);
        }

        private void ToggleInfoCardCallback()
        {
            throw new NotImplementedException();
        }

        private void ToggleHistoryCallback()
        {
            throw new NotImplementedException();
        }

        private void ToggleNotesCallback()
        {
            throw new NotImplementedException();
        }

        private LocationSummary GetDefaultLocationCallback()
        {
            return null;
        }

        private void LocatePatientCallback(PatientViewModel patientViewModel)
        {
            throw new NotImplementedException();
        }

        private void DismissPopupCallback()
        {
            throw new NotImplementedException();
        }

        private void PopupOrderCallback(OrderViewModel orderViewModel)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

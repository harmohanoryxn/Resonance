using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using Cloudmaster.WCS.DataServicesInvoker.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WCS.Shared.Department;
using WCS.Shared.Department.Schedule;
using WCS.Shared.Orders;
using WCS.Shared.Schedule;
using WCS.Shared.Ward.Schedule;

namespace WCS.Services.Test.ViewModel
{
    [TestClass]
    public class DepartmentPatientObservableCollectionTests
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

            var collection = new DepartmentPatientObservableCollection(TransformFunction)
            {
                PatientAdmissionType = MultiSelectAdmissionTypeFlag.In,
                OrderStatusFilter = OrderStatus.InProgress,
                AdmissionStatusFlagFilter = MultiSelectAdmissionStatusFlag.Admitted,
                                     Location = "US" };

            var topPatientList = new List<TopPatient>(1) { topPatient };

            collection.Synchronise(topPatientList);

            Assert.AreEqual(collection.Count, 1);

            collection.Location = "CT";

            Assert.AreEqual(collection.Count, 0);
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
            var topPatientList = new List<TopPatient>(1) { topPatient };

            var patientViewModel = new PatientViewModel(topPatient);

            var collection = new DepartmentPatientObservableCollection(TransformFunction);

            // Set Admission Status and Admission Type Filters

            collection.PatientAdmissionType = type;
            collection.AdmissionStatusFlagFilter = admissionStatusFlag;
            collection.ShowHiddenOverride = isHidden;

            // Test In Progress

            order = OrderViewModelTest.CreateDefaultOrder(admission);
            order.Status = OrderStatus.InProgress;
            order.IsHidden = isHidden;
            orders = new List<Order>(1) { order };
            topPatient = new TopPatient(patient, admission, orders);
            topPatientList = new List<TopPatient>(1) { topPatient };

            collection.Synchronise(topPatientList);

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
            topPatient = new TopPatient(patient, admission, orders);
            topPatientList = new List<TopPatient>(1) { topPatient };

            collection.Synchronise(topPatientList);

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
            topPatient = new TopPatient(patient, admission, orders);
            topPatientList = new List<TopPatient>(1) { topPatient };

            collection.Synchronise(topPatientList);

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

        
        
        
        #region Transform Functions

        private DepartmentPatientViewModel TransformFunction(TopPatient topPatient)
        {
            return new DepartmentPatientViewModel(topPatient, LocatePatientCallback, GetDefaultLocationCallback,
                                                  TransformFunction);
        }

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

        private void DismissPopupCallback()
        {
            throw new NotImplementedException();
        }

        private void PopupOrderCallback(OrderViewModel orderViewModel)
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
    }

#endregion
}

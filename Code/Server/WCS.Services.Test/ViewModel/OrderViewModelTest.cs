using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using Cloudmaster.WCS.DataServicesInvoker.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WCS.Shared.Orders;
using WCS.Shared.Schedule;
using WCS.Shared.Ward.Schedule;
using NotificationType = Cloudmaster.WCS.DataServicesInvoker.DataServices.NotificationType;

namespace WCS.Services.Test
{
    [TestClass]
    public class OrderViewModelTest
    {
        const int UpdateDurationInMilliseconds = 1000;

        private const MultiSelectAdmissionStatusFlag DefaultAdmissionStatus = MultiSelectAdmissionStatusFlag.Admitted;
        private const AdmissionType DefaultAdmissionType = AdmissionType.In;

        private static readonly DateTime _defaultAdmitDateTime = DateTime.Today.AddHours(12);
        private static readonly DateTime _defaultProcedureTime = DateTime.Today.AddHours(12);

        private const string DefaultDepartmentName = "Ultrasound";
        private const string DefaultDepartmentCode = "US";

        [TestMethod]
        public void SyncronizeTest()
        {
            OrderViewModel orderViewModel = CreateDefaultOrderViewModel();

            Assert.AreEqual(orderViewModel.AdmissionStatusFlag, DefaultAdmissionStatus);
            Assert.AreEqual(orderViewModel.AdmissionType, DefaultAdmissionType);
            Assert.AreEqual(orderViewModel.AdmitDateTime, _defaultAdmitDateTime);
            Assert.AreEqual(orderViewModel.OrderCoordinator.Order.StartTime, _defaultProcedureTime.TimeOfDay);
        }

        [TestMethod]
        public void SyncronizeProcedureTimeTest()
        {
            OrderViewModel orderViewModel = CreateDefaultOrderViewModel();

            var defaultOrderWithUpdateProcedureTime = DefaultOrderWithAdmission();

            defaultOrderWithUpdateProcedureTime.ProcedureTime = DateTime.Today.AddHours(14);

            orderViewModel.Synchronise(defaultOrderWithUpdateProcedureTime);

            Assert.AreNotEqual(orderViewModel.OrderCoordinator.Order.StartTime, _defaultProcedureTime.TimeOfDay);
            Assert.AreEqual(orderViewModel.OrderCoordinator.Order.StartTime, defaultOrderWithUpdateProcedureTime.ProcedureTime.Value.TimeOfDay);
        }

        [TestMethod]
        public void UpdateProcedureTimeTest()
        {
            OrderViewModel orderViewModel = CreateDefaultOrderViewModel();

            // Create order with updated procedureTime

            var defaultOrderWithUpdateProcedureTime = DefaultOrderWithAdmission();

            var newProcedureTime = DateTime.Today.AddHours(14);

            defaultOrderWithUpdateProcedureTime.ProcedureTime = newProcedureTime;

            // Update ProcedureTime

            orderViewModel.UpdateProcedureTimeTest(defaultOrderWithUpdateProcedureTime, UpdateDurationInMilliseconds);

            Thread.Sleep(UpdateDurationInMilliseconds * 2);

            // Synconization with update should have been called and new procedure time in place

            Assert.AreEqual(orderViewModel.OrderCoordinator.Order.StartTime, newProcedureTime.TimeOfDay);
        }

        /// <summary>
        /// Test to solve problem where procedure time updates and notes were not appearing as the viewModel was locked after a syncronization
        /// </summary>
        [TestMethod]
        public void SyncronizeAndThenUpdateProcedureTimeTest()
        {
            OrderViewModel orderViewModel = CreateDefaultOrderViewModel();

            var defaultOrder = DefaultOrderWithAdmission();

            orderViewModel.Synchronise(defaultOrder);

            // Ensure order view model still has default procedure time as expected

            Assert.AreEqual(orderViewModel.OrderCoordinator.Order.StartTime, defaultOrder.ProcedureTime.Value.TimeOfDay);

            // Create order with updated procedureTime

            var defaultOrderWithUpdateProcedureTime = DefaultOrderWithAdmission();

            var newProcedureTime = DateTime.Today.AddHours(14);

            defaultOrderWithUpdateProcedureTime.ProcedureTime = newProcedureTime;

            // Update ProcedureTime

            orderViewModel.UpdateProcedureTimeTest(defaultOrderWithUpdateProcedureTime, UpdateDurationInMilliseconds);

            Thread.Sleep(UpdateDurationInMilliseconds * 2);

            // Synconization with update should have been called and new procedure time in place

            Assert.AreNotEqual(orderViewModel.OrderCoordinator.Order.StartTime, defaultOrder.ProcedureTime.Value.TimeOfDay);
            Assert.AreEqual(orderViewModel.OrderCoordinator.Order.StartTime, newProcedureTime.TimeOfDay);
        }

        [TestMethod]
        public void UpdateProcedureTimeWithSyncronizationDuringUpdateTest()
        {
            OrderViewModel orderViewModel = CreateDefaultOrderViewModel();

            // Create order with updated procedureTime

            var defaultOrderWithUpdateProcedureTime = DefaultOrderWithAdmission();

            var newProcedureTime = DateTime.Today.AddHours(14);

            defaultOrderWithUpdateProcedureTime.ProcedureTime = newProcedureTime;

            // Create order with incorrect procedureTime

            var orderWithIncorrectProcedureTime = DefaultOrderWithAdmission();

            var incorrectProcedureTime = DateTime.Today.AddHours(16);

            orderWithIncorrectProcedureTime.ProcedureTime = incorrectProcedureTime;

            // Update ProcedureTime

            orderViewModel.UpdateProcedureTimeTest(defaultOrderWithUpdateProcedureTime, UpdateDurationInMilliseconds);

            // Sleep for a split second as flag to mark as updating happens within a task

            Thread.Sleep(UpdateDurationInMilliseconds / 5);

            // Attempt Syncronization with incorrect time

            orderViewModel.Synchronise(orderWithIncorrectProcedureTime);

            // Should not have syncronized as update is in progress

            Assert.AreNotEqual(orderViewModel.OrderCoordinator.Order.StartTime, incorrectProcedureTime.TimeOfDay);

            // Sleep so update has time to complete

            Thread.Sleep(UpdateDurationInMilliseconds * 2);

            // Synconization with update should have been called and new procedure time in place

            Assert.AreEqual(orderViewModel.OrderCoordinator.Order.StartTime, newProcedureTime.TimeOfDay);
        }

       
        #region Create Default Methods

        public Order DefaultOrderWithAdmission()
        {
            var patient = CreateDefaultPatient();

            var admission = CreateDefaultAdmission(patient);

            var defaultOrderWithUpdateProcedureTime = CreateDefaultOrder(admission);
            return defaultOrderWithUpdateProcedureTime;
        }

        public static OrderViewModel CreateDefaultOrderViewModel()
        {
            var patient = CreateDefaultPatient();

            var admission = CreateDefaultAdmission(patient);

            var order = CreateDefaultOrder(admission);

            var orders = new List<Order>() {order};

            var topPatient = new TopPatient(patient, admission, orders);

            var patientViewModel = new PatientViewModel(topPatient);

            var orderViewModel = new OrderViewModel(order, patientViewModel, PopupOrderCallback, DismissPopupCallback,
                                                LocatePatientCallback, GetDefaultLocationCallback,
                                                ToggleNotesCallback, ToggleHistoryCallback, ToggleInfoCardCallback,
                                                NotificationType.Prep, TimelineItemType.Order);
            return orderViewModel;
        }

        public static Patient CreateDefaultPatient()
        {
            var patient = new Patient()
                              {
                                  AssistanceDescription = "Mobility",
                                  DateOfBirth = new DateTime(1960, 10, 10),
                                  FamilyName = "FamilyName",
                                  GivenName = "GivenName",
                                  IPeopleNumber = "123",
                                  IsAssistanceRequired = false,
                                  Sex = PatientSex.Male,
                                  PatientId = 1
                              };
            return patient;
        }

        public static Admission CreateDefaultAdmission(Patient patient)
        {
            var admission = new Admission()
                                {
                                    Patient = patient,
                                    AdmissionId = 1,
                                    AdmissionStatusFlag = DefaultAdmissionStatus,
                                    AdmitDateTime = _defaultAdmitDateTime,
                                    AdmittingDoctor = string.Empty,
                                    AttendingDoctor = string.Empty,
                                    Type = DefaultAdmissionType
                                };

            admission.Location = new Location()
                                     {
                                         Bed = "1",
                                         FullName = "Location",
                                         Name = "Name",
                                         IsEmergency = false,
                                         Room = "101"
                                     };

            admission.CriticalCareIndicators = new CriticalCareIndicators()
                                                   {
                                                       HasLatexAllergy = false,
                                                       IsMrsaRisk = false,
                                                       IsRadiationRisk = false
                                                   };

            return admission;
        }

        public static Order CreateDefaultOrder(Admission admission)
        {
            var orderId = 43;

            var order = new Order()
                            {
                                OrderId = orderId,
                                Acknowledged = false,
                                Admission = admission,
                                ClinicalIndicators = string.Empty,
                                CompletedTime = DateTime.Today.AddHours(13),
                                DepartmentCode =DefaultDepartmentCode,
                                Duration = TimeSpan.FromMinutes(60),
                                DepartmentName = DefaultDepartmentName,
                                IsHidden = false,
                                ProcedureCode = string.Empty,
                                ProcedureTime = _defaultProcedureTime,
                                ProcedureDescription = string.Empty,
                                EstimatedProcedureDuration = 60,
                                OrderNumber = "ORD045",
                                Status = OrderStatus.InProgress,
                                OrderingDoctor = string.Empty
                            };

            order.Notes = new Note[0];
            order.Notifications = new Notification[0];
            order.Updates = new Update[0];

            return order;
        }

        #endregion

        #region Empty Callbacks

        private static Cloudmaster.WCS.DataServicesInvoker.DataServices.LocationSummary GetDefaultLocationCallback()
        {
            return null;
        }

        private static void ToggleInfoCardCallback()
        {
            return;
        }

        private static void ToggleHistoryCallback()
        {
            return;
        }

        private static void ToggleNotesCallback()
        {
            return;
        }

        private static void LocatePatientCallback(PatientViewModel patientViewModel)
        {
            return;
        }

        private static void DismissPopupCallback()
        {
            return;
        }

        private static void PopupOrderCallback(OrderViewModel orderViewModel)
        {
            return;
        }

        #endregion
    }
}

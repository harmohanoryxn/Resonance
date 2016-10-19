using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using Cloudmaster.WCS.DataServicesInvoker.Types;
using WCS.Core.Composition;
using WCS.Shared.Ward.Schedule;
using WCS.Shared.Schedule;

namespace WCS.Services.Test
{
    [TestClass]
    public class TimelineCoordinatorTest
    {
        [TestMethod]
        public void NoteOutTest()
        {
            var testDevice = new DefaultDeviceIdentity().DeviceName;

            var patient = OrderViewModelTest.CreateDefaultPatient();
            var admission = OrderViewModelTest.CreateDefaultAdmission(patient);

            var order = OrderViewModelTest.CreateDefaultOrder(admission);

            order.CompletedTime = null;

            order.Notes = new Note[1]
                              {
                                  new Note()
                                      {
                                          NoteId = 1,
                                          NoteMessage = "Message",
                                          OrderId = order.Id,
                                          DateCreated = DateTime.Now,
                                          Source = testDevice
                                      }
                              };

            TimelineCoordinator timelineCoordinator = new TimelineCoordinator(TimelineItemType.NoteOut);
            timelineCoordinator.Synchronise(order, null);
            
            Assert.AreEqual(timelineCoordinator.TimelineItems.Count(), 1);

            timelineCoordinator = new TimelineCoordinator(TimelineItemType.OrderAssigned | TimelineItemType.PatientArrived | TimelineItemType.OrderCompleted);
            timelineCoordinator.Synchronise(order, null);

            Assert.AreEqual(timelineCoordinator.TimelineItems.Count(), 0);

            timelineCoordinator = new TimelineCoordinator(TimelineItemType.NoteIn | TimelineItemType.NoteOut | TimelineItemType.OrderAssigned);
            timelineCoordinator.Synchronise(order, null);

            Assert.AreEqual(timelineCoordinator.TimelineItems.Count(), 1);
        }

        [TestMethod]
        public void OrderCompletedTest()
        {
            var testDevice = new DefaultDeviceIdentity().DeviceName;

            var patient = OrderViewModelTest.CreateDefaultPatient();
            var admission = OrderViewModelTest.CreateDefaultAdmission(patient);

            var order = OrderViewModelTest.CreateDefaultOrder(admission);

            order.CompletedTime = null;

            var timelineCoordinator = new TimelineCoordinator(TimelineItemType.OrderAssigned | TimelineItemType.PatientArrived | TimelineItemType.OrderCompleted);
            timelineCoordinator.Synchronise(order, null);

            Assert.AreEqual(timelineCoordinator.TimelineItems.Count(), 0);

            // Set order completed time

            order.CompletedTime = DateTime.Now;

            timelineCoordinator = new TimelineCoordinator(TimelineItemType.OrderAssigned | TimelineItemType.PatientArrived | TimelineItemType.OrderCompleted);
            timelineCoordinator.Synchronise(order, null);

            Assert.AreEqual(timelineCoordinator.TimelineItems.Count(), 1);
        }
    }
}

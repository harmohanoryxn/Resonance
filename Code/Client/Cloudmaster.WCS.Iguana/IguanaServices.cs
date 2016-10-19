using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cloudmaster.WCS.IO;
using Cloudmaster.WCS.Iguana;
using Cloudmaster.WCS.Iguana.Services;
using Cloudmaster.WCS.Classes;
using Cloudmaster.WCS.Classes.Helpers;
using System.Security.Cryptography;
using System.IO;

namespace Cloudmaster.WCS.Iguana
{
	/// <summary>
	/// Connect to the Iguana Service and provides Order Management ability to the data read from the service
	/// </summary>
	/// <remarks>>
	/// Should this not inherit from ExternalServicesBase?
	/// </remarks>
	public class IguanaServices : IAdmisssionManager
    {
        private IguanaDatabaseConnection services;

        public IguanaServices(string connectionString)
        {
            Dictionary<string, string> connectionStringValues = ExternalServicesBase.ParseConnectionString(connectionString);

            services = new IguanaDatabaseConnection(new Uri(connectionStringValues["Uri"]));

            services.SendingRequest += new EventHandler<System.Data.Services.Client.SendingRequestEventArgs>(services_SendingRequest);
        }

        void services_SendingRequest(object sender, System.Data.Services.Client.SendingRequestEventArgs e)
        {
            ((System.Net.HttpWebRequest)e.Request).AutomaticDecompression = (System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate);

            e.RequestHeaders.Add("IsAnonymous: false");
        }

        #region Lab Results 
        /*
        public IList<Classes.LabResult> GetLabResultsForToday()
        {
            IList<Classes.LabResult> results = new List<Classes.LabResult>();

            var todaysLabResults = services.LabResults.ToList ();

            foreach (var labResult in todaysLabResults)
            {
                Classes.LabResult result = new Classes.LabResult(labResult.id);

                result.PatientId = labResult.patientId;
                result.AnalysisDateTime = labResult.analysisDateTime;
                result.ObservationDateTime = labResult.observationDateTime;

                results.Add(result);
            }

            return results;
        }*/

        #endregion

        #region Orders
        /*
        public Guid CreateEmergencyRoomOrder(Guid admissionId, string patientId, string familyName, string givenName, string serviceId)
        {
            // Create Order

            DateTime now = DateTime.Now;

            DateTime sevenAmToday = new DateTime(now.Year, now.Month, now.Day, 7, 0, 0);

            var order = new Services.OrderMessage();

            order.id = Guid.NewGuid();

            order.placerOrderNumberEI = order.id.ToString();
            order.requestedDateTime = sevenAmToday;
            order.location = "ER";
            order.orderControl = "ER";
            order.patientId = patientId;
            order.familyName = familyName;
            order.givenName = givenName;
            order.serviceId = serviceId;
            order.priority = "1";

            services.AddObject("OrderMessages", order);
            services.SaveChanges();

            // Update Admission

            var admissionsMetadatas = services.AdmissionsMetadatas.ToList();

            if (admissionsMetadatas.Count() > 0)
            {
                var admissionsMetadata = admissionsMetadatas.Single(a => a.admissionId == admissionId);

                if (admissionsMetadata != null)
                {
                    if (serviceId == OrderServiceType.CT) admissionsMetadata.requiresCTId = order.id;
                    else if (serviceId == OrderServiceType.MRI) admissionsMetadata.requiresMRIId = order.id;
                    else if (serviceId == OrderServiceType.Ultrasound) admissionsMetadata.requiresUltrasoundId = order.id;
                    else if (serviceId == OrderServiceType.XRay) admissionsMetadata.requiresXRayId = order.id;
                    else admissionsMetadata.requiresEchoId = order.id;

                    services.UpdateObject(admissionsMetadata);
                    services.SaveChanges();
                }
            }

            return order.id;
        }

        public void CancelEmergencyRoomOrder(Guid id)
        {
            var orders = services.OrderMessages.Where(o => o.id == id).ToList();

            if (orders.Count() > 0)
            {
                var order = orders.First();

                order.orderControl = "CA";

                services.UpdateObject(order);
                services.SaveChanges();
            }
        }
        */

        public IList<Order> GetOrdersForToday()
        {
            Dictionary<string, Order> orders = new Dictionary<string, Order>();

            IList<Order> result = new List<Order>();

            DateTime startDate = FrequencyHelper.GetStartDate("Daily");
            DateTime endDate = FrequencyHelper.GetEndDate("Daily");

            var admissionsToday = services.Admissions.Where(a => ((a.location == "ACC2") || (a.location == "ACC1") || (a.location == "ACC3") || (a.location == "ACCG") || (a.location == "ACCLG") || (a.location == "DAY WARD") || (a.location == "TESTBED"))).ToList();

            var allOrders = services.OrderMessages.Where(o => (o.requestedDateTime >= startDate)
                && ((o.location == "ACC2") || (o.location == "ACC1") || (o.location == "U") || (o.location == "ACC3") || (o.location == "ACCG") || (o.location == "ACCLG") || (o.location == "DAY WARD") || (o.location == "TESTBED"))
                && (o.serviceId != "RAD CATH") 
                && (o.serviceId != "FLUORO")
                && (o.serviceId != "CAR")
                && (o.serviceId != "RAD CARD")).ToList();

            var todaysOrders = allOrders.OrderBy(o => o.dateCreated);

            FlattenOrders(orders, admissionsToday, allOrders, todaysOrders);

            // Order could have been moved forward so remove any updated order no long relevant to today

            result = orders.Values.Where(o => (o.RequestedDateTime < endDate)).OrderBy(o => o.RequestedDateTime).ToList();

            return result;
        }

        public IList<Order> GetOrdersForWardToday(string wardIdentifier)
        {
            Dictionary<string, Order> orders = new Dictionary<string, Order>();

            IList<Order> result = new List<Order>();

            DateTime startDate = FrequencyHelper.GetStartDate ("Daily");
            DateTime endDate = FrequencyHelper.GetEndDate ("Daily");

            var admissionsToday = services.Admissions.Where(a => a.location == wardIdentifier).ToList();

            var allOrders = services.OrderMessages.Where(o => (o.requestedDateTime >= startDate) 
                && (o.location == wardIdentifier || (o.location == "U"))
                && (o.serviceId != "RAD CATH") 
                && (o.serviceId != "FLUORO")
                && (o.serviceId != "CAR")
                && (o.serviceId != "RAD CARD")).ToList();

            var todaysOrders = allOrders.OrderBy(o => o.dateCreated);

            FlattenOrders(orders, admissionsToday, allOrders, todaysOrders);

            // Order could have been moved forward so remove any updated order no long relevant to today

            result = orders.Values.Where(o => (o.RequestedDateTime < endDate)).OrderBy(o => o.RequestedDateTime).ToList();

            return result;
        }

        private static void FlattenOrders(Dictionary<string, Order> orders, List<Cloudmaster.WCS.Iguana.Services.Admission> admissionsToday, List<OrderMessage> allOrders, IOrderedEnumerable<OrderMessage> todaysOrders)
        {
            foreach (var orderForToday in todaysOrders)
            {
                bool isLocationKnown = DetermineIfLocationKnown(admissionsToday, orderForToday);

                if (isLocationKnown)
                {
                    bool isNewOrder = (orderForToday.orderControl == "NW");

                    var orderToUse = orderForToday;

                    // Check for cancellation

                    var allCancelations = allOrders.Where(o => o.placerOrderNumberEI == orderForToday.placerOrderNumberEI && o.orderControl == "CA");

                    bool isCancelled = (allCancelations.Count() > 0);

                    if (!isCancelled)
                    {
                        Order order = new Order(orderToUse.id);

                        MapOrderEntityToClass(orderToUse, order);

                        if (order.PlaceOrderId.CompareTo(string.Empty) != 0)
                        {
                            if (orders.ContainsKey(order.PlaceOrderId))
                            {
                                // Updated order so overwrite

                                orders[order.PlaceOrderId] = order;
                            }
                            else
                            {
                                orders.Add(order.PlaceOrderId, order);
                            }
                        }
                    }
                }
            }
        }

        private static bool DetermineIfLocationKnown(List<Cloudmaster.WCS.Iguana.Services.Admission> admissionsToday, OrderMessage orderForToday)
        {
            bool isLocationKnown = true;

            if (orderForToday.location == "U")
            {
                var admissions = admissionsToday.Where(a => a.patientId == orderForToday.patientId).OrderBy(a => a.dateCreated);

                if (admissions.Count() > 0)
                {
                    var admission = admissions.Last();

                    orderForToday.location = admission.location;
                    orderForToday.room = admission.room;
                    orderForToday.bed = admission.bed;
                }
                else
                {
                    isLocationKnown = false;
                }
            }
            return isLocationKnown;
        }

        private static void MapOrderEntityToClass(OrderMessage orderToUse, Order order)
        {
            order.PlaceOrderId = string.Format("{0}-{1}", orderToUse.placerOrderNumberEI, orderToUse.serviceText);
            order.PatientId = orderToUse.patientId;

            try
            {
                order.FamilyName = Decrypt(orderToUse.familyName);
            }
            catch (Exception ex)
            {
                order.FamilyName = ex.Message;
            }

            try
            {
                order.GivenName = Decrypt(orderToUse.givenName);
            }
            catch (Exception ex)
            {
                order.GivenName = ex.Message;
            }

            order.Location = orderToUse.location;
            order.Room = orderToUse.room;
            order.Bed = orderToUse.bed;
            order.Service = orderToUse.serviceId;
            order.ServiceText = orderToUse.serviceText;
            order.ObservationDateTime = orderToUse.observationDateTime;
            order.ObservationEndDateTime = orderToUse.observationEndDateTime;
            order.RequestedDateTime = orderToUse.requestedDateTime;
            order.DateOfBirth = orderToUse.dateOfBirth;
        }

        public static string Decrypt(string text)
        {
            StringBuilder builder = new StringBuilder();

            int index = 0;

            foreach (var c in text)
            {
                int numericValue = Convert.ToInt16(c);

                char value = Convert.ToChar(numericValue - 12 - index);

                index += 1;

                builder.Append(value);
            }


            return builder.ToString();
        }

        #endregion

        #region Order Metadata

        public IList<Classes.OrderMetadata> GetOrdersMetadataForWardToday(Floor floor)
        {
            IList<Classes.OrderMetadata> orderMetadatas = new List<Classes.OrderMetadata>();

            var results = services.OrderMetadatas; //.Where(r => r.parentId == floor.Id);

            foreach (var entity in results)
            {
                Classes.OrderMetadata orderMetadata = MapOrderMetadataToClass(entity);

                orderMetadatas.Add(orderMetadata);
            }

            return orderMetadatas;
        }

        private static Classes.OrderMetadata MapOrderMetadataToClass(Cloudmaster.WCS.Iguana.Services.OrderMetadata metadata)
        {
            Classes.OrderMetadata orderMetadata = new Classes.OrderMetadata();

            orderMetadata.OrderNumber = metadata.orderNumber;
            orderMetadata.IsExamAcknowledged = metadata.isExamAcknowledged;
            orderMetadata.IsInjectionAcknowledged = metadata.isInjectionAcknowledged;
            orderMetadata.IsFastingAcknowledged = metadata.isFastingAcknowledged;
            orderMetadata.IsPrepWorkAcknowledged = metadata.isPrepWorkAcknowledged;
            orderMetadata.RequestedDateTimeOverride = metadata.requestedDateTimeOverride;
            orderMetadata.LastRequestedDateTimeOverrideModified = metadata.lastRequestedDateTimeOverrideModified;
            orderMetadata.Notes = metadata.notes;

            return orderMetadata;
        }

        public void SendOrderAcknowledgements(string orderNumber, bool isFastingAcknowledged, bool isPrepWorkAcknowledged, bool isExamAcknowledged, bool isInjectionAcknowledged)
        {
            var results = services.OrderMetadatas.Where(om => om.orderNumber == orderNumber);

            Cloudmaster.WCS.Iguana.Services.OrderMetadata entity;

            if (results.Count() > 0)
            {
                entity = results.First();

                entity.orderNumber = orderNumber;
                entity.isFastingAcknowledged = isFastingAcknowledged;
                entity.isPrepWorkAcknowledged = isPrepWorkAcknowledged;
                entity.isExamAcknowledged = isExamAcknowledged;
                entity.isInjectionAcknowledged = isInjectionAcknowledged;

                services.UpdateObject(entity);
                services.SaveChanges();
            }
            else
            {
                entity = new Cloudmaster.WCS.Iguana.Services.OrderMetadata();

                entity.id = Guid.NewGuid();

                entity.orderNumber = orderNumber;
                entity.isFastingAcknowledged = isFastingAcknowledged;
                entity.isPrepWorkAcknowledged = isPrepWorkAcknowledged;
                entity.isExamAcknowledged = isExamAcknowledged;
                entity.isInjectionAcknowledged = isInjectionAcknowledged;

                services.AddObject("OrderMetadatas", entity);
                services.SaveChanges();
            }
        }

        public void UpdateRequestedDateTimeOverride(string orderNumber, DateTime? requestedDateTimeOverride, string notes)
        {
            var results = services.OrderMetadatas.Where(om => om.orderNumber == orderNumber);

            Cloudmaster.WCS.Iguana.Services.OrderMetadata entity;

            if (results.Count() > 0)
            {
                entity = results.First();

                entity.orderNumber = orderNumber;
                entity.requestedDateTimeOverride = requestedDateTimeOverride;
                entity.notes = notes;
                entity.lastRequestedDateTimeOverrideModified = DateTime.Now;

                services.UpdateObject(entity);
                services.SaveChanges();
            }
            else
            {
                entity = new Cloudmaster.WCS.Iguana.Services.OrderMetadata();

                entity.id = Guid.NewGuid();

                entity.orderNumber = orderNumber;
                entity.requestedDateTimeOverride = requestedDateTimeOverride;
                entity.notes = notes;
                entity.lastRequestedDateTimeOverrideModified = DateTime.Now;

                services.AddObject("OrderMetadatas", entity);
                services.SaveChanges();
            }
        }

        #endregion

        #region Order Requests
        /*
        public IList<Classes.OrderRequest> GetOrderRequestsForToday()
        {
            IList<Classes.OrderRequest> orderRequests = new List<Classes.OrderRequest>();

            var results = services.OrderRequests; //.Where(r => r.parentId == floor.Id);

            foreach (var entity in results)
            {
                Classes.OrderRequest orderMetadata = MapOrderReuqestToClass(entity);

                orderRequests.Add(orderMetadata);
            }

            return orderRequests;
        }

        private static Classes.OrderRequest MapOrderReuqestToClass(Services.OrderRequest orderToUse)
        {
            Classes.OrderRequest orderRequest = new Classes.OrderRequest();

            orderRequest.PlaceOrderId = orderToUse.placeOrderId;
            orderRequest.FamilyName = orderToUse.familyName;
            orderRequest.GivenName = orderToUse.givenName;
            orderRequest.Location = orderToUse.location;
            orderRequest.Room = orderToUse.room;
            orderRequest.Bed = orderToUse.bed;
            orderRequest.Service = orderToUse.serviceId;
            orderRequest.ServiceText = orderToUse.serviceText;
            orderRequest.ObservationDateTime = orderToUse.observationDateTime;
            orderRequest.RequestedDateTime = orderToUse.requestedDateTime;

            return orderRequest;
        }
        */
        #endregion
        
        #region Admissions
        /*
        public IList<Classes.Admission> GetAdmisssionForWardToday(Floor floor)
        {
            var admissionEvents = services.Admissions.Where(a => (a.location == floor.Name)).ToList();

            var dischages = admissionEvents.Where(a => a.type == "A03").ToList();

            var admissions = admissionEvents.Where(a => a.type != "A03").OrderBy(a => a.dateCreated).ToList();

            Dictionary<string, Classes.Admission> results = new Dictionary<string, Classes.Admission>();

            foreach (var admission in admissions)
            {
                if (dischages.Count(d => d.patientId == admission.patientId) == 0)
                {
                    Classes.Admission result = new Classes.Admission(admission.id);

                    result.PatientId = admission.patientId;
                    result.VisitId = admission.visitId;
                
                    result.AdmissionDateTime = admission.admitDateTime;
                    result.ExpectedDischargeDateTime = admission.dischargeDateTime;

                    result.GivenName = admission.givenName;
                    result.FamilyName = admission.familyName;
                    result.Location = admission.location;
                    result.Room = admission.room;
                    result.Bed = admission.bed;

                    result.DateOfBirth = admission.dateOfBirth;

                    if (results.ContainsKey(admission.patientId))
                    {
                        results[admission.patientId] = result;
                    }
                    else
                    {
                        results.Add(admission.patientId, result);
                    }
                }
            }

            return results.Values.ToList();
        }

        
        public IList<Classes.Admission> GetDischargesForWardToday(Floor floor)
        {
            List<Classes.Admission> results = new List<Classes.Admission>();

            var admissionEvents = services.Admissions.Where(a => (a.location == floor.Name)).ToList();

            DateTime startDate = FrequencyHelper.GetStartDate("Daily");
            DateTime endDate = FrequencyHelper.GetEndDate("Daily");

            var dischages = admissionEvents.Where(a => (a.type == "A02") && (a.dischargeDateTime < endDate && a.dischargeDateTime > startDate)).ToList();

            foreach (var discharge in dischages)
            {
                Classes.Admission result = new Classes.Admission(discharge.id);

                result.PatientId = discharge.patientId;
                result.VisitId = discharge.visitId;

                result.DischargeDateTime = discharge.dischargeDateTime;

                result.Location = discharge.location;
                result.Room = discharge.room;
                result.Bed = discharge.bed;

                results.Add(result);
            }

            results = results.OrderBy(a => a.DischargeDateTime).ToList();

            return results;
        }


        public IList<Classes.AdmissionMetadata> GetAdmisssionMetadataForWardToday(Floor floor)
        {
            IList<Classes.AdmissionMetadata> results = new List<Classes.AdmissionMetadata>();

            DateTime startDate = FrequencyHelper.GetStartDate("Daily");
            DateTime endDate = FrequencyHelper.GetEndDate("Daily");

            var todaysAdmissions = services.AdmissionsMetadatas.ToList();

            foreach (var admission in todaysAdmissions)
            {
                Classes.AdmissionMetadata result = new Classes.AdmissionMetadata();

                result.Id = admission.id;

                if (admission.admissionId.HasValue)
                {
                    result.AdmissionId = admission.admissionId.Value;
                }

                result.RequiresCT = admission.requiresCT;
                result.RequiresEcho = admission.requiresEcho;
                result.RequiresMRI = admission.requiresMRI;
                result.RequiresUltasound = admission.requiresUltrasound;
                result.RequiresXRay = admission.requiresXRay;

                result.RequiresCTId = admission.requiresCTId;
                result.RequiresEchoId = admission.requiresEchoId;
                result.RequiresMRIId = admission.requiresMRIId;
                result.RequiresUltasoundId = admission.requiresUltrasoundId;
                result.RequiresXRayId = admission.requiresXRayId;
                
                results.Add(result);
            }

            return results;
        }

        public void UpdateAdmissionMetadata(Guid admissionId, bool? requiresCT, bool? requiresEcho, bool? requiresMRI, bool? requiresUltasound, bool? requiresXRay, Guid? requiresCTId, Guid? requiresEchoId, Guid? requiresMRIId, Guid? requiresUltasoundId, Guid? requiresXRayId)
        {
            var results = services.AdmissionsMetadatas.Where(a => a.admissionId == admissionId);

            Services.AdmissionsMetadata entity;

            if (results.Count() > 0)
            {
                entity = results.First();

                entity.requiresCT = requiresCT;
                entity.requiresEcho = requiresEcho;
                entity.requiresMRI = requiresMRI;
                entity.requiresUltrasound = requiresUltasound;
                entity.requiresXRay = requiresXRay;

                entity.requiresCTId = requiresCTId;
                entity.requiresEchoId = requiresEchoId;
                entity.requiresMRIId = requiresMRIId;
                entity.requiresUltrasoundId = requiresUltasoundId;
                entity.requiresXRayId = requiresXRayId;

                services.UpdateObject(entity);
                services.SaveChanges();
            }
            else
            {
                entity = new Services.AdmissionsMetadata();

                entity.id = Guid.NewGuid();

                entity.admissionId = admissionId;
                entity.requiresCT = requiresCT;
                entity.requiresEcho = requiresEcho;
                entity.requiresMRI = requiresMRI;
                entity.requiresUltrasound = requiresUltasound;
                entity.requiresXRay = requiresXRay;

                entity.requiresCTId = requiresCTId;
                entity.requiresEchoId = requiresEchoId;
                entity.requiresMRIId = requiresMRIId;
                entity.requiresUltrasoundId = requiresUltasoundId;
                entity.requiresXRayId = requiresXRayId;

                services.AddObject("AdmissionsMetadatas", entity);
                services.SaveChanges();
            }
        }
        */
        #endregion
    }
}

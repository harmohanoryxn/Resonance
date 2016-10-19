using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Objects;
using System.Linq;
using System.Text;
using System.Threading;
using WCS.Core;
using WCS.Data.EF;

namespace WCS.Services.DataServices.Data
{
	/// <summary>
	/// Provides an API to the EF 
	/// </summary>
	public partial class ServerFacade
	{
		#region Precompiled Queries

		private static Func<WCSEntities, int, WCS.Data.EF.Order> OrdersById =
			CompiledQuery.Compile((WCSEntities db, int orderid) =>
								  (from order in db.Order
									.Include("Admission")
									.Include("Procedure")
									.Include("Notifications")
									.Include("Notes")
								   where order.orderId == orderid
								   select order).FirstOrDefault());

		private static Func<WCSEntities, int, WCS.Data.EF.Bed> BedById =
			CompiledQuery.Compile((WCSEntities db, int bedId) =>
								  (from bed in db.Bed.Include("Room").Include("Admission").Include("BedCleaningEvent")
									.Include("Notes")
								   where bed.bedId == bedId
								   select bed).FirstOrDefault());

		#endregion

		#region Security

		public AuthenticationToken Authenticate(string deviceName, string pin, DateTime timestamp)
		{
			return new DatabaseInvoker<AuthenticationToken>().Execute((efContext) =>
																		{
																			var device = GetDeviceByName(efContext, deviceName);
																			var hasPin =
																				efContext.Pin.Any(
																					p =>
																					string.Compare(p.pin, pin) == 0 &&
																					p.Device_deviceId == device.deviceId);

																			return new AuthenticationToken()
																					{
																						IsAuthenticated = hasPin,
																						LockScreenTimeout = device.lockTimeout,
																						Message =
																							hasPin
																								? string.Format("Logged in at {0}",
                                                                                                                timestamp.ToWcsTimeFormat())
																								: string.Format("Login failed at {0}",
                                                                                                                timestamp.ToWcsTimeFormat())
																					};
																		});
		}

        private WCS.Data.EF.Device GetDeviceByName(WCSEntities efContext, string deviceName)
        {
            return efContext.Device.FirstOrDefault(d => string.Compare(d.name, deviceName) == 0);
        }

        #endregion

		#region Configurations

		public DeviceConfiguration GetConfigurations(string deviceName)
		{
			DeviceConfiguration result = new DeviceConfiguration();

			var transactionOptions = new System.Transactions.TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted };
			using (var transactionScope = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
			{
				using (var efContext = new WCSEntities())
				{
					var deviceQueryResult = (from dc in efContext.DeviceConfiguration.Include("Configuration")
											 where dc.Device.name == deviceName
											 select dc).ToList();

					result.DeviceName = deviceName;
					result.Instances = (from dc in deviceQueryResult
										select new DeviceConfigurationInstance()
											{
												LocationCode = dc.Device.Location != null ? dc.Device.Location.code : String.Empty,
                                                WaitingRoomLocationCode = ((dc.Device.Location != null) && (dc.Device.Location.WaitingArea != null)) ? dc.Device.Location.WaitingArea.code : string.Empty,
												LocationName = dc.Device.Location != null ? dc.Device.Location.name : String.Empty,
												RequiresLoggingOn = dc.Device.lockTimeout > 0,
												ShortcutKey = dc.shortcutKeyNo,
												PollingTimeouts = new PollingTimeouts()
													{
														CleaningBedDataTimeout = dc.cleaningBedDataTimeout,
														ConfigurationTimeout = dc.Device.configurationTimeout,
														OrderTimeout = dc.orderTimeout,
														PresenceTimeout = dc.presenceTimeout,
														RfidTimeout = dc.rfidTimeout
													},
												Type = (DeviceConfigurationType)Enum.Parse(typeof(DeviceConfigurationType), dc.Configuration.ConfigurationType.name),
												VisibleLocations = (from cl in dc.Configuration.ConfigurationLocations
																	select new LocationSummary()
																		{
																			Code = cl.Location.code,
																			LocationId = cl.locationId,
																			Name = cl.Location.name,
                                                                            WaitingRoomCode = ((cl.Location != null) && (cl.Location.WaitingArea != null)) ? cl.Location.WaitingArea.code : string.Empty
																		}).ToList()
											}).ToList();

					transactionScope.Complete();
				}
				return result;
			}
		}

		public PollingTimeouts GetLatestPollingTimeouts(string deviceName, int shortcutKeyNo)
		{
			return new DatabaseInvoker<PollingTimeouts>().Execute((efContext) =>
			{
				var pts = (from dc in efContext.DeviceConfiguration
						   where dc.Device.name == deviceName && dc.shortcutKeyNo == shortcutKeyNo
						   select new PollingTimeouts
						   {
							   CleaningBedDataTimeout = dc.cleaningBedDataTimeout,
							   OrderTimeout = dc.orderTimeout,
							   PresenceTimeout = dc.presenceTimeout,
							   RfidTimeout = dc.rfidTimeout,
							   ConfigurationTimeout = dc.Device.configurationTimeout,
                               DischargeTimeout = dc.dischargeTimeout,
                               AdmissionsTimeout = dc.admissionsTimeout
						   }).FirstOrDefault();
				return pts;
			});
		}

		#endregion

		#region Orders

		private Order GetOrder(WCSEntities efContext, int orderId, DateTime timestamp)
		{
            WCS.Data.EF.Order efOrder = OrdersById(efContext, orderId);
            bool isRadiationRisk = CheckOrdersForRadiationRisk(efOrder, timestamp);
			return efOrder.Convert(isRadiationRisk);
		}

		private WCS.Data.EF.Order GetEfOrder(WCSEntities efContext, int orderId)
		{
			return OrdersById(efContext, orderId);
		}

        public IList<Order> GetOrders(DateTime fromDate, DateTime toDate, LocationCodes locations, DateTime timestamp)
        {
            var efContext = new WCSEntities();
            var orders = 
                from o in efContext.Order
                    .Include("Admission")
                    .Include("Procedure")
                    .Include("Notifications")
                    .Include("Notes")
                where
                    o.procedureTime >= fromDate && o.procedureTime < toDate
                    && (locations.Count == 0  || locations.Contains(o.Department.code) || locations.Contains(o.Admission.Location.code) )
                select o;

            var admissionsIds = (from o in orders select o.admissionId).Distinct();
            
            var relatedOrders =
                from o in efContext.Order
                    .Include("Admission")
                    .Include("Procedure")
                    .Include("Notifications")
                    .Include("Notes")
                where
                    o.procedureTime >= fromDate && o.procedureTime < toDate
                    && admissionsIds.Contains(o.admissionId)
                select o;

            var allOrders = orders.Union(relatedOrders).ToList();

            List<Order> result = new List<Order>();

            foreach(var efOrder in allOrders)
            {
                int admissionId = efOrder.admissionId;
                var allEfOrdersForThisAdmission = (from o in allOrders where o.admissionId == admissionId select o).ToList();
                bool isRadiationRisk = CheckOrdersForRadiationRisk(allEfOrdersForThisAdmission, timestamp);

                Order newOrder = efOrder.Convert(isRadiationRisk);
                result.Add(newOrder);
            }

            return result;
        }

        public Order HideOrder(int orderId, DateTime timestamp, string source)
        {
            return DoHideUnhideOrder(orderId, true, timestamp, source);
        }

        public Order UnhideOrder(int orderId, DateTime timestamp, string source)
        {
            return DoHideUnhideOrder(orderId, false, timestamp, source);
        }

        private Order DoHideUnhideOrder(int orderId, bool isHidden, DateTime timestamp, string source)
        {
            DateTime effectiveTimestamp = timestamp.ToNearestSecond();

            return new DatabaseInvoker<Order>().Execute((efContext) =>
            {
                var order = GetEfOrder(efContext, orderId);

                order.isHidden = isHidden;

                var update = new WCS.Data.EF.Update()
                {
                    source = source,
                    type = WCSUpdateTypes.OrderHiddenFlagChanged,
                    value = isHidden.ToString(),
                    dateCreated = effectiveTimestamp
                };
                order.Updates.Add(update);

                efContext.SaveChanges();

                return GetOrder(efContext, orderId, timestamp);
            });
        }

        public Order AcknowledgeOrder(int orderId, DateTime timestamp, string source)
        {
            DateTime effectiveTimestamp = timestamp.ToNearestSecond(); 
            
            return new DatabaseInvoker<Order>().Execute((efContext) =>
            {
                var order = GetEfOrder(efContext, orderId);

                if (order != null)
                {
                    order.acknowledged = true;
                    order.acknowledgedTime = effectiveTimestamp;
                    order.acknowledgedBy = source;
                }

                order = GetEfOrder(efContext, order.orderId);
                var update = new WCS.Data.EF.Update()
                {
                    source = source,
                    type = WCSUpdateTypes.OrderAcknowledged,
                    value = order.orderNumber,
                    dateCreated = effectiveTimestamp
                };
                order.Updates.Add(update);

                efContext.SaveChanges();

                return GetOrder(efContext, order.orderId, timestamp);
            });
        }

        public Order AddOrderNote(int orderId, string note, DateTime timestamp, string source)
        {
            DateTime effectiveTimestamp = timestamp.ToNearestSecond(); 
            
            return new DatabaseInvoker<Order>().Execute((efContext) =>
            {
                var order = GetEfOrder(efContext, orderId);
                var noteToAdd = new WCS.Data.EF.Note()
                {
                    orderId = orderId,
                    source = source,
                    notes = note,
                    dateCreated = effectiveTimestamp,
                };
                order.Notes.Add(noteToAdd);

                var update = new WCS.Data.EF.Update()
                {
                    source = source,
                    type = WCSUpdateTypes.OrderNoteAdded,
                    value = note,
                    dateCreated = effectiveTimestamp
                };
                order.Updates.Add(update);

                efContext.SaveChanges();

                return GetOrder(efContext, orderId, timestamp);
            });
        }

        public Order DeleteOrderNote(int noteId, DateTime timestamp, string source)
        {
            DateTime effectiveTimestamp = timestamp.ToNearestSecond(); 
            
            return new DatabaseInvoker<Order>().Execute((efContext) =>
            {
                var noteToDelete = (from n in efContext.Note
                                    where n.noteId == noteId
                                    select n).FirstOrDefault();

                int orderId = noteToDelete.orderId.Value;

                if (noteToDelete != null)
                {
                    if (noteToDelete.source == source)
                    {
                        efContext.Note.DeleteObject(noteToDelete);
                    }
                    else
                    {
                        throw new AccessViolationException(
                            "Only creator of a note may remove it");
                    }
                }

                var order = GetEfOrder(efContext, orderId);
                var update = new WCS.Data.EF.Update()
                {
                    source = source,
                    type = WCSUpdateTypes.OrderNoteDeleted,
                    value = string.Empty,
                    dateCreated = effectiveTimestamp
                };
                order.Updates.Add(update);

                efContext.SaveChanges();

                return GetOrder(efContext, orderId, timestamp);
            });
        }

		/// <summary>
		/// Updates the procedure time of an order
		/// </summary>
		/// <param name="orderId">The order id.</param>
		/// <param name="procedureTime">The procedure time.</param>
		/// <param name="timestamp">The procedure time timestamp.</param>
		/// <param name="source">The device the request comes from </param>
		/// <returns>The Order object that is the target of the operation</returns>
        public Order UpdateProcedureTime(int orderId, DateTime procedureTime, DateTime timestamp, string source)
        {
            DateTime effectiveTimestamp = timestamp.ToNearestSecond(); 
            
            return new DatabaseInvoker<Order>().Execute((efContext) =>
            {
                var order = GetEfOrder(efContext, orderId);

                order.procedureTime = procedureTime;

                var update = new WCS.Data.EF.Update
                {
                    source = source,
                    type = WCSUpdateTypes.ProcedureTimeUpdated,
                    value = order.procedureTime.Value.ToShortTimeString(),
                    dateCreated = effectiveTimestamp
                };
                order.Updates.Add(update);

                efContext.SaveChanges();

                return GetOrder(efContext, orderId, timestamp);
            });
        }

        private static bool CheckOrdersForRadiationRisk(WCS.Data.EF.Order order, DateTime timestamp)
        {
            List<WCS.Data.EF.Order> orders = new List<WCS.Data.EF.Order>() { order };

            return CheckOrdersForRadiationRisk(orders, timestamp);
        }

        private static bool CheckOrdersForRadiationRisk(List<WCS.Data.EF.Order> orders, DateTime timestamp)
        {
            bool result = false;

            if (orders != null)
            {
                foreach (var currentOrder in orders)
                {
                    var notificationsWithRadiationRisk = currentOrder.Notifications.Where(n => n.radiationRiskDurationMinutes > 0);

                    foreach (WCS.Data.EF.Notification notificationWithRadiationRisk in notificationsWithRadiationRisk)
                    {
                        DateTime startOfRadiationRisk = DateTime.MinValue;

                        if (notificationWithRadiationRisk.acknowledged)
                        {
                            startOfRadiationRisk = notificationWithRadiationRisk.acknowledgedTime.Value;
                        }
                        else if (currentOrder.procedureTime.HasValue)
                        {
                            startOfRadiationRisk = currentOrder.procedureTime.Value.AddMinutes(-1.0 * notificationWithRadiationRisk.priorToProcedureTime);
                        }

                        DateTime endOfRadiationRisk = startOfRadiationRisk > DateTime.MinValue ? startOfRadiationRisk.AddMinutes(notificationWithRadiationRisk.radiationRiskDurationMinutes) : DateTime.MinValue;

                        if (timestamp >= startOfRadiationRisk && timestamp < endOfRadiationRisk)
                        {
                            result = true;
                            break;
                        }
                    }

                    if (result == true)
                    {
                        break;
                    }
                }
            }

            return result;
        }

		#endregion

		#region Devices

		public void LogDeviceInformation(WCSRequestHeaderData data, DateTime timestamp)
		{
			new DatabaseInvoker().Execute((efContext) =>
											{
												var device = (from d in efContext.Device
															  where d.name == data.Device
															  select d).FirstOrDefault();

												if (device == null) return;

												device.ipAddress = data.IPAddress;
                                                device.lastConnection = timestamp;
												device.os = data.OS;
												device.clientVersion = data.Version;

                                                var connection = new WCS.Data.EF.Connection { connectionTime = timestamp, Device = device };

												efContext.Connection.AddObject(connection);

												efContext.SaveChanges();
											});
		}

        /// <summary>
        /// Gets the latest location information on all the locations in the hospital
        /// </summary>
        /// <returns></returns>
        public IList<Presence> GetPresenceData(DateTime timestamp)
        {
            return new DatabaseInvoker<IList<Presence>>().Execute((efContext) =>
            {
                DateTime today = DateTime.Today;
                DateTime tomorrow = today.AddDays(1);
                DateTime yesterday = today.AddDays(-1);
                DateTime todayAt5 = DateTime.Today.AddHours(5);

                var activeUnscheduledOrders = (
                                                from o in efContext.Order
                                                where
                                                    o.OrderStatus.status == "InProgress" &&
                                                    o.procedureTime.HasValue &&
                                                    o.procedureTime >= today &&
                                                    o.procedureTime < todayAt5
                                                select o.orderId).ToList();

                var activeUnscheduledDepartmentCandidates = (from o in efContext.Order
                                                             where activeUnscheduledOrders.Contains(o.orderId)

                                                             select new
                                                             {
                                                                 o.orderId,
                                                                 o.Department.locationId,
                                                                 procedureTime = o.procedureTime,
                                                                 PatientType = o.Admission.AdmissionType.type,
                                                                 DeptCode = o.Department.code
                                                             }).ToList();

                var activeUnscheduledDepartmentTargets = (from candidate in activeUnscheduledDepartmentCandidates
                                                          where (candidate.procedureTime.HasValue && candidate.procedureTime.Value.TimeOfDay < TimeSpan.FromHours(5))
                                                          select new
                                                          {
                                                              candidate.locationId,
                                                              candidate.orderId
                                                          }).ToList();

                var activeScheduledOrders = (from o in efContext.Order
                                             where
                                                 o.OrderStatus.status == "InProgress" &&
                                                 o.procedureTime.HasValue &&
                                                 o.procedureTime >= todayAt5 &&
                                                 o.procedureTime < tomorrow
                                             select o.orderId).ToList();

                var activeUnscheduledDepartment = (from n in activeUnscheduledDepartmentTargets
                                                   group n by n.locationId
                                                       into ack
                                                       select new
                                                       {
                                                           LocationId = ack.Key,
                                                           ShortTermCount = 0,//ack.Where(st => st.Delay < 5).Count(),
                                                           LongTermCount = ack.Count()
                                                       }).ToList();


                var unAckNotifications = (from o in efContext.Order.Include("Admission")
                                          join n in efContext.Notification on o.orderId equals n.orderId
                                          where
                                              !n.acknowledged
                                              && o.procedureTime.HasValue
                                              && o.procedureTime >= todayAt5
                                              && activeScheduledOrders.Contains(o.orderId)
                                          select new
                                          {
                                              o,
                                              n
                                          }).ToList();


                // finds the overdue notifications for the ward
                var notificationWardCandidates = (from unackn in unAckNotifications
                                                  where ((unackn.o.procedureTime.Value - TimeSpan.FromMinutes(unackn.n.priorToProcedureTime)) < timestamp)
                                                  select new
                                                  {
                                                      //unackn.o.orderId,
                                                      //unackn.n.notificationId,
                                                      //st = unackn.o.procedureTime.Value - TimeSpan.FromMinutes(unackn.n.priorToProcedureTime),

                                                      unackn.o.Admission.Location.locationId,
                                                      unackn.o.orderId,
                                                      unackn.o.acknowledged,
                                                      PatientType = unackn.o.Admission.AdmissionType.type,
                                                      procedureTime = unackn.o.procedureTime.Value - TimeSpan.FromMinutes(unackn.n.priorToProcedureTime),
                                                      DeptCode = unackn.o.Department.code
                                                  }).ToList();


                // finds the overdue orders for the ward
                var orderWardCandidates = (from o in efContext.Order.Include("Admission")
                                           where
                                               !o.acknowledged
                                               && o.procedureTime.HasValue
                                               && o.procedureTime >= todayAt5
                                               && activeScheduledOrders.Contains(o.orderId)
                                           select new
                                           {
                                               o.Admission.Location.locationId,
                                               o.orderId,
                                               o.acknowledged,
                                               PatientType = o.Admission.AdmissionType.type,
                                               procedureTime = o.procedureTime.Value,
                                               DeptCode = o.Department.code
                                           }).ToList();


                // combine the notifications and the orders for the ward
                var wardCandidates = notificationWardCandidates.Union(orderWardCandidates).ToList();


                var wardTargets = (from candidate in wardCandidates
                                   where (candidate.procedureTime < timestamp)
                                   group candidate by candidate.orderId
                                       into cands
                                       let maxDate = cands.Max(d => (timestamp - d.procedureTime).TotalMinutes)
                                       from cand in cands
                                       where cands.Max(d => (timestamp - cand.procedureTime).TotalMinutes) == maxDate
                                       select new
                                       {
                                           cand.locationId,
                                           cand.orderId,
                                           Delay = cands.Max(d => (timestamp - cand.procedureTime).TotalMinutes)
                                       }).ToList();


                var unAckWard = (from n in wardTargets
                                 group n by n.locationId
                                     into ack
                                     select
                                     new
                                     {
                                         LocationId = ack.Key,
                                         ShortTermCount = ack.Where(st => st.Delay < 5).Count(),
                                         LongTermCount = ack.Where(lt => lt.Delay > 5).Count()
                                     }).ToList();


                var unAcks = unAckWard.Union(activeUnscheduledDepartment).ToList();


                var latestLocationConnections = (from device in efContext.Device
                                                 where device.locationId != null
                                                 && device.locationId.HasValue
                                                 group device by device.locationId
                                                     into ds
                                                     let maxDate = ds.Max(d => d.lastConnection)
                                                     from d in ds
                                                     where d.lastConnection == maxDate
                                                     select new { locationId = d.locationId.Value, d.lastConnection, d.deviceId }).ToList();



                var locations = (from location in efContext.Location.AsEnumerable()
                                 join ldc in latestLocationConnections on location.locationId equals ldc.locationId
                                  into iLocs
                                 from iLoc in iLocs.DefaultIfEmpty()
                                 select new Presence
                                 {
                                     LocationId = location.locationId,
                                     TimeSinceLastConnection = iLoc != null ? iLoc.lastConnection.HasValue ? (timestamp - iLoc.lastConnection.Value) : default(TimeSpan?) : default(TimeSpan?),
                                     LocationCode = location.code,
                                     Contact = location.contactInfo,
                                     ShortTermNotificationsPending = 0,
                                     LongTermNotificationsPending = 0
                                 }
                                             ).ToList();


                unAcks.ForEach(ack =>
                {
                    var locationConnection = locations.FirstOrDefault(dl => dl.LocationId == ack.LocationId);
                    if (locationConnection == null)
                        return;

                    locationConnection.ShortTermNotificationsPending = ack.ShortTermCount;
                    locationConnection.LongTermNotificationsPending = ack.LongTermCount;
                });

                return locations;
            });
        }

        #endregion

		#region Notifications

        public Order AcknowledgeNotification(int notificationId, DateTime timestamp, string source)
		{
            DateTime effectiveTimestamp = timestamp.ToNearestSecond();

			return new DatabaseInvoker<Order>().Execute((efContext) =>
																		{
																			var notification = (from n in efContext.Notification
																								where
																									n.notificationId == notificationId
																								select n).FirstOrDefault();

																			if (notification != null)
																			{
																				notification.acknowledged = true;
                                                                                notification.acknowledgedTime = effectiveTimestamp;
																				notification.acknowledgedBy = source;
																			}

																			var order = GetEfOrder(efContext, notification.orderId);
																			var update = new WCS.Data.EF.Update()
																							{
																								source = source,
																								type =
																								WCSUpdateTypes.NotificationAcknowledged,
																								value = notification.description,
                                                                                                dateCreated = effectiveTimestamp
																							};
																			order.Updates.Add(update);

																			efContext.SaveChanges();

																			return GetOrder(efContext, notification.orderId, timestamp);
																		});
		}

		#endregion

		#region Cleaning

		private WCS.Data.EF.Bed GetEfBed(WCSEntities efContext, int bedId)
		{
			return BedById(efContext, bedId);
		}

        public IList<Bed> GetCleaningBedData(LocationCodes locations, DateTime timestamp)
        {
            return GetCleaningBedDataByLocationOrForOneBed(locations, null, timestamp);
        }

        private IList<Bed> GetCleaningBedDataByLocationOrForOneBed(LocationCodes locations, int? bedId, DateTime timestamp)
        {
            // It seems to be important to EF that locations is not null, otherwise it doesn't seem to be able to realise this is a collection of
            // strings and can't use it in the queries
            bool searchByLocation = true;
            if (locations == null)
            {
                searchByLocation = false;
                locations = new LocationCodes();
            }

            // Cleaning data is only relevant for today
            DateTime fromDate = DateTime.Today;
            DateTime toDate = DateTime.Today.AddDays(1);

            DateTime startOfCleaningDay = DateTime.Today.AddHours(8);
            DateTime endOfCleaningDay = DateTime.Today.AddHours(20);

            var efContext = new WCSEntities();

            var allBedData = 
                (from bed in efContext.Bed
                    .Include("Room")
                    .Include("Room.Location")
                where
                    (searchByLocation && (locations.Count == 0 || locations.Contains(bed.Room.Location.code)))
                    || (bedId != null && bed.bedId == bedId)
                select new
                {
                    ThisBed = bed,
                    MostRecentAdmission = (from adm in bed.Admission select adm).OrderByDescending(a=>a.admitDateTime).FirstOrDefault(),
                    MostRecentBedCleanedEvent = (from bce in bed.BedCleaningEvent where bce.BedCleaningEventType.eventType == "BedCleaned" select bce).OrderByDescending(bce => bce.timestamp).FirstOrDefault(),
                    MostRecentBedMarkedAsDirtyEvent = (from bce in bed.BedCleaningEvent where bce.BedCleaningEventType.eventType == "BedMarkedAsDirty" select bce).OrderByDescending(bce => bce.timestamp).FirstOrDefault(),
                    AdmissionsInRange = bed.Admission.Where(adm => 
                        (adm.admitDateTime.HasValue && adm.admitDateTime.Value >= fromDate && adm.admitDateTime.Value < toDate) 
                        || (adm.dischargeDateTime.HasValue && adm.dischargeDateTime.Value >= fromDate && adm.dischargeDateTime.Value < toDate)
                        || ((adm.AdmissionStatus.admissionStatusId == (int)AdmissionStatus.Admitted) && (adm.AdmissionType.admissionTypeId == (int)AdmissionType.In))
                        ),
                    BedCleaningEventsInRange = bed.BedCleaningEvent.Where(bce => bce.timestamp >= fromDate && bce.timestamp < toDate),
                    NotesInRange = bed.Notes.Where(n => n.dateCreated.HasValue && n.dateCreated.Value >= fromDate && n.dateCreated.Value < toDate)
                }).ToList();

            var bedsWithOrdersDuringDateRange =
                (from o in efContext.Order
                where
                o.Admission != null
                && o.Admission.Bed != null
                && o.procedureTime.HasValue
                && o.procedureTime >= fromDate
                && o.procedureTime < toDate
                &&
                (
                    (searchByLocation && (locations.Count == 0 || locations.Contains(o.Admission.Location.code)))
                    || (bedId != null && o.Admission.Bed.bedId == bedId)
                )
                select new
                {
                    o.Admission.Bed.bedId,
                    o.procedureTime,
                    o.estimatedProcedureDuration,
                    o.Procedure.durationMinutes,
                    Order = o
                }).ToList();


            IList<Bed> result = new List<Bed>();

            foreach (var allDataForCurrentBed in allBedData)
            {
                Bed b = new Bed()
                {
                    BedId = allDataForCurrentBed.ThisBed.bedId,
                    Name = allDataForCurrentBed.ThisBed.name,
                    Room = RoomFromEfBed(allDataForCurrentBed.ThisBed),
                    Location = LocationFromEfBed(allDataForCurrentBed.ThisBed),
                    EstimatedDischargeDate = allDataForCurrentBed.MostRecentAdmission != null ? allDataForCurrentBed.MostRecentAdmission.estimatedDischargeDateTime : null,
                    CriticalCareIndicators = new CriticalCareIndicators()
                    {
                        IsMrsaRisk = false, // Default to false
                        IsFallRisk = false,
                        HasLatexAllergy = false,
                        IsRadiationRisk = false
                    },
                    CurrentStatus = BedStatus.Dirty, // Default to dirty
                };

                // Set services
                var cleaningServices = from bce in allDataForCurrentBed.BedCleaningEventsInRange
                                       select new CleaningService()
                                       {
                                           CleaningServiceId = bce.bedCleaningEventId,
                                           ServiceTime = bce.timestamp,
                                           CleaningServiceType = (BedCleaningEventType)Enum.Parse(typeof(BedCleaningEventType), bce.BedCleaningEventType.eventType)
                                       };
                b.Services = cleaningServices.ToList();
                b.LatestService = b.Services.OrderBy(service => service.ServiceTime).LastOrDefault();

                // Set notes
                var notes = from n in allDataForCurrentBed.NotesInRange
                                select n.Convert();
                b.Notes = notes.ToList();

                // Helper variables for all the calculations that will require dates
                DateTime mostRecentBedCleanedTimestamp = allDataForCurrentBed.MostRecentBedCleanedEvent != null ? allDataForCurrentBed.MostRecentBedCleanedEvent.timestamp : DateTime.MinValue;
                DateTime mostRecentBedMarkedAsDirtyTimestamp = allDataForCurrentBed.MostRecentBedMarkedAsDirtyEvent != null ? allDataForCurrentBed.MostRecentBedMarkedAsDirtyEvent.timestamp : DateTime.MinValue;

                // Work out if bed is MRSA positive from most recent admission
                if (allDataForCurrentBed.MostRecentAdmission != null)
                {
                    b.CriticalCareIndicators.IsMrsaRisk = allDataForCurrentBed.MostRecentAdmission.Patient.isMrsaPositive;
                    b.CriticalCareIndicators.IsFallRisk = allDataForCurrentBed.MostRecentAdmission.Patient.isFallRisk;
                    b.CriticalCareIndicators.HasLatexAllergy = allDataForCurrentBed.MostRecentAdmission.Patient.hasLatexAllergy;

                    // If patient has been discharged and the room has been  cleaned then it is no longer MRSA positive
                    if (allDataForCurrentBed.MostRecentAdmission.dischargeDateTime.HasValue)
                    {
                        if (mostRecentBedCleanedTimestamp >= allDataForCurrentBed.MostRecentAdmission.dischargeDateTime.Value)
                        {
                            b.CriticalCareIndicators.IsMrsaRisk = false;
                        }
                    }
                }

                // First set the current status to clean if there is a cleaning event today except if the bed has been specifically marked as dirty
                if (mostRecentBedCleanedTimestamp.Date == DateTime.Today)
                {
                    if (mostRecentBedCleanedTimestamp > mostRecentBedMarkedAsDirtyTimestamp)
                    {
                        b.CurrentStatus = BedStatus.Clean;
                    }
                }

                DateTime mostRecentDischargeTime = allDataForCurrentBed.MostRecentAdmission != null ? allDataForCurrentBed.MostRecentAdmission.dischargeDateTime != null ? allDataForCurrentBed.MostRecentAdmission.dischargeDateTime.Value : DateTime.MinValue : DateTime.MinValue;

                // Check for reasons that the bed would need a deep clean
                if (mostRecentDischargeTime > mostRecentBedCleanedTimestamp)
                {
                    b.CurrentStatus = BedStatus.RequiresDeepClean;
                }

                // Set the available times
                b.AvailableTimes = new List<BedTime>();

                // Beds with no admissions are free all day.
                if (allDataForCurrentBed.AdmissionsInRange.Count() == 0)
                {
                    b.AvailableTimes.Add(
                        new BedTime()
                        {
                            BedId = b.BedId,
                            StartTime = startOfCleaningDay,
                            EndTime = endOfCleaningDay,
                            IsDueToDischarge = false
                        });
                }
                else
                {
                    // Bed has admission so first use the orders to determine if there is a radiation risk
                    var efOrdersToDetermineRadiationRisk =
                        (
                            from bedWithOrders in bedsWithOrdersDuringDateRange
                            where bedWithOrders.bedId == allDataForCurrentBed.ThisBed.bedId
                            select bedWithOrders.Order
                        ).ToList();

                    b.CriticalCareIndicators.IsRadiationRisk = CheckOrdersForRadiationRisk(efOrdersToDetermineRadiationRisk, timestamp);


                    // Now look to create bed times for the corresponding orders for this bed
                    var ordersInDateRangeForThisBedAsList = 
                        (
                            from bedWithOrders in bedsWithOrdersDuringDateRange 
                            where bedWithOrders.bedId == allDataForCurrentBed.ThisBed.bedId
                            select new
                            {
                                BedId = bedWithOrders.bedId,
                                ProcedureTime = bedWithOrders.procedureTime.Value,
                                EstimatedProcedureDuration = bedWithOrders.estimatedProcedureDuration,
                                DurationMinutes = bedWithOrders.durationMinutes
                            }
                        ).ToList();

                    var bedTimes =
                        from o in ordersInDateRangeForThisBedAsList
                        select new BedTime()
                        {
                            BedId = o.BedId,
                            StartTime = o.ProcedureTime,
                            EndTime = o.ProcedureTime.AddMinutes(InferProcedureDuration(o.EstimatedProcedureDuration, o.DurationMinutes)),
                            IsDueToDischarge = false
                        };

                    b.AvailableTimes.AddRange(bedTimes);

                    List<BedTime> gapsFromAdmissions = FindGapsFromAdmissions(b.BedId, allDataForCurrentBed.AdmissionsInRange, startOfCleaningDay, endOfCleaningDay);

                    b.AvailableTimes.AddRange(gapsFromAdmissions);
                }
                
                result.Add(b);
            }

            return result;
        }

        // Beds with an admission/discharge today and those with more than one admission today could have availability for cleaning in between them
        private List<BedTime> FindGapsFromAdmissions(int bedId, IEnumerable<WCS.Data.EF.Admission> admissions, DateTime startOfCleaningDay, DateTime endOfCleaningDay)
        {
            List<BedTime> result = new List<BedTime>();

            if (admissions.Count() > 0)
            {
                var sortedAdmissions = admissions.OrderBy(a => a.admitDateTime);

                // Look for a gap before the start of the day and the first admission
                WCS.Data.EF.Admission earliestAdmission = sortedAdmissions.ElementAt(0);

                if (earliestAdmission.admitDateTime.HasValue && earliestAdmission.admitDateTime.Value > startOfCleaningDay)
                {
                    result.Add(new BedTime()
                    {
                        BedId = bedId,
                        StartTime = startOfCleaningDay,
                        EndTime = earliestAdmission.admitDateTime.Value,
                        IsDueToDischarge = false
                    });
                }

                // Go through the admissions looking for potential gaps after discharge
                for (int i = 0; i < sortedAdmissions.Count(); i++)
                {
                    WCS.Data.EF.Admission currentAdmission = sortedAdmissions.ElementAt(i);
                    WCS.Data.EF.Admission nextAdmission = null;
                    if (i < sortedAdmissions.Count() - 1)
                    {
                        nextAdmission = sortedAdmissions.ElementAt(i + 1);
                    }

                    // If the admission has a discharge time it is a candidate for creating availability
                    if (currentAdmission.dischargeDateTime.HasValue && currentAdmission.dischargeDateTime.Value < endOfCleaningDay)
                    {
                        DateTime candidateStartTime = currentAdmission.dischargeDateTime.Value;

                        DateTime candidateEndTime;

                        // The end time will be the next admission time if present or the end of the cleaning day
                        if (nextAdmission != null && nextAdmission.admitDateTime.HasValue && nextAdmission.admitDateTime.Value < endOfCleaningDay)
                        {
                            candidateEndTime = nextAdmission.admitDateTime.Value;
                        }
                        else
                        {
                            candidateEndTime = endOfCleaningDay;
                        }

                        // If theres a gap add a bed availability event
                        if (candidateStartTime < candidateEndTime)
                        {
                            result.Add(new BedTime()
                            {
                                BedId = bedId,
                                StartTime = candidateStartTime,
                                EndTime = candidateEndTime,
                                IsDueToDischarge = true
                            });
                        }
                    }
                }
            }

            return result;
        }

        private int InferProcedureDuration(int? estimatedProcedureDuration, int? procedureDuration)
		{
			// Checked estimated and procedure durations for values, otherwise default to one hour.
			return estimatedProcedureDuration.HasValue ? estimatedProcedureDuration.Value : procedureDuration.HasValue ? procedureDuration.Value : 60;
		}

		public Bed MarkBedAsClean(int bedId, DateTime timestamp, string callingDevice)
		{
            return AddBedCleaningEvent(bedId, timestamp, WCSUpdateTypes.BedMarkedClean, callingDevice);
		}

        public Bed MarkBedAsDirty(int bedId, DateTime timestamp, string callingDevice)
		{
            return AddBedCleaningEvent(bedId, timestamp, WCSUpdateTypes.BedMarkedDirty, callingDevice);
		}

        private Bed AddBedCleaningEvent(int bedId, DateTime timestamp, string updateType, string callingDevice)
		{
            DateTime effectiveTimestamp = timestamp.ToNearestSecond();

			return new DatabaseInvoker<Bed>().Execute((efContext) =>
																		{
																			var bedToUpdate = GetEfBed(efContext, bedId);

                                                                            string eventType = updateType == WCSUpdateTypes.BedMarkedClean ? "BedCleaned" : "BedMarkedAsDirty";

                                                                            var bcet = (from b in efContext.BedCleaningEventType where b.eventType == eventType select b).First();

																			var bce = new WCS.Data.EF.BedCleaningEvent()
																			{
																				Bed = bedToUpdate,
                                                                                timestamp = effectiveTimestamp,
																				BedCleaningEventType = bcet
																			};

																			bedToUpdate.BedCleaningEvent.Add(bce);

																			var update = new WCS.Data.EF.Update()
																							{
																								source = callingDevice,
																								type = updateType,
																								value = String.Empty,
                                                                                                dateCreated = effectiveTimestamp
																							};
																			bedToUpdate.Updates.Add(update);


																			efContext.SaveChanges();

																			return GetCleaningBedDataByLocationOrForOneBed(null, bedId, timestamp).First();
																		});
		}

        public Bed AddBedNote(int bedId, string note, DateTime timestamp, string source)
        {
            DateTime effectiveTimestamp = timestamp.ToNearestSecond(); 
            
            return new DatabaseInvoker<Bed>().Execute((efContext) =>
            {
                var bed = GetEfBed(efContext, bedId);

                var noteToAdd = new WCS.Data.EF.Note()
                {
                    bedId = bedId,
                    source = source,
                    notes = note,
                    dateCreated = effectiveTimestamp,
                };
                bed.Notes.Add(noteToAdd);

                var update = new WCS.Data.EF.Update()
                {
                    source = source,
                    type = WCSUpdateTypes.BedNoteAdded,
                    value = note,
                    dateCreated = effectiveTimestamp
                };
                bed.Updates.Add(update); efContext.SaveChanges();

                return GetCleaningBedDataByLocationOrForOneBed(null, bedId, timestamp).First();
            });
        }

        public Bed DeleteBedNote(int noteId, DateTime timestamp, string source)
        {
            DateTime effectiveTimestamp = timestamp.ToNearestSecond();

            return new DatabaseInvoker<Bed>().Execute((efContext) =>
            {
                var noteToDelete = (from n in efContext.Note
                                    where n.noteId == noteId
                                    select n).FirstOrDefault();

                int bedId = noteToDelete.bedId.Value;

                if (noteToDelete != null)
                {
                    if (noteToDelete.source == source)
                    {
                        efContext.Note.DeleteObject(noteToDelete);
                    }
                    else
                    {
                        throw new AccessViolationException("Only creator of a note may remove it");
                    }
                }

                var bed = GetEfBed(efContext, bedId);
                var update = new WCS.Data.EF.Update()
                {
                    source = source,
                    type = WCSUpdateTypes.BedNoteDeleted,
                    value = string.Empty,
                    dateCreated = effectiveTimestamp
                };
                bed.Updates.Add(update);

                efContext.SaveChanges();

                return GetCleaningBedDataByLocationOrForOneBed(null, bedId, timestamp).First();
            });
        }

		#endregion

		#region RFID

        public IList<Detection> GetEntireDetectionHistory(DateTime fromDate, DateTime toDate, PatientCodes patientCodes)
        {
            return new DatabaseInvoker<IList<Detection>>().Execute((efContext) =>
            {
                return (from detection in efContext.RfidDetection
                        where
                        (patientCodes.Count == 0 || patientCodes.Contains(detection.Patient.externalId))
                        && detection.dateTimeDetected >= fromDate && detection.dateTimeDetected < toDate
                        select detection).ToList().Select(d => d.Convert()).ToList();
            });
        }

        public IList<Detection> GetLatestDetection(DateTime fromDate, DateTime toDate, PatientCodes patientCodes)
		{
			var results = new DatabaseInvoker<IList<Detection>>().Execute((efContext) =>
			{
				return (from detection in efContext.RfidDetection
                        where detection.dateTimeDetected >= fromDate && detection.dateTimeDetected < toDate
                        && (patientCodes.Count == 0 || patientCodes.Contains(detection.Patient.externalId))
						group detection by detection.patientId into personalDetections
						let latest = personalDetections.Max(gt => gt.dateTimeDetected)
						select personalDetections.FirstOrDefault(pd => pd.dateTimeDetected == latest)).ToList().Select(d => d.Convert()).ToList();
			});

            return results;
		}

		public Detection InsertDetection(string trackableIdSource, string trackableId, string locationSource, string location, DetectionDirection direction, DateTime timestamp)
		{

			return new DatabaseInvoker<Detection>().Execute((efContext) =>
																	{
																		var p = (from pat in efContext.Patient
																				 where pat.ExternalSource.source == trackableIdSource && pat.externalId == trackableId
																				 select pat).FirstOrDefault();

																		if (p == null)
																			throw new InvalidOperationException(string.Format("Cannot find patient with external source {0} and id {1}  for tracking", trackableIdSource, trackableId));

																		var l = (from loc in efContext.RfidDetector
																				 where loc.ExternalSource.source == locationSource && loc.externalId == location
																				 select loc).FirstOrDefault();
																		if (l == null)
																			throw new InvalidOperationException(string.Format("Cannot find location with external source {0} and id {1} for tracking", locationSource, location));

																		string directionAsText = direction.ToString();
																		var d = (from dir in efContext.RfidDirection
																				 where dir.direction == directionAsText
																				 select dir).FirstOrDefault();
																		if (d == null)
																			throw new InvalidOperationException(string.Format("Cannot find direction {0} for tracking", direction.ToString()));

																		var newDetection = new RfidDetection()
                                                                            {
                                                                                dateTimeDetected = timestamp,
                                                                                RfidDirection = d,
                                                                                RfidDetector = l,
                                                                                Patient = p
                                                                            };

																		efContext.RfidDetection.AddObject(newDetection);
																		efContext.SaveChanges();

																		return (from detection in efContext.RfidDetection
																				where detection.rfidDetectionId == newDetection.rfidDetectionId
																				select detection).ToList().Select(def => def.Convert()).FirstOrDefault();


																	});
		}

		private static Random _random = new Random();

        public void RandomlyMovePatient(DateTime timestamp)
		{
			new DatabaseInvoker().Execute((efContext) =>
													{
														lock (_random)
														{
															var allPatients = (from o in efContext.Order
																			   select new
																						{
																							dept = o.Department,
																							ward = o.Admission.Location,
																							o.Admission.Patient.externalId,
																							o.Admission.patientId
																						}).ToList();


															int toSkip = _random.Next(0, allPatients.Count);

															var targetPatient = allPatients.Skip(toSkip).Take(1).First();

                                                            var pcs = new PatientCodes() { targetPatient.externalId };
                                                            var currentLocation = GetLatestDetection(DateTime.Today, DateTime.Today.AddDays(1), pcs).FirstOrDefault();

															if (currentLocation == null)
															{
                                                                InsertDetection("HIS", targetPatient.externalId, "RFID", targetPatient.dept.name, DetectionDirection.In, timestamp);
															}
															else if (currentLocation.DetectionLocation.LocationName == targetPatient.ward.name)
															{
                                                                InsertDetection("HIS", targetPatient.externalId, "RFID", targetPatient.ward.name, DetectionDirection.Out, timestamp);
																new ManualResetEventSlim().Wait(1000);
                                                                InsertDetection("HIS", targetPatient.externalId, "RFID", targetPatient.dept.name, DetectionDirection.In, timestamp);
															}
															else if (currentLocation.DetectionLocation.LocationName == targetPatient.dept.name)
															{
                                                                InsertDetection("HIS", targetPatient.externalId, "RFID", targetPatient.dept.name, DetectionDirection.Out, timestamp);
																new ManualResetEventSlim().Wait(1000);
                                                                InsertDetection("HIS", targetPatient.externalId, "RFID", targetPatient.ward.name, DetectionDirection.In, timestamp);
															}
															else
															{
                                                                InsertDetection("HIS", targetPatient.externalId, "RFID", currentLocation.DetectionLocation.LocationName, DetectionDirection.Out, timestamp);
																new ManualResetEventSlim().Wait(1000);
                                                                InsertDetection("HIS", targetPatient.externalId, "RFID", targetPatient.ward.name, DetectionDirection.In, timestamp);
															}
														}
													});
		}

		#endregion

        public IList<BedDischargeData> GetDischargesBedData(LocationCodes locations, DateTime timestamp)
        {
            return GetDischargesBedDataByLocationOrForOneBed(locations, null, timestamp);
        }

        private IList<BedDischargeData> GetDischargesBedDataByLocationOrForOneBed(LocationCodes locations, int? bedId, DateTime timestamp)
        {
            // It seems to be important to EF that locations is not null, otherwise it doesn't seem to be able to realise this is a collection of
            // strings and can't use it in the queries
            bool searchByLocation = true;
            if (locations == null)
            {
                searchByLocation = false;
                locations = new LocationCodes();
            }

            var efContext = new WCSEntities();

            var allBedData =
                from bed in efContext.Bed
                    .Include("Room")
                    .Include("Room.Location")
                where
                    (searchByLocation && (locations.Count == 0 || locations.Contains(bed.Room.Location.code)))
                    || (bedId != null && bed.bedId == bedId)
                select new
                {
                    ThisBed = bed,
                    CurrentAdmission =
                        (
                            from adm in bed.Admission 
                            where
                                adm.admitDateTime <= timestamp
                                &&
                                (
                                    !(adm.dischargeDateTime.HasValue && adm.dischargeDateTime.Value <= timestamp)
                                    &&
                                    !(adm.estimatedDischargeDateTime.HasValue && adm.estimatedDischargeDateTime.Value <= timestamp)
                                )
                            select adm
                        ).OrderByDescending(a => a.admitDateTime).FirstOrDefault()
                };

            IList<BedDischargeData> result = new List<BedDischargeData>();
            foreach (var allDataForCurrentBed in allBedData)
            {
                DateTime? estimatedDischargeDateLastUpdated = null;
                if (allDataForCurrentBed.CurrentAdmission != null)
                {
                    var mostRecentUpdateForEstimatedDischargeDate =
                            (
                                from u in allDataForCurrentBed.CurrentAdmission.Updates
                                where
                                    u.type == WCSUpdateTypes.EstimatedDischargeDateUpdated
                                    && u.dateCreated <= timestamp
                                select u
                            ).OrderByDescending(u => u.dateCreated).FirstOrDefault();
                    estimatedDischargeDateLastUpdated = mostRecentUpdateForEstimatedDischargeDate != null ? mostRecentUpdateForEstimatedDischargeDate.dateCreated : null;
                }

                bool isRadiationRisk = false;
                if (allDataForCurrentBed.CurrentAdmission != null)
                {
                    isRadiationRisk = CheckOrdersForRadiationRisk(allDataForCurrentBed.CurrentAdmission.Order.ToList(), timestamp);
                }

                BedDischargeData bdd = new BedDischargeData()
                {
                    BedId = allDataForCurrentBed.ThisBed.bedId,
                    Name = allDataForCurrentBed.ThisBed.name,
                    Room = RoomFromEfBed(allDataForCurrentBed.ThisBed),
                    Location = LocationFromEfBed(allDataForCurrentBed.ThisBed),
                    CurrentAdmission = allDataForCurrentBed.CurrentAdmission != null ? allDataForCurrentBed.CurrentAdmission.Convert(isRadiationRisk) : null,
                    EstimatedDischargeDateLastUpdated = estimatedDischargeDateLastUpdated
                };

                result.Add(bdd);
            }

            return result;
        }

        public BedDischargeData UpdateEstimatedDischargeDate(int admissionId, DateTime estimatedDischargeDate, DateTime timestamp, string source)
        {
            BedDischargeData result;

            DateTime effectiveEstimatedDischargeDate = estimatedDischargeDate.ToNearestSecond();
            DateTime effectiveTimestamp = timestamp.ToNearestSecond();

            int? bedIdToReturn;

            var transactionOptions = new System.Transactions.TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted };
            using (var transactionScope = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, transactionOptions))
            {
                using (var efContext = new WCSEntities())
                {
                    var admissionToUpdate =
                        (from admission in efContext.Admission
                         where admission.admissionId == admissionId
                         select admission).FirstOrDefault();

                    if (admissionToUpdate == null)
                    {
                        throw new ApplicationException(String.Format("No admission found with id {0}.", admissionId));
                    }

                    bedIdToReturn = admissionToUpdate.Bed_bedId;

                    admissionToUpdate.estimatedDischargeDateTime = effectiveEstimatedDischargeDate;

                    var update = new WCS.Data.EF.Update()
                    {
                        source = source,
                        type = WCSUpdateTypes.EstimatedDischargeDateUpdated,
                        value = effectiveEstimatedDischargeDate.ToString(),
                        dateCreated = effectiveTimestamp
                    };
                    admissionToUpdate.Updates.Add(update);

                    efContext.SaveChanges();
                }
                transactionScope.Complete();
            }

            if( bedIdToReturn.HasValue)
            {
                result = GetDischargesBedDataByLocationOrForOneBed(null, bedIdToReturn, effectiveTimestamp).First();
            }
            else
            {
                result = null;
            }

            return result;
        }

        private static Location LocationFromEfBed(WCS.Data.EF.Bed bed)
        {
            return new Location()
            {
                Bed = bed.name,
                Room = bed.Room.name,
                Name = bed.Room.Location.code,
                FullName = bed.Room.Location.name,
                IsEmergency = bed.Room.Location.isEmergency
            };
        }

        private static Room RoomFromEfBed(WCS.Data.EF.Bed bed)
        {
            return new Room()
            {
                RoomId = bed.Room.roomId,
                Name = bed.Room.name,
                Ward = bed.Room.Location.code
            };
        }

        public AdmissionsData GetAdmissionsData(LocationCodes locations, DateTime timestamp)
        {
            AdmissionsData result = new AdmissionsData();

            LocationCodes allBeds = new LocationCodes();
            
            result.Beds = GetDischargesBedDataByLocationOrForOneBed(allBeds, null, timestamp);

            var efContext = new WCSEntities();

            var efAdmissions =
                (
                from a in efContext.Admission
                where locations.Contains(a.Location.code)
                select a
                ).ToList();

            var admissions = efAdmissions.Select(a => a.Convert(false)).ToList();

            result.Admissions = admissions;

            return result;
        }
    }
}

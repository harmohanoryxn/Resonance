using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using GalaSoft.MvvmLight;
using WCS.Core;
using WCS.Shared.Controls;
using WCS.Shared.Orders;
using WCS.Shared.Timeline;

namespace WCS.Shared.Schedule
{
    /// <summary>
    /// Coordinates all the interactions between all the of different timeline items 
    /// </summary>
    public class TimelineCoordinator : ViewModelBase
    {
        #region Fields

        private TimelineItemObservableCollection _timelineItems;
        private CollectionViewSource _timelineSource;
        private TimelineItemType _itemTypes;

        #endregion

        #region Public API

        #region Constructors

        public TimelineCoordinator(TimelineItemType itemTypes)
        {
            _itemTypes = itemTypes;
            TimelineItems = new TimelineItemObservableCollection();

            if (Application.Current != null)
            {
                Application.Current.Dispatcher.InvokeIfRequired((() =>
                                                                     {
                                                                         TimelineSource = new CollectionViewSource();
                                                                         TimelineSource.SortDescriptions.Add(
                                                                             new SortDescription("StartTime",
                                                                                                 ListSortDirection.
                                                                                                     Ascending));
                                                                         TimelineSource.Source = TimelineItems;
                                                                     }));
            }
        }

        #endregion

        #region IDispose

        private bool _disposed = false;

        protected override void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (_timelineItems != null)
                {
                    _timelineItems.ForEach(n =>
                    {
                        if (n != null)
                        {
                            n.Dispose();
                        }
                    });
                    _timelineItems.Clear();
                    _timelineItems = null;
                }

                _disposed = true;

            }
        }
        #endregion

		/// <summary>
		/// Tries to update the coordinators collections with a potentially different order's details
		/// </summary>
		/// <param name="order">The order.</param>
		/// <param name="notificationTypeFilter">The notification type filter.</param>
        internal void Synchronise(Order order, NotificationType? notificationTypeFilter)
        {
            // Notes
            if (_itemTypes.IsSet(TimelineItemType.NoteIn | TimelineItemType.NoteOut))
            {
                MergeAlgorithm<Note, ITimelineItem>.Merge(order.Notes.Where(s => s.DateCreated.Date == DateTime.Today),
                    _timelineItems.Where(a => a.TimelineType == TimelineItemType.NoteIn || a.TimelineType == TimelineItemType.NoteOut),
                    (toAdd) => DoAddItem(toAdd),
                    (toRemove) => DoRemoveItem(toRemove));
            }

            // Notification Acknowledgments
            if (_itemTypes.IsSet(TimelineItemType.NotificationAcknowlegement))
            {
                MergeAlgorithm<Notification, ITimelineItem>.Merge(order.Notifications.Where(n => n.Acknowledged),
                                                                  _timelineItems.Where(a => a.TimelineType == TimelineItemType.NotificationAcknowlegement),
                                                                  (toAdd) => DoAddItem(toAdd),
                                                                  (toRemove) => DoRemoveItem(toRemove));
            }

            // procedure time updates
            if (_itemTypes.IsSet(TimelineItemType.ProcedureTimeUpdated))
            {
                var existingItems = TimelineItems.Where(ti => ti.TimelineType == TimelineItemType.ProcedureTimeUpdated).ToList();
                var existingTimes = existingItems.Select(ei => ei.StartTime).ToList();
                var procedureTimeUpdates = order.Updates.Where(u => u.Type == "Procedure Time Updated" && !existingTimes.Contains(u.Created.TimeOfDay)).ToList();
                procedureTimeUpdates.ForEach(u =>
                    {
                        DoAddItem(new TimelineEventViewModel(u.Created.TimeOfDay, "Procedure Time Update", TimelineItemType.ProcedureTimeUpdated, u.Source));
                    });
            }

            // order assigned
            if (_itemTypes.IsSet(TimelineItemType.OrderAssigned))
            {
            	var update = order.Updates.FirstOrDefault(u => u.Type == "Order Imported");
                if (update != null && update.Created.Date == DateTime.Today && update.Created.TimeOfDay > TimeSpan.FromHours(5))
                {
                    var item = TimelineItems.FirstOrDefault(ti => ti.TimelineType == TimelineItemType.OrderAssigned);
                    if (item == null)
                    {
                        DoAddItem(new TimelineEventViewModel(update.Created.TimeOfDay, "Order Assigned", TimelineItemType.OrderAssigned, false));
                    }
                    else
                    {
                        item.StartTime = update.Created.TimeOfDay;
                    }
                }
            }


            // patient is occupied 
            if (_itemTypes.IsSet(TimelineItemType.PatientOccupied))
            {
                if (order.ProcedureTime.HasValue)
                {
                    var timelineItem = TimelineItems.FirstOrDefault(ti => ti.TimelineType == TimelineItemType.PatientOccupied && ti.Id == order.OrderId) as TimelineVariableDurationEventViewModel;
                    if (timelineItem == null)
                        DoAddItem(new TimelineVariableDurationEventViewModel(order.OrderId, order.ProcedureTime.Value.TimeOfDay, order.Duration, "Patient is Occupied", TimelineItemType.PatientOccupied, null));
                    else
                    {
                        timelineItem.StartTime = order.ProcedureTime.Value.TimeOfDay;
                        timelineItem.Duration = order.Duration;
                    }
                }
            }

            if (_itemTypes.IsSet(TimelineItemType.OutstandingNotification))
            {
                if (order.ProcedureTime.HasValue)
                {
					var notifications = (notificationTypeFilter.HasValue ? order.Notifications.Where(n => n.NotificationType == notificationTypeFilter.Value) : order.Notifications).Select(n => new NotificationItemViewModel(n)).ToList();
					//var notifications = notificationBuilderFunc(order).Select(n => new NotificationItemViewModel(n));
                    MergeAlgorithm<INotificationItem, ITimelineItem>.Merge(notifications.Where(n => n.PriorTime.HasValue && (order.ProcedureTime.Value - n.PriorTime.Value).TimeOfDay < DateTime.Now.TimeOfDay),
                    _timelineItems.Where(a => a.TimelineType == TimelineItemType.OutstandingNotification),
                    (toAdd) => DoAddOutstandingNotification(toAdd, order),
                    (toRemove) => DoRemoveOutstandingNotification(toRemove));

                }
            }

            if (_itemTypes.IsSet(TimelineItemType.PatientArrived))
            {
                // arrival time
                var arrivalUpdate = order.Updates.FirstOrDefault(u => u.Type == "Arrival Time");
                if (arrivalUpdate != null && !TimelineItems.Any(ti => ti.TimelineType == TimelineItemType.PatientArrived))
                {
                    DoAddItem(new TimelineEventViewModel(arrivalUpdate.Created.TimeOfDay, "Patient Arrived", TimelineItemType.PatientArrived, arrivalUpdate.Source));
                }
            }

            if (_itemTypes.IsSet(TimelineItemType.OrderCompleted))
            {

                //completed time
                if (order.CompletedTime.HasValue)
                {
                    var item = TimelineItems.FirstOrDefault(ti => ti.TimelineType == TimelineItemType.OrderCompleted);
                    if (item == null)
                    {
                        DoAddItem(new TimelineEventViewModel(order.CompletedTime.Value.TimeOfDay, "Completed", TimelineItemType.OrderCompleted, true));
                    }
                    else
                    {
                        item.StartTime = order.CompletedTime.Value.TimeOfDay;
                    }
                }

             
            }
 
        }

        /// <summary>
        /// Tries to update the coordinators collections with a potentially different order's details
        /// </summary>
        /// <param name="bed">The bed</param>
        internal void Synchronise(Bed bed)
        {
            // Notes
            if (_itemTypes.IsSet(TimelineItemType.NoteIn | TimelineItemType.NoteOut))
            {
                MergeAlgorithm<Note, ITimelineItem>.Merge(bed.Notes.Where(s => s.DateCreated.Date == DateTime.Today),
                                                          _timelineItems.Where(a => a.TimelineType == TimelineItemType.NoteIn || a.TimelineType == TimelineItemType.NoteOut),
                                                          (toAdd) => DoAddItem(toAdd),
                                                          (toRemove) => DoRemoveItem(toRemove));
            }

            // Services
            if (_itemTypes.IsSet(TimelineItemType.CleaningService))
            {

                MergeAlgorithm<CleaningService, ITimelineItem>.Merge(
                    bed.Services.Where(s => s.CleaningServiceType == BedCleaningEventType.BedCleaned && s.ServiceTime.HasValue && s.ServiceTime.Value.Date == DateTime.Today),
                    _timelineItems.Where(a => a.TimelineType == TimelineItemType.CleaningService),
                    (toAdd) => DoAddItem(toAdd, TimelineItemType.CleaningService),
                    (toRemove) => DoRemoveItem(toRemove));

            }

            // Marking bed as dirty
            if (_itemTypes.IsSet(TimelineItemType.BedMarkedAsDirty))
            {

                MergeAlgorithm<CleaningService, ITimelineItem>.Merge(
                    bed.Services.Where(s => s.CleaningServiceType == BedCleaningEventType.BedMarkedAsDirty && s.ServiceTime.HasValue && s.ServiceTime.Value.Date == DateTime.Today),
                    _timelineItems.Where(a => a.TimelineType == TimelineItemType.BedMarkedAsDirty),
                    (toAdd) => DoAddItem(toAdd, TimelineItemType.BedMarkedAsDirty),
                    (toRemove) => DoRemoveItem(toRemove));

            }

			// discharge
			if (_itemTypes.IsSet(TimelineItemType.Discharge))
			{
				if (bed.EstimatedDischargeDate.HasValue && bed.EstimatedDischargeDate.Value.Date == DateTime.Today && bed.EstimatedDischargeDate.Value.TimeOfDay > TimeSpan.FromHours(5))
				{
					var item = TimelineItems.FirstOrDefault(ti => ti.TimelineType == TimelineItemType.OrderAssigned);
					if (item == null)
					{
						DoAddItem(new TimelineEventViewModel(bed.EstimatedDischargeDate.Value.TimeOfDay, "Estimated Discharge", TimelineItemType.Discharge, false));
					}
					else
					{
						item.StartTime = bed.EstimatedDischargeDate.Value.TimeOfDay;
					}
				}
			}

			Synchronise(bed.AvailableTimes.OfType<ITimeDefinition>().ToList());

        }

        /// <summary>
        /// Tries to update the coordinators collections with a potentially different order's details
        /// </summary>
        /// <param name="bedTimes">The bedTime</param>
        internal void Synchronise(List<ITimeDefinition> bedTimes)
        {
            var mergedTimes = TimeDefinition.MergeTimes(bedTimes);

            // Update Free Rooms
            if (_itemTypes.IsSet(TimelineItemType.FreeRoom))
            {
                MergeAlgorithm<TimeDefinition, ITimelineItem>.Merge(
                        mergedTimes,
                        _timelineItems.Where(a => a.TimelineType == TimelineItemType.FreeRoom),
                        (toAdd) => DoAddAvailableBedTimeItem(toAdd),
                        (toRemove) => DoRemoveItem(toRemove));
            }
        }


        public string Location { get; set; }

        public OrderViewModel ScheduleItemViewModel { get; set; }


        public TimelineItemObservableCollection TimelineItems
        {
            private set
            {
                _timelineItems = value;
                this.DoRaisePropertyChanged(() => TimelineItems, RaisePropertyChanged);
            }
            get { return _timelineItems; }
        }

        public CollectionViewSource TimelineSource
        {
            get { return _timelineSource; }
            set
            {
                _timelineSource = value;
                this.DoRaisePropertyChanged(() => TimelineSource, RaisePropertyChanged);
            }
        }

        public void BringItemIntoFocus(IOrderItem item)
        { }


        /// <summary>
        /// Adds the new item to the head of the stack
        /// </summary>
        /// <param name="item">The item.</param>
        public void AddNewTimelineItem(ITimelineItem item)
        {
            lock ((TimelineItems as ICollection).SyncRoot)
            {
                if (Application.Current != null)
                {
                    Application.Current.Dispatcher.InvokeIfRequired((() => TimelineItems.Add(item)));
                }
                else
                {
                    TimelineItems.Add(item);
                }
            }
        }

        /// <summary>
        /// Clears out all the state
        /// </summary>
        public void Clear()
        {
            if (ScheduleItemViewModel == null)
                throw new InvalidOperationException("ScheduleItemViewModel cannot be null");

            lock ((TimelineItems as ICollection).SyncRoot)
            {
                UnhookTimelineItem(TimelineItems[0]);
            }
        }


        /// <summary>
        /// It fires when the order resets its procedure time so the timeline outstanding notification event can update accordingly
        /// </summary>
        /// <param name="notificationId">The notification id.</param>
        /// <param name="newNotificationStartTime">The new notification start time.</param>
        internal void AbsorbNewNotificationStartTime(int notificationId, TimeSpan newNotificationStartTime)
        {
            lock ((TimelineItems as ICollection).SyncRoot)
            {
                var timelineItems = TimelineItems.OfType<IUpdateableTimelineItem>().ToList();
                var updateableTimelineItems = timelineItems.Where(ti => ti.TimelineType == TimelineItemType.OutstandingNotification && ti.Id == notificationId).ToList();
                updateableTimelineItems.ForEach(ti => ti.AbsorbNewNotificationStartTime(newNotificationStartTime));
            }
        }

        #endregion

        #region Private Members


        /// <summary>
        /// Handles the event when a item wants to be removed from the timeline
        /// </summary>
        private void DoAddItem(Note itemToAdd)
        {
            AddNewTimelineItem(new TimelineNoteViewModel(itemToAdd));
        } 

        private void DoAddItem(CleaningService itemToAdd, TimelineItemType timelineType)
        {
            AddNewTimelineItem(new TimelineServiceViewModel(itemToAdd, timelineType));
        } 

        private void DoAddAvailableBedTimeItem(ITimeDefinition itemToAdd)
        {
            AddNewTimelineItem(new TimelineVariableDurationEventViewModel(itemToAdd.Id, itemToAdd.BeginTime, itemToAdd.Duration, "Bed is Available", TimelineItemType.FreeRoom, null));
        }

        private void DoAddItem(Notification notification)
        {
            AddNewTimelineItem(new TimelineEventViewModel(notification.NotificationId,
                                                          notification.AcknowledgedTime.HasValue
                                                            ? notification.AcknowledgedTime.Value.TimeOfDay
                                                            : DateTime.Now.TimeOfDay, notification.Description,
                                                          TimelineItemType.NotificationAcknowlegement, notification.AcknowledgedBy));
        }

        private void DoAddOutstandingNotification(INotificationItem notification, Order order)
        {
            if (!order.ProcedureTime.HasValue || !notification.PriorTime.HasValue)
                throw new ArgumentException("Order must have a start time");


            var realStartTime = (order.ProcedureTime.Value - notification.PriorTime.Value).TimeOfDay;
            if (realStartTime < TimeSpan.FromHours(5))
                realStartTime = TimeSpan.FromHours(5);

            var endTime = (notification.IsAcknowledged && notification.AcknowledgedTime.HasValue) ? notification.AcknowledgedTime.Value.TimeOfDay : DateTime.Now.TimeOfDay;
            var nvm = new TimelineVariableDurationEventViewModel(notification.Id, realStartTime, endTime - realStartTime, "Notification Overdue", TimelineItemType.OutstandingNotification, notification.IsAcknowledged ? endTime : default(TimeSpan?));

            AddNewTimelineItem(nvm);

        }

        private void DoRemoveOutstandingNotification(ITimelineItem itemToRemove)
        {
            var outstandingNotification = itemToRemove as TimelineVariableDurationEventViewModel;
            if (outstandingNotification == null)
                throw new ArgumentException("itemToRemove must be a TimelineVariableDurationEventViewModel Type");

            outstandingNotification.StopGrowing();
        }

        private void DoAddItem(TimelineEventViewModel timelineEventViewModel)
        {
            if (Application.Current != null)
            {
                Application.Current.Dispatcher.InvokeIfRequired((() =>
                                                                     {
                                                                         TimelineItems.Add(timelineEventViewModel);
                                                                     }));
            }
            else
            {
                TimelineItems.Add(timelineEventViewModel);
            }
        }

        private void DoAddItem(TimelineVariableDurationEventViewModel timelineEventViewModel)
        {
            if (Application.Current != null)
            {
                Application.Current.Dispatcher.InvokeIfRequired(() => TimelineItems.Add(timelineEventViewModel));
            }
            else
            {
                TimelineItems.Add(timelineEventViewModel);
            }
        }


        /// <summary>
        /// Handles the event when a item wants to be removed from the editor
        /// </summary>
        private void DoRemoveItem(ITimelineItem itemToRemove)
        {
            Application.Current.Dispatcher.InvokeIfRequired((() =>
                                                                {
                                                                    HandleRequestRemoveItem(itemToRemove);
                                                                }));
        }

        /// <summary>
        /// Handles the event when a item wants to be removed from the editor
        /// </summary>
        private void HandleRequestRemoveItem(ITimelineItem itemToRemove)
        {
            Application.Current.Dispatcher.InvokeIfRequired((() =>
            {
                TimelineItems.Remove(itemToRemove);
                itemToRemove.Dispose();
            }));
        }



        /// <summary>
        /// Unhooks all the events for the item so it can be released from memory
        /// </summary>
        /// <param name="itemToRemove">The item to remove.</param>
        private void UnhookTimelineItem(ITimelineItem itemToRemove)
        {
            Application.Current.Dispatcher.InvokeIfRequired((() =>
            {
                TimelineItems.Remove(itemToRemove);
                itemToRemove.Dispose();
            }));
        }

        #endregion

        internal void HandleMinuteTimerTick()
        {
            TimelineItems.ForEach(ti => ti.HandleMinuteTimerTick());
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using GalaSoft.MvvmLight;
using WCS.Core;
using WCS.Shared.Controls;
using WCS.Shared.Schedule;

namespace WCS.Shared.Orders
{
	/// <summary>
	/// Coordinates all the interactions between all the of different Order Items 
	/// </summary>
	public class OrderCoordinator : ViewModelBase
	{
		public event Action<Order> OrderUpdateAvailable;

		#region Fields

		private ObservableCollection<IOrderItem> _orderItems;

		#endregion

		#region Public API

		#region Constructors

		public OrderCoordinator()
		{
			AppointmentItems = new ObservableCollection<IOrderItem>();
		}

		#endregion

		#region IDispose

		private bool _disposed = false;

		protected override void Dispose(bool disposing)
		{
			if (!this._disposed)
			{
				if (_orderItems != null)
				{
					_orderItems.OfType<NotificationItemViewModel>().ForEach(ai => ai.OrderUpdateAvailable -= SiganlOrderUpdate);

					_orderItems.ForEach(n =>
					{
						if (n != null)
						{
							n.Dispose();
						}
					});
					Application.Current.Dispatcher.InvokeIfRequired((() =>
					{
						_orderItems.Clear();
						_orderItems = null;
					                                                 	}));
	
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
			// Main items
			_orderItems.ForEach(ai =>
			{
				if (ai.Id == order.OrderId && ai.AppointmentType == OrderScheduleItemType.Order)
					ai.Synchronise(order);
			});

			//var notifications = order.Notifications.Where(n => n.NotificationType == notificationType);
			var notifications = (notificationTypeFilter.HasValue ? order.Notifications.Where(n => n.NotificationType == notificationTypeFilter.Value) : order.Notifications).ToList();
			List<IOrderItem> items;
			if(!notificationTypeFilter.HasValue)
				items = _orderItems.Where(n => n.AppointmentType != OrderScheduleItemType.Order).ToList();
			else if (notificationTypeFilter.Value == NotificationType.Physio)
				items = _orderItems.Where(n => n.AppointmentType == OrderScheduleItemType.PhysioNotification).ToList();
			else if (notificationTypeFilter.Value == NotificationType.Prep)
				items = _orderItems.Where(n => n.AppointmentType == OrderScheduleItemType.Notification).ToList();
			else
				items = _orderItems.Where(n => n.AppointmentType != OrderScheduleItemType.Order).ToList();
			//	  _orderItems.Where(a => a.AppointmentType == OrderScheduleItemType.Notification);
			MergeAlgorithm<Notification, IOrderItem>.Merge(notifications,
			items,
			(toAdd) => DoAddItem(toAdd),
			(toRemove) => DoRemoveItem(toRemove),
			(old, @new) => DoSynchroniseItem(old, @new));
		}

		/// <summary>
		/// Tries to update the coordinators collections with an new detections
		/// </summary>
		/// <param name="detections">The detections</param>
		internal void Synchronise(IList<Detection> detections)
		{
			// Main items
			_orderItems.ForEach(oi =>
			{
				if (oi.AppointmentType == OrderScheduleItemType.Order)
					oi.Synchronise(detections);
			});
		}

		public OrderViewModel ScheduleItemViewModel { get; set; }

		public ObservableCollection<IOrderItem> AppointmentItems
		{
			private set
			{
				_orderItems = value;
				this.DoRaisePropertyChanged(() => AppointmentItems, RaisePropertyChanged);
			}
			get { return _orderItems; }
		}



		public OrderItemViewModel Order
		{
			get
			{
				return (OrderItemViewModel)AppointmentItems.First(ai => ai.AppointmentType == OrderScheduleItemType.Order);
			}
		}

		public void BringItemIntoFocus(IOrderItem item)
		{ }

		/// <summary>
		/// Adds the new item to the head of the stack
		/// </summary>
		/// <param name="item">The item.</param>
		public void AddNewAppointmentItem(IOrderItem item)
		{
			lock ((AppointmentItems as ICollection).SyncRoot)
			{
                if (Application.Current != null)
                {
                    Application.Current.Dispatcher.InvokeIfRequired((() => AppointmentItems.Add(item)));
                }
                else
                {
                    AppointmentItems.Add(item);
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

			lock ((AppointmentItems as ICollection).SyncRoot)
			{
				Application.Current.Dispatcher.InvokeIfRequired((() =>
				{
					UnhookAppointmentItem(AppointmentItems[0]);
				}));
			}

		}

		internal void HandleMinuteTimerTick()
		{
			AppointmentItems.ForEach(o => o.HandleMinuteTimerTick());
		}

		#endregion

		#region Private Members



		private void DoAddItem(Notification notification)
		{

			Application.Current.Dispatcher.InvokeIfRequired((() =>
			{

				var nvm = new NotificationItemViewModel(notification);
				nvm.OrderUpdateAvailable += SiganlOrderUpdate;
				AddNewAppointmentItem(nvm);
			}));
		}


		private void DoRemoveItem(IOrderItem itemToRemove)
		{
			Application.Current.Dispatcher.InvokeIfRequired((() =>
			{
				HandleRequestRemoveItem(itemToRemove);
			}));
		}

		private void DoSynchroniseItem(IOrderItem existingItem, Notification notification)
		{

			existingItem.Synchronise(notification);
		}


		/// <summary>
		/// Handles the event when a item wants to be removed from the editor
		/// </summary>
		private void HandleRequestRemoveItem(IOrderItem itemToRemove)
		{
			UnhookAppointmentItem(itemToRemove);
		}

		/// <summary>
		/// Unhooks all the events for the item so it can be released from memory
		/// </summary>
		/// <param name="itemToRemove">The item to remove.</param>
		private void UnhookAppointmentItem(IOrderItem itemToRemove)
		{
			Application.Current.Dispatcher.InvokeIfRequired((() =>
			{
				AppointmentItems.Remove(itemToRemove);
				itemToRemove.Dispose();
			}));
		}

		/// <summary>
		/// Callback from when the user updates the order. Need to signal anybody who's interested in this
		/// </summary>
		/// <param name="order">The order.</param>
		private void SiganlOrderUpdate(Order order)
		{
			var ora = OrderUpdateAvailable;
			if (ora != null)
			{
				ora.Invoke(order);
			}
		}
		#endregion
	}
}

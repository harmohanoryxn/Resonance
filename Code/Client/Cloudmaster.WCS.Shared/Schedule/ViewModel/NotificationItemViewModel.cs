using System;
using System.Collections.Generic;
using System.Diagnostics;
using Cloudmaster.WCS.DataServicesInvoker;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Threading;
using WCS.Core;
using WCS.Shared.Orders;

namespace WCS.Shared.Schedule
{
	public class NotificationItemViewModel : ViewModelBase, INotificationItem
	{
		public event Action<Order> OrderUpdateAvailable;

		private Notification _notification;

		private TimeSpan? _startTime;

		private bool _canAcknowledge;

		public NotificationItemViewModel(Notification notification)
		{
			Notification = notification;

			Duration = notification.Duration;
		}

		#region IOrderItem Members

		public int Id
		{
			get { return Notification.NotificationId; }
		}


		public OrderScheduleItemType AppointmentType
		{
			get
			{
				switch (Notification.NotificationType)
				{
					case NotificationType.Physio:
						return OrderScheduleItemType.PhysioNotification;
					case NotificationType.Prep:
						return OrderScheduleItemType.Notification;
					default:
						throw new InvalidOperationException("Notification.NotificationType");
				}
			}
		}

		public TimeSpan? StartTime
		{
			get { return _startTime; }
			set
			{
				_startTime = value;
				this.DoRaisePropertyChanged(() => StartTime, RaisePropertyChanged);
				this.DoRaisePropertyChanged(() => RequiresAcknowledgement, RaisePropertyChanged);
			}
		}


		public TimeSpan? PriorTime
		{
			get { return _notification.PriorToProcedureTime; }
		}

		public TimeSpan Duration
		{
			get { return _notification.Duration; }
			private set
			{
				_notification.Duration = value;
				this.DoRaisePropertyChanged(() => Duration, RaisePropertyChanged);
			}
		}

		public void Synchronise(Order order)
		{
			throw new NotImplementedException();
		}

		public void Synchronise(IList<Detection> detections)
		{
			throw new NotImplementedException();
		}

		public void Synchronise(Notification notification)
		{
			_notification = notification;

			this.DoRaisePropertyChanged(() => StartTime, RaisePropertyChanged);
			this.DoRaisePropertyChanged(() => Duration, RaisePropertyChanged);
			this.DoRaisePropertyChanged(() => RequiresAcknowledgement, RaisePropertyChanged);
			this.DoRaisePropertyChanged(() => CanAcknowledge, RaisePropertyChanged);
			this.DoRaisePropertyChanged(() => IsAcknowledged, RaisePropertyChanged);
		}


		public void HandleMinuteTimerTick()
		{
			this.DoRaisePropertyChanged(() => StartTime, RaisePropertyChanged);
		}

		public bool Equals(IOrderItem other)
		{
			if (other == null) return false;
			return (this.Id.Equals(other.Id) && this.AppointmentType.Equals(other.AppointmentType));
		}

		public override int GetHashCode()
		{
			return Id ^ (int)AppointmentType;
		}

		public int GetFingerprint()
		{
			return _notification.GetFingerprint();
		}
		#endregion

		#region IDispose

		private bool _disposed = false;

		public new void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this._disposed)
			{
				_disposed = true;

			}
		}
		#endregion

		public Notification Notification
		{
			get { return _notification; }
			private set { _notification = value; }
		}

		public string Description
		{
			get { return _notification.Description; }
			//private set
			//{
			//    _order.LastUpdated = value;
			//    this.DoRaisePropertyChanged(() => LastUpdated, RaisePropertyChanged);
			//}
		}

		public NotificationType NotificationType
		{
			get { return _notification.NotificationType; }
		}

		public DateTime? AcknowledgedTime
		{
			get { return _notification.AcknowledgedTime; }
		}

		public bool CanAcknowledge
		{
			get { return _canAcknowledge; }
			set
			{
				_canAcknowledge = value;
				this.DoRaisePropertyChanged(() => CanAcknowledge, RaisePropertyChanged);
			}
		}

		public bool IsAcknowledged
		{
			get { return _notification.Acknowledged; }
			set
			{
				_notification.Acknowledged = value;
				this.DoRaisePropertyChanged(() => IsAcknowledged, RaisePropertyChanged);
				this.DoRaisePropertyChanged(() => RequiresAcknowledgement, RaisePropertyChanged);
			}
		}

		public bool RequiresAcknowledgement
		{
			get { return _notification.RequiresAcknowledgement && !IsAcknowledged && StartTime < DateTime.Now.TimeOfDay; }
			set
			{
				_notification.RequiresAcknowledgement = value;
				this.DoRaisePropertyChanged(() => RequiresAcknowledgement, RaisePropertyChanged);
			}
		}

		public bool DoesRequireAcknowledgement(DateTime? dateTimeToUse)
		{
			return !IsAcknowledged && DateTime.Now.TimeOfDay > StartTime;
		}

		public RelayCommand ToggleAllowAcknowledgeCommand
		{
			get { return new RelayCommand(DoToggleAllowAcknowledgeCommand); }
		}

		public RelayCommand AcknowledgeCommand
		{
			get { return new RelayCommand(DoAcknowledgeCommand); }
		}

		/// <summary>
		/// Fires when the order resets its procedure time so the notification can update accordingly
		/// </summary>
		/// <param name="newProcedureTime">The new procedure time.</param>
		public void AbsorbNewOrderProcedureTime(DateTime newProcedureTime)
		{
			StartTime = newProcedureTime.TimeOfDay - PriorTime;
			//	Duration = (StartTime.Value < TimeSpan.FromHours(4)) ? newProcedureTime.TimeOfDay - TimeSpan.FromHours(4) : newProcedureTime.TimeOfDay - StartTime.Value;
		}

		private void DoAcknowledgeCommand()
		{
			IsAcknowledged = true;
			CanAcknowledge = false;

			var invoker = WcsViewModel.MefContainer.GetExportedValue<IWcsAsyncInvoker>();
			if (invoker != null)
				invoker.AcknowledgeNotificationAsync(Notification.NotificationId, SynchroniseOrder);
		}
		private void DoToggleAllowAcknowledgeCommand()
		{
			CanAcknowledge = !CanAcknowledge;
		}

		/// <summary>
		/// Callback from when the user updates the order. Need to signal anybody who's interested in this
		/// </summary>
		/// <param name="order">The order.</param>
		private void SynchroniseOrder(Order order)
		{
			var ora = OrderUpdateAvailable;
			if (ora != null)
			{
				ora.Invoke(order);
			}
		}


	}
}

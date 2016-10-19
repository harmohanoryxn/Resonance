using System;
using Cloudmaster.WCS.DataServicesInvoker;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using WCS.Shared;
using WCS.Core;
using WCS.Shared.Schedule;
using WCS.Shared.Timeline;
using System.Threading;

namespace WCS.Shared.Timeline
{
	/// <summary>
	/// A timeline view model that is stretchy and grows every second
	/// </summary>
    class TimelineVariableDurationEventViewModel : ViewModelBase, IUpdateableTimelineItem
	{
		private bool _isSelected;
		private readonly int _id;
		private readonly string _label;
		private TimeSpan _eventTime;
		private TimeSpan? _stopGrowTime;
		private TimeSpan _duration;
		private readonly TimelineItemType _type;

		public TimelineVariableDurationEventViewModel(int id, TimeSpan eventTime, TimeSpan duration, string label, TimelineItemType type, TimeSpan? stopGrowTime)
		{
			_id = id;
			_eventTime = eventTime;
			_duration = duration;
			_label = label;
			_type = type;
			_stopGrowTime = stopGrowTime;
		}

		#region ITimelineMembers

		public event Action<ITimelineItem> ShowItem;

		public int Id
		{
			get { return _id; }
		}

        public TimeSpan? StartTime
        {
            get { return _eventTime; }
            set
            {
                _eventTime = value.Value;
                this.DoRaisePropertyChanged(() => StartTime, RaisePropertyChanged);
                this.DoRaisePropertyChanged(() => EndTime, RaisePropertyChanged);
            }
        }

         
		public TimelineItemType TimelineType
		{
			get { return _type; }
		}

		public string Label
		{
			get { return _label; }
		}

		public bool IsSelected
		{
			get { return _isSelected; }
			set
			{
				_isSelected = value;
				this.DoRaisePropertyChanged(() => IsSelected, RaisePropertyChanged);
			}
		}

		public RelayCommand ShowItemCommand
		{
			get { return new RelayCommand(DoShowItem); }
		}

		public TimeSpan Duration
		{
			get { return _duration; }
			set
			{
				_duration = value;
				this.DoRaisePropertyChanged(() => Duration, RaisePropertyChanged);
				this.DoRaisePropertyChanged(() => EndTime, RaisePropertyChanged);
			}
		}

		public TimeSpan BeginTime
		{
			get { return _eventTime; }
		}

		public void HandleMinuteTimerTick()
		{
			if (!_stopGrowTime.HasValue)
			{
				Duration = Duration.Add(TimeSpan.FromMinutes(1));
			}
		}

		public void Synchronise(ITimelineItem item)
		{
		}

		public bool Equals(ITimelineItem other)
		{
			if (other == null) return false;
			return (this.Id.Equals(other.Id) && this.TimelineType.Equals(other.TimelineType));
		}

		public override int GetHashCode()
		{
			return Id ^ (int)TimelineType;
		}

		public int GetFingerprint()
		{
			return _id ^ _eventTime.GetHashCode() ^ _duration.GetHashCode() ^ _label.GetHashCode() ^ _type.GetHashCode();
		}

		public void AbsorbNewNotificationStartTime(TimeSpan newNotificationStartTime)
		{
			if (newNotificationStartTime < TimeSpan.FromHours(5))
				newNotificationStartTime = TimeSpan.FromHours(5);

			StartTime = newNotificationStartTime;
			if (!_stopGrowTime.HasValue)
			{
				Duration = (DateTime.Now.TimeOfDay < StartTime.Value)
				           	? TimeSpan.FromMinutes(0)
				           	: DateTime.Now.TimeOfDay - StartTime.Value;
			}
		}

		public bool Equals(IUpdateableTimelineItem other)
		{
			if (other == null) return false;
			return (this.Id.Equals(other.Id) && this.TimelineType.Equals(other.TimelineType));
		}

		public int CompareTo(ITimeDefinition other)
		{
			return BeginTime.CompareTo(other.BeginTime);
		}

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
		
        #endregion

		public TimeSpan EndTime
		{
			get { return _eventTime.Add(_duration); }
		}

		private void DoShowItem()
		{
			var si = ShowItem;
			if (si != null)
				ShowItem.Invoke(this);
		}

		/// <summary>
		/// Stops the the notification growing
		/// </summary>
		internal void StopGrowing()
		{
			_stopGrowTime = DateTime.Now.TimeOfDay;
		}
	}
}

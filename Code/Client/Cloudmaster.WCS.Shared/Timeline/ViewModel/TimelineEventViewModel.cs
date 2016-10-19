using System;
using Cloudmaster.WCS.DataServicesInvoker;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using WCS.Core;
using WCS.Core.Composition;
using WCS.Shared.Schedule;

namespace WCS.Shared.Timeline
{
	public class TimelineEventViewModel : ViewModelBase, ITimelineItem
	{
		private bool _isSelected;
		private readonly int _id;
		private readonly string _label;
		private TimeSpan _eventTime;
		private readonly TimelineItemType _type;
		private string _currentDevice;
		private readonly string _source;

 
		public TimelineEventViewModel(int id, TimeSpan eventTime, string label, TimelineItemType type, string source)
		{
			_id = id;
			_eventTime = eventTime;
			_label = label;
			_type = type;
			_source = source;

			_currentDevice = new DefaultDeviceIdentity().DeviceName;
		}

		public TimelineEventViewModel(TimeSpan eventTime, string label, TimelineItemType type, bool _isNative)
		{
			_id = 1;
			_eventTime = eventTime;
			_label = label;
			_type = type;

			_currentDevice = new DefaultDeviceIdentity().DeviceName;

            
            _source = _isNative?_currentDevice:"";
		}

		public TimelineEventViewModel(TimeSpan eventTime, string label, TimelineItemType type, string source)
		{
			_id = 1;
			_eventTime = eventTime;
			_label = label;
			_type = type;
			_source = source;

			_currentDevice = new DefaultDeviceIdentity().DeviceName;
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
				if (value.HasValue)
				{
					_eventTime = value.Value;
					this.DoRaisePropertyChanged(() => IsSelected, RaisePropertyChanged);
				}
			}
		}
		public TimeSpan BeginTime
		{
			get { return _eventTime; }
		}

		public TimeSpan Duration
		{
			get { return TimeSpan.FromMinutes(1); }
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

		public void Synchronise(ITimelineItem item)
		{
		}

		public void HandleMinuteTimerTick()
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
			return _id ^ _eventTime.GetHashCode() ^ _label.GetHashCode() ^ _type.GetHashCode() ^ _source.GetHashCode();
		}

		public int CompareTo(ITimeDefinition other)
		{
			return Id.CompareTo(other.Id);
		}

		#endregion


		public string Source
		{
			get { return _source; }
		}
		public bool IsNativeItem
		{
			get { return !string.IsNullOrEmpty(_source) && _source == _currentDevice; }
		}
		public string Time
		{
			get { return (new DateTime(StartTime.Value.Ticks)).ToNoteTimeFormat(); }
		}

		private void DoShowItem()
		{
			var si = ShowItem;
			if (si != null)
				ShowItem.Invoke(this);
		}
	}
}

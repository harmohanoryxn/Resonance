using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using WCS.Core;
using WCS.Shared.Schedule;

namespace WCS.Shared.Location
{
	public class MovementViewModel : ViewModelBase, ITimelineItem, IComparable<MovementViewModel>
	{
		private bool _isSelected;
		private readonly int _id;
		private readonly string _label;
		private TimeSpan _startTime;
		private TimeSpan? _stopGrowTime;
		private TimeSpan _duration;

		public MovementViewModel(int id, TimeSpan startTime, TimeSpan duration, string label, TimeSpan? stopGrowTime)
		{
			_id = id;
			_startTime = startTime;
			_duration = duration;
			_label = label;
			_stopGrowTime = stopGrowTime;
		}

		#region IUpdateableTimelineItem

		public event Action<ITimelineItem> ShowItem;

		public int Id
		{
			get { return _id; }
		}

		public TimeSpan? StartTime
		{
			get { return _startTime; }
			set
			{

			}
		}
		public TimeSpan BeginTime
		{
			get { return _startTime; }
		}

		public TimeSpan Duration
		{
			get { return _duration; }
			set
			{
				_duration = value;
				this.DoRaisePropertyChanged(() => Duration, RaisePropertyChanged);
			}
		} 

		public TimelineItemType TimelineType
		{
			get { return TimelineItemType.RdifDetection; }
		}

		public string Label
		{
			get { return _label; }
		}

		public string Location
		{
			get { return _label; }
		}

		public RelayCommand ShowItemCommand
		{
			get { return new RelayCommand(DoShowItem); }
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

		public void Synchronise(ITimelineItem item)
		{
		}

		public void HandleMinuteTimerTick()
		{
			if (!_stopGrowTime.HasValue)
			{
				Duration = Duration.Add(TimeSpan.FromMinutes(1));
			}
		}

		public bool Equals(ITimelineItem other)
		{
			if (other == null) return false;
			return (this.Id.Equals(other.Id) && this.TimelineType.Equals(other.TimelineType));
		}


		public int GetFingerprint()
		{
			return _id ^ _startTime.GetHashCode() ^   _label.GetHashCode();
		}

		public int CompareTo(ITimeDefinition other)
		{
			return Id.CompareTo(other.Id);
		}

		public override int GetHashCode()
		{
			return Id ^ (int)TimelineType;
		}

		#endregion
		 

		private void DoShowItem()
		{
			var si = ShowItem;
			if (si != null)
				ShowItem.Invoke(this);
		}

		public int CompareTo(MovementViewModel other)
		{
			return BeginTime.CompareTo(BeginTime);
		}

		 
		public bool Equals(IUpdateableTimelineItem other)
		{
			if (other == null) return false;
			return (this.Id.Equals(other.Id) && this.TimelineType.Equals(other.TimelineType));
		}
	}
}

using System; 
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using WCS.Core;
using WCS.Core.Composition;
using WCS.Shared.Schedule;

namespace WCS.Shared.Location
{
	public class DetectionViewModel : ViewModelBase,  ITimelineItem , IComparable<DetectionViewModel>
	{
		private bool _isSelected;
		private Detection _detection;
		private TimeSpan? _stopGrowTime;
		private string _currentDevice;

		public DetectionViewModel(Detection detection, TimeSpan? stopGrowTime)
		{
			_detection = detection;
			_duration = TimeSpan.FromMinutes(1);
			_stopGrowTime = stopGrowTime;
			_currentDevice = new DefaultDeviceIdentity().DeviceName;
		}

		#region IUpdateableTimelineItem

		public event Action<ITimelineItem> ShowItem;

		public int Id
		{
			get { return _detection.DetectionId; }
		}

		public TimeSpan? StartTime
		{
			get { return _detection.Timestamp.TimeOfDay; }
			set
			{

			}
		}
		public TimeSpan BeginTime
		{
			get { return _detection.Timestamp.TimeOfDay; }
		}

		private TimeSpan _duration;
		public TimeSpan Duration
		{
			get { return _duration; }
			set
			{
				_duration = value;
				this.DoRaisePropertyChanged(() => Duration, RaisePropertyChanged);
			}
		}

		public DetectionDirection Direction
		{
			get { return _detection.Direction; }
		}

		public TimelineItemType TimelineType
		{
			get { return TimelineItemType.RdifDetection; }
		}

		public string Label
		{
			get { return string.Format("{0} {1}", _detection.Direction == DetectionDirection.In ? "Entered" : "Left", _detection.DetectionLocation.LocationName); }
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
			return _detection.GetFingerprint();
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

		public string Location
		{
			get { return _detection.DetectionLocation.LocationName; }
			set { }
		}

        public string Code
        {
            get { return _detection.DetectionLocation.LocationCode; }
            set { }
        }

		private void DoShowItem()
		{
			var si = ShowItem;
			if (si != null)
				ShowItem.Invoke(this);
		}

		public int CompareTo(DetectionViewModel other)
		{
			return BeginTime.CompareTo(BeginTime);
		}

		public bool IsNativeItem
		{
			get { return !string.IsNullOrEmpty(Location) && Location == _currentDevice; }
		}
	 

		public bool Equals(IUpdateableTimelineItem other)
		{
			if (other == null) return false;
			return (this.Id.Equals(other.Id) && this.TimelineType.Equals(other.TimelineType));
		}
	}
}

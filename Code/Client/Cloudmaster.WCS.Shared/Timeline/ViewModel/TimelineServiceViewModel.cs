using System;
using Cloudmaster.WCS.DataServicesInvoker;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using WCS.Core;
using WCS.Core.Composition;
using WCS.Shared;
using WCS.Shared.Schedule;
using WCS.Shared.Timeline;

namespace WCS.Shared.Timeline
{
	class TimelineServiceViewModel : ViewModelBase, ITimelineItem
	{
		private bool _isSelected;
		private CleaningService _service;
		private TimelineItemType _timelineType;
		private string _currentDevice;

		public TimelineServiceViewModel(CleaningService service, TimelineItemType timelineType)
		{
			Service = service;
			_timelineType = timelineType;
			_currentDevice = new DefaultDeviceIdentity().DeviceName;
		}

		#region ITimelineMembers

		public event Action<ITimelineItem> ShowItem;

		public int Id
		{
			get { return Service.CleaningServiceId; }
		}

		public TimeSpan? StartTime
		{
			get { return Service.ServiceTime.Value.TimeOfDay; }
			set
			{ }
		}

		public TimelineItemType TimelineType
		{
			get { return _timelineType; }
		}

		public string Label
		{
			get
			{
				switch (_timelineType)
				{
					case TimelineItemType.BedMarkedAsDirty:
				return "Cleaned";
					case TimelineItemType.CleaningService:
				return "Requires Attention";
					default:
				return "Unknown";
				}
			}
		}

		public TimeSpan BeginTime
		{
			get { return Service.ServiceTime.Value.TimeOfDay; }
		}

		public TimeSpan Duration
		{
			get { return TimeSpan.FromMinutes(1); }
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
			if (item is TimelineServiceViewModel)
			{
				Service = (item as TimelineServiceViewModel).Service;
			}
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
			return _service.GetFingerprint();
		}


		public int CompareTo(ITimeDefinition other)
		{
			return Id.CompareTo(other.Id);
		}

		#endregion


		public DateTime? ServiceTime
		{
			get { return Service.ServiceTime.Value; }
		}

		public CleaningService Service
		{
			get { return _service; }
			private set
			{
				_service = value;
				this.DoRaisePropertyChanged(() => Label, RaisePropertyChanged);
				//this.DoRaisePropertyChanged(() => HasContactPrecaution, RaisePropertyChanged);
				//this.DoRaisePropertyChanged(() => HasDropletPrecaution, RaisePropertyChanged);
				//this.DoRaisePropertyChanged(() => HasAirbornePrecaution, RaisePropertyChanged);
				//this.DoRaisePropertyChanged(() => HasIsolationPrecaution, RaisePropertyChanged);
				this.DoRaisePropertyChanged(() => StartTime, RaisePropertyChanged);
			}
		}
		//public bool HasContactPrecaution
		//    {
		//        get
		//        {
		//            return _service.Warnings == BedCaution.Contact_precautions;
		//        }
		//    }

		//    public bool HasDropletPrecaution
		//    {
		//        get
		//        {
		//            return _service.Warnings == BedCaution.Contact_precautions;
		//        }
		//    }

		//    public bool HasAirbornePrecaution
		//    {
		//        get
		//        {
		//            return _service.Warnings == BedCaution.Contact_precautions;
		//        }
		//    }

		//    public bool HasIsolationPrecaution
		//    {
		//        get
		//        {
		//            return _service.Warnings == BedCaution.Contact_precautions;
		//        }
		//    }



		public string Source
		{
			get { return _currentDevice; }
		}
		public bool IsNativeItem
		{
			get { return _timelineType==TimelineItemType.CleaningService; }
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

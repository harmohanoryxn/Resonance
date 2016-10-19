using System;
using Cloudmaster.WCS.DataServicesInvoker;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using WCS.Core;
using WCS.Shared.Schedule;

namespace WCS.Shared.Timeline
{
	public class TimelineRequiresCleaningViewModel : ViewModelBase, ITimelineItem
	{
	private bool _isSelected;
		private readonly Update _update;

		public TimelineRequiresCleaningViewModel(Update update)
		{
			_update = update;
		}

		#region ITimelineMembers

		public event Action<ITimelineItem> ShowItem;

		public int Id
		{
			get { return _update.Id; }
		}

		public TimeSpan? StartTime
		{
			get { return _update.Created.TimeOfDay; }
			set
			{}
		}

		public TimeSpan BeginTime
		{
			get { return _update.Created.TimeOfDay; }
		}

		public TimeSpan Duration
		{
			get { return TimeSpan.FromMinutes(1); }
		}

		public TimelineItemType TimelineType
		{
			get { return TimelineItemType.BedMarkedAsDirty; }
		}

		public string Label
		{
			get { return _update.Value; }
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
			return _update.GetFingerprint();
		} 
		public int CompareTo(ITimeDefinition other)
		{
			return Id.CompareTo(other.Id);
		}

		#endregion


		private void DoShowItem()
		{
			var si = ShowItem;
			if (si != null)
				ShowItem.Invoke(this);
		}
	}
}

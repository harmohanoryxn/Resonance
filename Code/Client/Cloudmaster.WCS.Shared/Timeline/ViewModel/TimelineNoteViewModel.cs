using System;
using Cloudmaster.WCS.DataServicesInvoker;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using WCS.Core;
using WCS.Core.Composition;
using WCS.Shared.Schedule;

namespace WCS.Shared.Timeline
{
	public class TimelineNoteViewModel : ViewModelBase, ITimelineItem 
	{
		private bool _isSelected;
		private Note _note;
		private string _currentDevice;
		 
		public TimelineNoteViewModel(Note note)
		{
			Note = note;
			_currentDevice = new DefaultDeviceIdentity().DeviceName;
		}

		#region ITimelineMembers

		public event Action<ITimelineItem> ShowItem;

		public int Id
		{
			get { return Note.NoteId; }
		}

		public TimeSpan? StartTime
		{
			get { return Note.DateCreated.TimeOfDay; }
			set
			{
				}
		}
		public TimeSpan BeginTime
		{
			get { return Note.DateCreated.TimeOfDay; }
		}

		public TimeSpan Duration
		{
			get { return TimeSpan.FromMinutes(1); }
		}

		public TimelineItemType TimelineType
		{
			get { return Note.Source == _currentDevice ? TimelineItemType.NoteOut : TimelineItemType.NoteIn; }
		}

		public string Label
		{
			get { return Note.NoteMessage; }
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
			if (item is TimelineNoteViewModel)
			{
				Note = (item as TimelineNoteViewModel).Note;
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
			return _note.GetFingerprint();
		}

		public int CompareTo(ITimeDefinition other)
		{
			return Id.CompareTo(other.Id);
		}

		#endregion

		public Note Note
		{
			get { return _note; }
			private set
			{
				_note = value;
				this.DoRaisePropertyChanged(() => Label, RaisePropertyChanged);
				this.DoRaisePropertyChanged(() => Source, RaisePropertyChanged);
				this.DoRaisePropertyChanged(() => StartTime, RaisePropertyChanged);
			}
		}

		public string Source
		{
			get { return Note.Source; }
		}


		public string Time
		{
			get { return Note.DateCreated.ToNoteTimeFormat(); }
		}
		public bool IsNativeItem
		{
			get { return Note.Source == _currentDevice; }
		}
		 
		private void DoShowItem()
		{
			var si = ShowItem;
			if(si != null)
				ShowItem.Invoke(this);
		}
	}
}

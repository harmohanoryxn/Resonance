using System;
using Cloudmaster.WCS.DataServicesInvoker;
using GalaSoft.MvvmLight.Command;
using WCS.Core;

namespace WCS.Shared.Schedule
{
	public interface ITimelineItem : IEquatable<ITimelineItem>, ITimeDefinition, ISynchroniseable<ITimelineItem>
	{
		event Action<ITimelineItem> ShowItem;

		bool IsSelected { get; set; }
		TimeSpan? StartTime { get; set; }	// This is to be replaced by ITimeDefinition.BeginTime when this property becomes not nullable

		TimelineItemType TimelineType { get; }
		RelayCommand ShowItemCommand { get; }
		string Label { get; }

		void HandleMinuteTimerTick();
	}
}

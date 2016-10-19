using System.Windows;
using System.Windows.Controls;
using WCS.Shared.Timeline;

namespace WCS.Shared.Schedule
{
	/// <summary>
	/// Used to pick different templates for the different things that can appear inside the timeline
	/// </summary>
    public class TimelineDataTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var element = container as FrameworkElement;

            if (item is TimelineEventViewModel)
                return element.FindResource("DefaultTimelineDataTemplate") as DataTemplate;

			else if (item is TimelineNoteViewModel)
				return element.FindResource("NoteTimelineDataTemplate") as DataTemplate;


            return null;
        }
    }
}

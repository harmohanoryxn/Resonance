using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WCS.Shared.Schedule
{
	public class ScheduleItemCanvas : Canvas
    {
		public ScheduleItemCanvas()
        {
            Background = Brushes.Transparent;
        }

        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            var contentPresenter = visualAdded as ContentPresenter;
            if (contentPresenter != null)
            {
                AddNewItem(contentPresenter);
            }
            contentPresenter = visualRemoved as ContentPresenter; 
            if (contentPresenter != null)
            {
                RemoveNewItem(contentPresenter);
            }
        }

        /// <summary>
        /// Adds a new item to the collection
        /// </summary>
        /// <param name="contentPresenter">The content presenter that represents the new item in the collection</param>
        private void AddNewItem(ContentPresenter contentPresenter)
        {
            var item = contentPresenter.DataContext as IOrderItem;
            if (item != null)
            {
                contentPresenter.Name = string.Format("item_{0}", VisualChildrenCount - 1);
			}
            base.OnVisualChildrenChanged(contentPresenter, null);
      
        }

        /// <summary>
        /// Removes an item from the list
        /// </summary>
        /// <param name="contentPresenter">The content presenter that represents the item in the collection that needs removing</param>
        private void RemoveNewItem(ContentPresenter contentPresenter)
        {
            var item = contentPresenter.DataContext as IOrderItem;
            if (item != null)
            {
			          base.OnVisualChildrenChanged(null, contentPresenter);
            }
        }
    }
}
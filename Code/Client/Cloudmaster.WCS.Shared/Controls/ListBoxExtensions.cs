using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;
using WCS.Shared.Beds;
using WCS.Shared.Cleaning.Schedule;
using WCS.Shared.Location;
using WCS.Shared.Notes;
using WCS.Shared.Orders;
using WCS.Shared.Physio.Schedule;
using WCS.Shared.Schedule;

namespace WCS.Shared.Controls
{
	/// <summary>
	/// Class implementing helpful extensions to ListBox.
	/// </summary>
	public static class ListBoxExtensions
	{
		/// <summary>
		/// Causes the object to scroll to the top of the listbox's scrollviewer's viewport
		/// </summary>
		/// <param name="listBox">ListBox instance.</param>
		/// <param name="item">Object to scroll.</param>
		[SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
			Justification = "Deliberately targeting ListBox.")]
		public static void ScrollIntoViewCentered(this ListBox listBox, object item)
		{
			Debug.Assert(!VirtualizingStackPanel.GetIsVirtualizing(listBox),
			             "VirtualizingStackPanel.IsVirtualizing must be disabled for ScrollIntoViewCentered to work.");

			// Get the container for the specified item
			var container = listBox.ItemContainerGenerator.ContainerFromItem(item) as FrameworkElement;
			if (null != container)
			{
				// Get the bounds of the item container
				var rect = new Rect(new Point(), new Size(container.RenderSize.Width, 1075));

				// Find constraining parent (either the nearest ScrollContentPresenter or the ListBox itself)
				FrameworkElement constrainingParent = container;
				do
				{
					constrainingParent = VisualTreeHelper.GetParent(constrainingParent) as FrameworkElement;
				} while ((null != constrainingParent) &&
				         (listBox != constrainingParent) &&
				         !(constrainingParent is ScrollContentPresenter));
				do
				{
					constrainingParent = VisualTreeHelper.GetParent(constrainingParent) as FrameworkElement;
				} while ((null != constrainingParent) &&
				         (listBox != constrainingParent) &&
				         !(constrainingParent is ScrollContentPresenter));

				if (null != constrainingParent)
				{
					// Inflate rect to fill the constraining parent
					rect.Inflate(
						Math.Max((constrainingParent.ActualWidth - rect.Width)/2, 0),
						Math.Max((constrainingParent.ActualHeight - rect.Height)/2, 0));
					constrainingParent.BringIntoView(rect);
				}

				// Bring the (inflated) bounds into view

				//if (ScrollViewer.GetCanContentScroll(listBox))
				//{
				//    // Get the parent IScrollInfo
				//    var scrollInfo = VisualTreeHelper.GetParent(container) as IScrollInfo;
				//    if (null != scrollInfo)
				//    {
				//        // Need to know orientation, so parent must be a known type
				//        var stackPanel = scrollInfo as StackPanel;
				//        var virtualizingStackPanel = scrollInfo as VirtualizingStackPanel;
				//        Debug.Assert((null != stackPanel) || (null != virtualizingStackPanel),
				//            "ItemsPanel must be a StackPanel or VirtualizingStackPanel for ScrollIntoViewCentered to work.");

				//        // Get the container's index
				//        var index = listBox.ItemContainerGenerator.IndexFromContainer(container);

				//        // Center the item by splitting the extra space
				//        if (((null != stackPanel) && (Orientation.Horizontal == stackPanel.Orientation)) ||
				//            ((null != virtualizingStackPanel) && (Orientation.Horizontal == virtualizingStackPanel.Orientation)))
				//        {
				//            scrollInfo.SetHorizontalOffset(index - Math.Floor(scrollInfo.ViewportWidth / 2));
				//        }
				//        else
				//        {
				//            scrollInfo.SetVerticalOffset(index - Math.Floor(scrollInfo.ViewportHeight / 2));
				//        }
				//    }
				//}
				//else
				//{
				//    // Get the bounds of the item container
				//    var rect = new Rect(new Point(), container.RenderSize);

				//    // Find constraining parent (either the nearest ScrollContentPresenter or the ListBox itself)
				//    FrameworkElement constrainingParent = container;
				//    do
				//    {
				//        constrainingParent = VisualTreeHelper.GetParent(constrainingParent) as FrameworkElement;
				//    } while ((null != constrainingParent) &&
				//             (listBox != constrainingParent) &&
				//             !(constrainingParent is ScrollContentPresenter));

				//    if (null != constrainingParent)
				//    {
				//        // Inflate rect to fill the constraining parent
				//        rect.Inflate(
				//            Math.Max((constrainingParent.ActualWidth - rect.Width) / 2, 0),
				//            Math.Max((constrainingParent.ActualHeight - rect.Height) / 2, 0));
				//    }

				//    // Bring the (inflated) bounds into view
				//    container.BringIntoView(rect);
				//}
			}
		}

		public static void ScrollToTopCurrentItem(this ListBox listBox, object item)
		{
            try
            {
                // Get the container for the specified item
                var listBoxItem = listBox.ItemContainerGenerator.ContainerFromItem(item) as FrameworkElement;
                if (null != listBoxItem)
                {
                    var scrollViewer = WpfHelper.TryFindChild<ScrollViewer>(listBox);
                    var baselingOffset = scrollViewer.PointToScreen(new Point()).Y;
                    var currentContentOffset = scrollViewer.ContentVerticalOffset;
                    var listboxOffset = listBoxItem.PointToScreen(new Point()).Y;
                    var newOffset = currentContentOffset + listboxOffset - baselingOffset;
                    scrollViewer.ScrollToVerticalOffset(newOffset);
                }
            }
            catch (Exception)
            {
            }
		}

	  
	}
}

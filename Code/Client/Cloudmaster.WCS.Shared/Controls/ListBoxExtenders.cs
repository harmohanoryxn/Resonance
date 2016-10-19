using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace WCS.Shared.Controls
{ 
	public class ListBoxExtenders : DependencyObject
	{
		#region AutoScrollToCurrentItem
		#region Properties

		public static readonly DependencyProperty AutoScrollToCurrentItemProperty = DependencyProperty.RegisterAttached("AutoScrollToCurrentItem", typeof(bool), typeof(ListBoxExtenders), new UIPropertyMetadata(default(bool), OnAutoScrollToCurrentItemChanged));

		/// <summary>
		/// Returns the value of the AutoScrollToCurrentItemProperty
		/// </summary>
		/// <param name="obj">The dependency-object whichs value should be returned</param>
		/// <returns>The value of the given property</returns>
		public static bool GetAutoScrollToCurrentItem(DependencyObject obj)
		{
			return (bool)obj.GetValue(AutoScrollToCurrentItemProperty);
		}

		/// <summary>
		/// Sets the value of the AutoScrollToCurrentItemProperty
		/// </summary>
		/// <param name="obj">The dependency-object whichs value should be set</param>
		/// <param name="value">The value which should be assigned to the AutoScrollToCurrentItemProperty</param>
		public static void SetAutoScrollToCurrentItem(DependencyObject obj, bool value)
		{
			obj.SetValue(AutoScrollToCurrentItemProperty, value);
		}

		#endregion

		#region Events

		/// <summary>
		/// This method will be called when the AutoScrollToCurrentItem
		/// property was changed
		/// </summary>
		/// <param name="s">The sender (the ListBox)</param>
		/// <param name="e">Some additional information</param>
		public static void OnAutoScrollToCurrentItemChanged(DependencyObject s, DependencyPropertyChangedEventArgs e)
		{
			var listBox = s as ListBox;
			if (listBox != null)
			{
				var listBoxItems = listBox.Items;
				if (listBoxItems != null)
				{
					var newValue = (bool)e.NewValue;

					var autoScrollToCurrentItemWorker = new EventHandler((s1, e2) => OnAutoScrollToCurrentItem(listBox, listBox.Items.CurrentPosition));

					if (newValue)
						listBoxItems.CurrentChanged += autoScrollToCurrentItemWorker;
					else
						listBoxItems.CurrentChanged -= autoScrollToCurrentItemWorker;
				}
			}
		}

		/// <summary>
		/// This method will be called when the ListBox should
		/// </summary>
		/// <param name="listBox">The ListBox which should be scrolled</param>
		/// <param name="index">The index of the item to which it should be scrolled</param>
		public static void OnAutoScrollToCurrentItem(ListBox listBox, int index)
		{
			
			if (listBox != null && listBox.Items != null && listBox.Items.Count > index && index >= 0)
			{
//				listBox.ScrollIntoView(listBox.Items[index]);


				var listBoxItem = listBox.ItemContainerGenerator.ContainerFromItem(listBox.Items[index]) as FrameworkElement;
				if (listBoxItem != null)
				{
					var y = listBoxItem.PointToScreen(new Point()).Y;

					if (listBox.ActualHeight - y - 200 < 0)
					{
						var scrollViewer = WpfHelper.TryFindChild<ScrollViewer>(listBox);
						var baselingOffset = scrollViewer.PointToScreen(new Point()).Y;
						var currentContentOffset = scrollViewer.ContentVerticalOffset;
						var listboxOffset = listBoxItem.PointToScreen(new Point()).Y;
						var newOffset = currentContentOffset + listboxOffset - baselingOffset;
						scrollViewer.ScrollToVerticalOffset(newOffset);

					}
				}

				
				//listBox.RaiseCurrentItemLocationToContainer(listBox.Items[index]);
			}
		}

		#endregion
		#endregion
		#region AutoScrollToCurrentItemAndFindInContainer
		#region Properties

		public static readonly DependencyProperty AutoScrollToCurrentItemAndFindInContainerProperty = DependencyProperty.RegisterAttached("AutoScrollToCurrentItemAndFindInContainer", typeof(bool), typeof(ListBoxExtenders), new UIPropertyMetadata(default(bool), OnAutoScrollToCurrentItemAndFindInContainerChanged));

		/// <summary>
		/// Returns the value of the AutoScrollToCurrentItemAndFindInContainerProperty
		/// </summary>
		/// <param name="obj">The dependency-object whichs value should be returned</param>
		/// <returns>The value of the given property</returns>
		public static bool GetAutoScrollToCurrentItemAndFindInContainer(DependencyObject obj)
		{
			return (bool)obj.GetValue(AutoScrollToCurrentItemAndFindInContainerProperty);
		}

		/// <summary>
		/// Sets the value of the AutoScrollToCurrentItemAndFindInContainerProperty
		/// </summary>
		/// <param name="obj">The dependency-object whichs value should be set</param>
		/// <param name="value">The value which should be assigned to the AutoScrollToCurrentItemAndFindInContainerProperty</param>
		public static void SetAutoScrollToCurrentItemAndFindInContainer(DependencyObject obj, bool value)
		{
			obj.SetValue(AutoScrollToCurrentItemAndFindInContainerProperty, value);
		}

		#endregion

		#region Events

		/// <summary>
		/// This method will be called when the AutoScrollToCurrentItemAndFindInContainer
		/// property was changed
		/// </summary>
		/// <param name="s">The sender (the ListBox)</param>
		/// <param name="e">Some additional information</param>
		public static void OnAutoScrollToCurrentItemAndFindInContainerChanged(DependencyObject s, DependencyPropertyChangedEventArgs e)
		{
			var listBox = s as ListBox;
			if (listBox != null)
			{
				var listBoxItems = listBox.Items;
				if (listBoxItems != null)
				{
					var newValue = (bool)e.NewValue;

					var AutoScrollToCurrentItemAndFindInContainerWorker = new EventHandler((s1, e2) => OnAutoScrollToCurrentItemAndFindInContainer(listBox, listBox.Items.CurrentPosition));

					if (newValue)
						listBoxItems.CurrentChanged += AutoScrollToCurrentItemAndFindInContainerWorker;
					else
						listBoxItems.CurrentChanged -= AutoScrollToCurrentItemAndFindInContainerWorker;
				}
			}
		}

		/// <summary>
		/// This method will be called when the ListBox should
		/// </summary>
		/// <param name="listBox">The ListBox which should be scrolled</param>
		/// <param name="index">The index of the item to which it should be scrolled</param>
		public static void OnAutoScrollToCurrentItemAndFindInContainer(ListBox listBox, int index)
		{
			if (listBox != null && listBox.Items != null && listBox.Items.Count > index && index >= 0)
			{
				//var sv = WpfHelper.TryFindChild<ScrollViewer>(listBox);
				//sv.ScrollChanged += (o,e) =>
				//                        {
				//                          
		
				//                        };
				listBox.ScrollIntoView(listBox.Items[index]);
			}
		}

	 

		#endregion
		#endregion

		#region AutoCenterCurrentItem
		#region Properties

		public static readonly DependencyProperty AutoCenterCurrentItemProperty = DependencyProperty.RegisterAttached("AutoCenterCurrentItem", typeof(bool), typeof(ListBoxExtenders), new UIPropertyMetadata(default(bool), OnAutoCenterCurrentItemChanged));

		/// <summary>
		/// Returns the value of the AutoCenterCurrentItemProperty
		/// </summary>
		/// <param name="obj">The dependency-object whichs value should be returned</param>
		/// <returns>The value of the given property</returns>
		public static bool GetAutoCenterCurrentItem(DependencyObject obj)
		{
			return (bool)obj.GetValue(AutoCenterCurrentItemProperty);
		}

		/// <summary>
		/// Sets the value of the AutoCenterCurrentItemProperty
		/// </summary>
		/// <param name="obj">The dependency-object whichs value should be set</param>
		/// <param name="value">The value which should be assigned to the AutoCenterCurrentItemProperty</param>
		public static void SetAutoCenterCurrentItem(DependencyObject obj, bool value)
		{
			obj.SetValue(AutoCenterCurrentItemProperty, value);
		}

		#endregion

		#region Events

		/// <summary>
		/// This method will be called when the AutoCenterCurrentItem
		/// property was changed
		/// </summary>
		/// <param name="s">The sender (the ListBox)</param>
		/// <param name="e">Some additional information</param>
		public static void OnAutoCenterCurrentItemChanged(DependencyObject s, DependencyPropertyChangedEventArgs e)
		{
			var listBox = s as ListBox;
			if (listBox != null)
			{
				var listBoxItems = listBox.Items;
				if (listBoxItems != null)
				{
					var newValue = (bool)e.NewValue;

					var AutoCenterCurrentItemWorker = new EventHandler((s1, e2) => OnAutoCenterCurrentItem(listBox, listBox.Items.CurrentPosition));

					if (newValue)
						listBoxItems.CurrentChanged += AutoCenterCurrentItemWorker;
					else
						listBoxItems.CurrentChanged -= AutoCenterCurrentItemWorker;
				}
			}
		}

		/// <summary>
		/// This method will be called when the ListBox should
		/// </summary>
		/// <param name="listBox">The ListBox which should be scrolled</param>
		/// <param name="index">The index of the item to which it should be scrolled</param>
		public static void OnAutoCenterCurrentItem(ListBox listBox, int index)
		{
			//	var sdsd = IsUserVisible(listBox.Items[index] as FrameworkElement, listBox);
			if (listBox != null && listBox.Items != null && listBox.Items.Count > index && index >= 0)
				listBox.ScrollIntoViewCentered(listBox.Items[index]);
		}

		#endregion
		#endregion

		#region AutoScrollToTopCurrentItem
		#region Properties

		public static readonly DependencyProperty AutoScrollToTopCurrentItemProperty = DependencyProperty.RegisterAttached("AutoScrollToTopCurrentItem", typeof(bool), typeof(ListBoxExtenders), new UIPropertyMetadata(default(bool), OnAutoScrollToTopCurrentItemChanged));

		/// <summary>
		/// Returns the value of the AutoScrollToTopCurrentItemProperty
		/// </summary>
		/// <param name="obj">The dependency-object whichs value should be returned</param>
		/// <returns>The value of the given property</returns>
		public static bool GetAutoScrollToTopCurrentItem(DependencyObject obj)
		{
			return (bool)obj.GetValue(AutoScrollToTopCurrentItemProperty);
		}

		/// <summary>
		/// Sets the value of the AutoScrollToTopCurrentItemProperty
		/// </summary>
		/// <param name="obj">The dependency-object whichs value should be set</param>
		/// <param name="value">The value which should be assigned to the AutoScrollToTopCurrentItemProperty</param>
		public static void SetAutoScrollToTopCurrentItem(DependencyObject obj, bool value)
		{
			obj.SetValue(AutoScrollToTopCurrentItemProperty, value);
		}

		#endregion

		#region Events

		/// <summary>
		/// This method will be called when the AutoScrollToTopCurrentItem
		/// property was changed
		/// </summary>
		/// <param name="s">The sender (the ListBox)</param>
		/// <param name="e">Some additional information</param>
		public static void OnAutoScrollToTopCurrentItemChanged(DependencyObject s, DependencyPropertyChangedEventArgs e)
		{
			var listBox = s as ListBox;
			if (listBox != null)
			{
				var listBoxItems = listBox.Items;
				if (listBoxItems != null)
				{
					var newValue = (bool)e.NewValue;

					var autoScrollToTopCurrentItemWorker = new EventHandler((s1, e2) => OnAutoScrollToTopCurrentItem(listBox, listBox.Items.CurrentPosition));

					if (newValue)
						listBoxItems.CurrentChanged += autoScrollToTopCurrentItemWorker;
					else
						listBoxItems.CurrentChanged -= autoScrollToTopCurrentItemWorker;
				}
			}
		}

		/// <summary>
		/// This method will be called when the ListBox should
		/// </summary>
		/// <param name="listBox">The ListBox which should be scrolled</param>
		/// <param name="index">The index of the item to which it should be scrolled</param>
		public static void OnAutoScrollToTopCurrentItem(ListBox listBox, int index)
		{
			if (listBox != null && listBox.Items != null && listBox.Items.Count > index && index >= 0)
				listBox.ScrollToTopCurrentItem(listBox.Items[index]);
		}

		#endregion
		#endregion
		 

	}
}

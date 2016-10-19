using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using GalaSoft.MvvmLight;
using WCS.Core;
using WCS.Shared.Controls;
using WCS.Shared.Orders;
using WCS.Shared.Physio.Schedule;
using WCS.Shared.Schedule;
using WCS.Shared.Timeline;

namespace WCS.Shared.Location
{
	/// <summary>
	/// Coordinates all the interactions between all the of different timeline items 
	/// </summary>
	public class LocationTrackingCoordinator : ViewModelBase
	{
		#region Fields

		private TimelineItemObservableCollection _timelineItems;
		private CollectionViewSource _detections;
		private LocationMovementCollection _locationMovementCollection;
		private DetectionViewModel _selectedDetection;

		#endregion

		#region Public API

		#region Constructors

		public LocationTrackingCoordinator()
		{
			_timelineItems = new TimelineItemObservableCollection();
			LocationMovementCollection = new LocationMovementCollection();

            if (Application.Current != null)
            {
                Application.Current.Dispatcher.BeginInvokeIfRequired((() =>
                                                                          {
                                                                              Detections = new CollectionViewSource();
                                                                              Detections.SortDescriptions.Add(
                                                                                  new SortDescription("StartTime",
                                                                                                      ListSortDirection.
                                                                                                          Descending));
                                                                              Detections.Source = _timelineItems;
                                                                          }));
            }
		}

		#endregion

		public string CurrentDetection
		{
			get
			{
				if (!_timelineItems.Any()) return "No detections recieved";
				var detections = _timelineItems.OfType<DetectionViewModel>().ToList();

                if (!detections.Any()) return "No detections recieved";
                if (detections.Last().Direction == DetectionDirection.Out) return "Attrium";

				var inDetections = detections.Where(d => d.Direction == DetectionDirection.In).ToList();

				if (!inDetections.Any()) return "Unknown";
				return inDetections.OrderBy(t => t.StartTime).Last().Code;
			}
		}

        public string CurrentDetectionLocation
        {
            get
            {
                if (!_timelineItems.Any()) return "No detections recieved";
                var detections = _timelineItems.OfType<DetectionViewModel>().ToList();

                if (!detections.Any()) return "No detections recieved";
                if (detections.Last().Direction == DetectionDirection.Out) return "Attrium";

                var inDetections = detections.Where(d => d.Direction == DetectionDirection.In).ToList();

                if (!inDetections.Any()) return "Unknown";
                return inDetections.OrderBy(t => t.StartTime).Last().Location;
            }
        }

		#region IDispose

		private bool _disposed = false;

		protected override void Dispose(bool disposing)
		{
			if (!this._disposed)
			{
				if (_timelineItems != null)
				{
					_timelineItems.ForEach(n =>
					{
						if (n != null)
						{
							n.Dispose();
						}
					});
					_timelineItems.Clear();
					_timelineItems = null;

				}

				if (_detections != null)
				{
					_detections = null;
				}
				_disposed = true;

			}
		}

		#endregion

		public CollectionViewSource Detections
		{
			get { return _detections; }
			set
			{
				_detections = value;
				this.DoRaisePropertyChanged(() => Detections, RaisePropertyChanged);
			}
		}

		public LocationMovementCollection LocationMovementCollection
		{
			get { return _locationMovementCollection; }
			set
			{
				_locationMovementCollection = value;
				this.DoRaisePropertyChanged(() => LocationMovementCollection, RaisePropertyChanged);
			}
		}

		public DetectionViewModel SelectedDetection
		{
			get { return _selectedDetection; }
			set
			{
                    if (_selectedDetection != null) { _selectedDetection.IsSelected = false; }
					_selectedDetection = value;
                    _selectedDetection.IsSelected = true;
					this.DoRaisePropertyChanged(() => SelectedDetection, RaisePropertyChanged);
				 
			}
		}
		 
		/// <summary>
		/// Tries to update the coordinators collections with a potentially different order's details
		/// </summary>
		/// <param name="detections">The detections</param>
		internal void Synchronise(IList<Detection> detections)
		{
			MergeAlgorithm<Detection, ITimelineItem>.Merge(detections,
					_timelineItems.Where(a => a.TimelineType == TimelineItemType.RdifDetection),
					(toAdd) => DoAddItem(toAdd),
					null);

			LocationMovementCollection.Synchronise(detections.ToList());
		}
		
		internal void HandleMinuteTimerTick()
		{
			_timelineItems.ForEach(ti => ti.HandleMinuteTimerTick());
			LocationMovementCollection.ForEach(location => location.ForEach(movement => movement.HandleMinuteTimerTick()));
		}


		public void BringItemIntoFocus(IOrderItem item)
		{ }

		/// <summary>
		/// Adds the new item to the head of the stack
		/// </summary>
		/// <param name="item">The item.</param>
		public void AddNewTimelineItem(ITimelineItem item)
		{
			lock ((_timelineItems as ICollection).SyncRoot)
			{
				Application.Current.Dispatcher.InvokeIfRequired((() => _timelineItems.Add(item)));
			}
		}

		/// <summary>
		/// Clears out all the state
		/// </summary>
		public void Clear()
		{
			if (SelectedDetection == null)
				throw new InvalidOperationException("ScheduleItemViewModel cannot be null");

			lock ((_timelineItems as ICollection).SyncRoot)
			{
				Application.Current.Dispatcher.InvokeIfRequired((() =>
				{
					UnhookTimelineItem(_timelineItems[0]);
				}));
			}
		}

		#endregion

		#region Private Members


		/// <summary>
		/// Handles the event when a item wants to be removed from the timeline
		/// </summary>
		private void DoAddItem(Detection itemToAdd)
		{
			Application.Current.Dispatcher.InvokeIfRequired((() =>
																{
																	var detection = new DetectionViewModel(itemToAdd, null);
																	AddNewTimelineItem(detection);
																	this.DoRaisePropertyChanged(() => CurrentDetection, RaisePropertyChanged);

																	SelectedDetection = detection;
																}));
		}

		/// <summary>
		/// Handles the event when a item wants to be removed from the editor
		/// </summary>
		private void DoRemoveItem(ITimelineItem itemToRemove)
		{
			Application.Current.Dispatcher.InvokeIfRequired((() =>
																{
																	HandleRequestRemoveItem(itemToRemove);
																}));
		}

		/// <summary>
		/// Handles the event when a item wants to be removed from the editor
		/// </summary>
		private void HandleRequestRemoveItem(ITimelineItem itemToRemove)
		{
			Application.Current.Dispatcher.InvokeIfRequired((() =>
			{
				_timelineItems.Remove(itemToRemove);
				itemToRemove.Dispose();
			}));
		}



		/// <summary>
		/// Unhooks all the events for the item so it can be released from memory
		/// </summary>
		/// <param name="itemToRemove">The item to remove.</param>
		private void UnhookTimelineItem(ITimelineItem itemToRemove)
		{
			Application.Current.Dispatcher.InvokeIfRequired((() =>
			{
				_timelineItems.Remove(itemToRemove);
				itemToRemove.Dispose();
			}));
		}

		#endregion
	}
}

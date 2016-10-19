using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using GalaSoft.MvvmLight.Command;
using WCS.Core;
using WCS.Shared.Controls;
using WCS.Shared.Location;
using WCS.Shared.Schedule;

namespace WCS.Shared.Physio.Schedule
{
	public class LocationMovementCollection : ObservableCollection<RfidMovementCollection>
	{
		internal void Synchronise(List<Detection> detections)
		{
			if (detections == null) return;

			var movements = LocationMovementCollection.MergeDetectionsIntoMovements(detections);
			var locationsMovements = movements.GroupBy(d => d.Label ).ToList();
			var newLocationsDefinitions = new List<RfidMovementCollection>();
			locationsMovements.ForEach(locationMovement =>
											{
												newLocationsDefinitions.Add(new RfidMovementCollection(locationMovement.First().Label, locationMovement.ToList()));
											});


			MergeAlgorithm<RfidMovementCollection, RfidMovementCollection>.Merge(newLocationsDefinitions, this,
					(toAdd) => {Application.Current.Dispatcher.InvokeIfRequired((() =>Add(toAdd))); },
					(toRemove) => { Application.Current.Dispatcher.InvokeIfRequired((() => Remove(toRemove))); });

			
		}

		public static List<MovementViewModel> MergeDetectionsIntoMovements(List<Detection> detections)
		{
			if (!detections.Any()) return new List<MovementViewModel>();


			var movements = new List<MovementViewModel>();
			TimeSpan? currentStartTime = null;

			detections.OrderBy(d => d.Timestamp).ForEach(detection =>
			{
				if (detection.Direction == DetectionDirection.In)
				{
					currentStartTime = detection.Timestamp.TimeOfDay;
				}
				else if (detection.Direction == DetectionDirection.Out)
				{
					currentStartTime = currentStartTime ?? TimeSpan.FromHours(5);
					var d = new Detection()
								{
									DetectionId = detection.Id,
									DetectionLocation = detection.DetectionLocation,
									PatientId = detection.PatientId,
									Timestamp = new DateTime(detection.Timestamp.Year, detection.Timestamp.Month, detection.Timestamp.Day, currentStartTime.Value.Hours, currentStartTime.Value.Minutes, currentStartTime.Value.Seconds)
								};

					movements.Add(new MovementViewModel(d.Id, currentStartTime.Value, detection.Timestamp.TimeOfDay - currentStartTime.Value,d.DetectionLocation.LocationName, detection.Timestamp.TimeOfDay));
					currentStartTime = null;
				}

			});
			if (currentStartTime.HasValue)
			{
				var detection = detections.Last();
				var d = new Detection()
				{
					DetectionId = detection.Id,
					DetectionLocation = detection.DetectionLocation,
					PatientId = detection.PatientId,
					Timestamp = detection.Timestamp
				};
				movements.Add(new MovementViewModel(d.Id, currentStartTime.Value, DateTime.Now.TimeOfDay - currentStartTime.Value, d.DetectionLocation.LocationName, null));

			}

			movements.Sort();
			return movements;
		}

	}
}

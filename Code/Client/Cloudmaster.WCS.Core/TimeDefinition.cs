using System;
using System.Collections.Generic;
using System.Linq;

namespace WCS.Core
{
	public struct TimeDefinition : ITimeDefinition, IComparable<TimeDefinition>
	{
		private TimeSpan _beginTime;
		private TimeSpan _duration;
		 
		public TimeDefinition(TimeSpan beginTime, TimeSpan duration)
		{
			_beginTime = beginTime;
			_duration = duration;
		}

		public TimeSpan BeginTime
		{
			get { return _beginTime; }
		}

		public TimeSpan Duration
		{
			get { return _duration; }
		}


		public int Id
		{
			get { return GetFingerprint(); }
		}

		public int GetFingerprint()
		{
			return BeginTime.GetHashCode() ^ Duration.GetHashCode();
		}

		public int CompareTo(ITimeDefinition other)
		{
			return BeginTime.CompareTo(other.BeginTime);
		}

		public int CompareTo(TimeDefinition other)
		{
			return BeginTime.CompareTo(other.BeginTime);
		}

		public static List<TimeDefinition> MergeTimes(List<ITimeDefinition> times)
		{
			if (!times.Any()) return times.Cast<TimeDefinition>().ToList();

			times.Sort();

			var mergedTimes = new List<TimeDefinition>();
			TimeSpan? currentStartTime = null;
			TimeSpan? currentDuration = null;
			TimeSpan? runningEndTime = null;
			times.ForEach(t =>
			{
				if (!currentStartTime.HasValue)
				{
					currentStartTime = t.BeginTime;
					currentDuration = t.Duration;
					runningEndTime = currentStartTime.Value + currentDuration.Value;
				}
				else if ((t.BeginTime + t.Duration) <= runningEndTime)
				{
					//ignore it
				}
				else if (t.BeginTime < runningEndTime)
				{
					currentDuration = (t.BeginTime + t.Duration) - currentStartTime;
					runningEndTime = currentStartTime.Value + currentDuration.Value;
				}
				else
				{
					mergedTimes.Add(new TimeDefinition(currentStartTime.Value, currentDuration.Value));
					currentStartTime = t.BeginTime;
					currentDuration = t.Duration;
					runningEndTime = currentStartTime.Value + currentDuration.Value;
				}
			});
			if (currentStartTime.HasValue) mergedTimes.Add(new TimeDefinition(currentStartTime.Value, currentDuration.Value));

			var baseline = TimeSpan.FromHours(5);
			var toRemove = mergedTimes.Where(t => (t.BeginTime + t.Duration) <= baseline).ToList();
			toRemove.ForEach(t => mergedTimes.Remove(t));

			var toShorten = mergedTimes.Where(t => (t.BeginTime) <= baseline).ToList();
			if (toShorten.Any())
			{
				toShorten.ForEach(t =>
				{
					mergedTimes.Add(new TimeDefinition(baseline, t.Duration + t.BeginTime - baseline));
					mergedTimes.Remove(t);

				});
			}
			mergedTimes.Sort();
			return mergedTimes;
		}

		public static bool ContainsTime(List<ITimeDefinition> timeDefinitions, TimeSpan time)
		{
			foreach (var timeDefinition in timeDefinitions)
			{
				if(timeDefinition.BeginTime < time && time < (timeDefinition.BeginTime + timeDefinition.Duration)) return true;
			}
			return false;
		}


		public static TimeSpan? GetNextTime(List<ITimeDefinition> timeDefinitions, TimeSpan time)
		{
			foreach (var timeDefinition in timeDefinitions)
			{
				if ((timeDefinition.BeginTime + timeDefinition.Duration) < time) continue;

				return timeDefinition.BeginTime;
			}
			return null;
		}
	}
}

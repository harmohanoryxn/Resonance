using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using WCS.Core;
using WCS.Shared.Location;
using WCS.Shared.Schedule;

namespace WCS.Shared.Physio.Schedule
{
	public class RfidMovementCollection : ObservableCollection<ITimelineItem>, IIdentifable
	{
		public RfidMovementCollection(string locationName, List<MovementViewModel> list)
		{
			LocationName = locationName;
			list.ForEach(Add);
		}

		public string LocationName { get; private set; }

		public int Id
		{
			get { return GetFingerprint(); }
		}

        public TimeSpan? FirstMovement
        {
            get { return this.Min(d => d.StartTime); }
        }

		public int GetFingerprint()
		{
			int fp = LocationName.GetHashCode();
			if (this.Any()) fp = fp ^ this.Select(i => i.GetFingerprint()).ToList().Aggregate((accFp, nextFp) => nextFp + accFp);
			return fp;
		}
	}
}

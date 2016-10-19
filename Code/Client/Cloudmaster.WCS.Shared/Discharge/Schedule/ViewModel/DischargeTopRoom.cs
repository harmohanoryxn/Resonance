using System.Collections.Generic;
using System.Linq;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using WCS.Core;

namespace WCS.Shared.Discharge.Schedule
{
	public class DischargeTopRoom : IIdentifable
	{
		public Room Room { get; set; }
		public List<BedDischargeData> Discharges { get; set; }

		public DischargeTopRoom(Room room, List<BedDischargeData> discharges)
		{
			Room = room;
			Discharges = discharges;
		}

		public int Id
		{
			get { return (Room != null) ? Room.Name.GetHashCode() : 0; }
		}

		public int GetFingerprint()
		{
			var fp = 0;
			if (Room != null)
				fp = fp ^ Room.GetFingerprint();
			Discharges.ToList().ForEach(o => fp = fp ^ o.GetFingerprint());
			return fp;
		}
	}
}

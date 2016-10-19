using System.Collections.Generic;
using System.Linq;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using WCS.Core;

namespace WCS.Shared.Beds
{
	public class TopRoom : IIdentifable
	{
		public Room Room { get; set; }
		public List<Bed> Beds { get; set; }

		public TopRoom(Room room, List<Bed> beds)
		{
			Room = room;
			Beds = beds;
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
			Beds.ToList().ForEach(o => fp = fp ^ o.GetFingerprint());
			return fp;
		}
	}
}

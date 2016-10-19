using System.Linq;
using WCS.Core;

namespace Cloudmaster.WCS.DataServicesInvoker.DataServices
{
	public partial class BedDischargeData : IIdentifable
	{

		public int Id
		{
			get { return BedId; }
		}

		public int GetFingerprint()
		{
			var fp = BedId;
			if (!string.IsNullOrEmpty(Name))
				fp = fp ^ Name.GetHashCode();
			if (Room != null)
				fp = fp ^ Room.GetFingerprint();
			if (Location != null)
				fp = fp ^ Location.GetFingerprint();
			if (Room != null)
				fp = fp ^ Room.GetFingerprint();
			if (CurrentAdmission != null)
                fp = fp ^ CurrentAdmission.GetFingerprint();
			
			return fp;

		}
	}
}

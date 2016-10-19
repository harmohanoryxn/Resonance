using System.Linq;
using WCS.Core;

namespace Cloudmaster.WCS.DataServicesInvoker.DataServices
{
	public partial class Bed : IIdentifable
	{

		public int Id
		{
			get { return BedId; }
		}

		public int GetFingerprint()
		{
            var fp = BedId ^ CurrentStatus.GetHashCode() ^ CriticalCareIndicators.GetFingerprint();
			if (!string.IsNullOrEmpty(Name))
				fp = fp ^ Name.GetHashCode();
			if (Room != null)
				fp = fp ^ Room.GetFingerprint();
			if (Location != null)
				fp = fp ^ Location.GetFingerprint();
			if (Room != null)
				fp = fp ^ Room.GetFingerprint();
			if (LatestService != null)
				fp = fp ^ LatestService.GetFingerprint();
            if (EstimatedDischargeDate.HasValue)
                fp = fp ^ EstimatedDischargeDate.GetHashCode();

			if (Notes.Any()) fp = fp ^ Notes.Select(n => n.GetFingerprint()).ToList().Aggregate((accFp, nextFp) => nextFp + accFp);
			if (Services.Any()) fp = fp ^ Services.Select(s => s.GetFingerprint()).ToList().Aggregate((accFp, nextFp) => nextFp + accFp);
			if (AvailableTimes.Any()) fp = fp ^ AvailableTimes.Select(s => s.GetFingerprint()).ToList().Aggregate((accFp, nextFp) => nextFp + accFp);
			
			return fp;

		}
	}
}

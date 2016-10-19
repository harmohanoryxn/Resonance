using System.Collections.Generic;
using System.Linq;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using WCS.Core;

namespace WCS.Shared.Admissions.Schedule
{
	public class AdmissionsTopWard : IIdentifable
	{
		public Cloudmaster.WCS.DataServicesInvoker.DataServices.Location Ward { get; set; }
		public List<BedDischargeData> Beds { get; set; }

		public AdmissionsTopWard(Cloudmaster.WCS.DataServicesInvoker.DataServices.Location ward, List<BedDischargeData> admissions)
		{
			Ward = ward;
			Beds = admissions;
		}

		public int Id
		{
			get { return (Ward != null) ? Ward.GetFingerprint() : 0; }
		}

		public int GetFingerprint()
		{
			var fp = 0;
			if (Ward != null)
				fp = fp ^ Ward.GetFingerprint();
			Beds.ToList().ForEach(b => fp = fp ^ b.GetFingerprint());
			return fp;
		}
	}
}

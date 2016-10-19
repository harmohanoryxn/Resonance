using System;
using System.Linq;
using WCS.Core;

namespace Cloudmaster.WCS.DataServicesInvoker.DataServices
{
	public partial class AdmissionsData : IIdentifable
	{

		public int Id
		{
			get { return GetFingerprint(); }
		}

		public int GetFingerprint()
		{
			var fp = 0;
			if (Beds.Any()) fp = fp ^ Beds.Select(b => b.GetFingerprint()).ToList().Aggregate((accFp, nextFp) => nextFp + accFp);
            if (Admissions.Any()) fp = fp ^ Admissions.Select(adm => adm.GetFingerprint()).ToList().Aggregate((accFp, nextFp) => nextFp + accFp);

			return fp;

		}
	}
}

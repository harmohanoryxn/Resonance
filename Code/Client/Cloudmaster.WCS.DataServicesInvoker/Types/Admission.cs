
using Cloudmaster.WCS.DataServicesInvoker.Types;
using WCS.Core;

namespace Cloudmaster.WCS.DataServicesInvoker.DataServices
{
	public partial class Admission : IIdentifable
	{
		public int GetFingerprint()
		{
            var fp = AdmissionId ^ AdmitDateTime.GetHashCode() ^ DischargeDateTime.GetHashCode() ^ Type.GetHashCode() ^ Status.GetHashCode();
			if (Patient != null)
				fp = fp ^ Patient.GetFingerprint();
			if (Location != null)
				fp = fp ^ Location.GetFingerprint();
			if (CriticalCareIndicators != null)
				fp = fp ^ CriticalCareIndicators.GetFingerprint();
			if (!string.IsNullOrEmpty(PrimaryDoctor))
				fp = fp ^ PrimaryDoctor.GetHashCode();
			if (!string.IsNullOrEmpty(AttendingDoctor))
				fp = fp ^ AttendingDoctor.GetHashCode();
			if (!string.IsNullOrEmpty(AdmittingDoctor))
				fp = fp ^ AdmittingDoctor.GetHashCode();
			return fp;

		}

		public int Id
		{
			get { return AdmissionId; }
		}

		public MultiSelectAdmissionStatusFlag AdmissionStatusFlag
		{
			get { return (MultiSelectAdmissionStatusFlag) (int) Status; }
			set { Status = (AdmissionStatus) (int) value; }
		}
	}
}
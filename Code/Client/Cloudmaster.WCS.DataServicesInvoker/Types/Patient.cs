
namespace Cloudmaster.WCS.DataServicesInvoker.DataServices
{
	public partial class Patient
	{
		public int GetFingerprint()
		{
			var fingerprint = PatientId ^ DateOfBirth.GetHashCode() ^ (IsAssistanceRequired ? "RequiresAssistance" : "NotRequiresAssistance").GetHashCode();
				if (!string.IsNullOrEmpty(IPeopleNumber))
					fingerprint = fingerprint ^ IPeopleNumber.GetHashCode();
				if (!string.IsNullOrEmpty(GivenName))
					fingerprint = fingerprint ^ GivenName.GetHashCode();
				if (!string.IsNullOrEmpty(FamilyName))
					fingerprint = fingerprint ^ FamilyName.GetHashCode();
					return fingerprint;
			
		}
	}
}

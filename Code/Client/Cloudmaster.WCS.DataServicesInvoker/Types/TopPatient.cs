using System.Collections.Generic;
using System.Linq;
using Cloudmaster.WCS.DataServicesInvoker.DataServices;
using WCS.Core;

namespace Cloudmaster.WCS.DataServicesInvoker.Types
{
	public class TopPatient : IIdentifable
	{
		public Patient Patient { get; set; }
		public Admission Admission { get; set; }
		public List<Order> Orders { get; set; }

		public TopPatient(Patient patient, Admission admission, List<Order> orders)
		{
			Patient = patient;
			Admission = admission;
			Orders = orders;
		}

		public int Id
		{
			get { return (Patient != null) ? Patient.PatientId : 0; }
		}

		public int GetFingerprint()
		{
			var fp = 0;
			if (Patient != null)
				fp = fp ^ Patient.GetFingerprint();
			if (Admission != null)
				fp = fp ^ Admission.GetFingerprint();

			if (Orders.Any()) fp = fp ^ Orders.Select(o => o.GetFingerprint()).ToList().Aggregate((accFp, nextFp) => nextFp + accFp);

			return fp;
		}
	}
}

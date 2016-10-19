
using System.Collections.Generic;
using System.Linq;
using WCS.Core;

namespace Cloudmaster.WCS.DataServicesInvoker.DataServices
{
	public partial class Order : IIdentifable
	{

		public int Id
		{
			get { return OrderId; }
		}

		public int GetFingerprint()
		{
			var fp = OrderId ^ ProcedureTime.GetHashCode() ^ Duration.GetHashCode() ^ Status.GetHashCode() ^
				(IsHidden ? "Hidden" : "NotHidden").GetHashCode() ^
				CompletedTime.GetHashCode() ^ (Acknowledged ? "Acknowledged" : "noAcknowledged").GetHashCode();
			if (!string.IsNullOrEmpty(ProcedureDescription))
				fp = fp ^ ProcedureDescription.GetHashCode();
			if (!string.IsNullOrEmpty(ProcedureCode))
				fp = fp ^ ProcedureCode.GetHashCode();
			if (!string.IsNullOrEmpty(DepartmentCode))
				fp = fp ^ DepartmentCode.GetHashCode();
			if (!string.IsNullOrEmpty(DepartmentName))
				fp = fp ^ DepartmentName.GetHashCode();
			if (!string.IsNullOrEmpty(OrderNumber))
				fp = fp ^ OrderNumber.GetHashCode();
			if (!string.IsNullOrEmpty(ClinicalIndicators))
				fp = fp ^ ClinicalIndicators.GetHashCode();
            if (!string.IsNullOrEmpty(History))
                fp = fp ^ History.GetHashCode();
            if (!string.IsNullOrEmpty(Diagnosis))
                fp = fp ^ Diagnosis.GetHashCode();
            if (!string.IsNullOrEmpty(CurrentCardiologist))
                fp = fp ^ Diagnosis.GetHashCode();
			if (!string.IsNullOrEmpty(OrderingDoctor))
				fp = fp ^ OrderingDoctor.GetHashCode();
			if (EstimatedProcedureDuration.HasValue)
				fp = fp ^ EstimatedProcedureDuration.GetHashCode();
			if (Admission != null)
				fp = fp ^ Admission.GetFingerprint();

			if (Notifications.Any()) fp = fp ^ Notifications.Select(n => n.GetFingerprint()).ToList().Aggregate((accFp, nextFp) => nextFp + accFp);
			if (Notes.Any()) fp = fp ^ Notes.Select(n => n.GetFingerprint()).ToList().Aggregate((accFp, nextFp) => nextFp + accFp);
			if (Updates.Any()) fp = fp ^ Updates.Select(u => u.GetFingerprint()).ToList().Aggregate((accFp, nextFp) => nextFp + accFp);
			return fp;

		}

		public static List<string> FindDifferences(Order x, Order y)
		{
			var diffs = new List<string>();
			if (x.OrderId != y.OrderId) diffs.Add("OrderId");
			if (x.ProcedureTime != y.ProcedureTime) diffs.Add("ProcedureTime");
			if (x.Duration != y.Duration) diffs.Add("Duration");
			if (x.Status != y.Status) diffs.Add("Status");
			if (x.IsHidden != y.IsHidden) diffs.Add("IsHidden");
			if (x.CompletedTime != y.CompletedTime) diffs.Add("CompletedTime");
			if (x.Acknowledged != y.Acknowledged) diffs.Add("Acknowledged");
			if (x.ProcedureDescription != y.ProcedureDescription) diffs.Add("ProcedureDescription");
			if (x.ProcedureCode != y.ProcedureCode) diffs.Add("ProcedureCode");
			if (x.DepartmentCode != y.DepartmentCode) diffs.Add("DepartmentCode");
			if (x.DepartmentName != y.DepartmentName) diffs.Add("DepartmentName");
			if (x.OrderNumber != y.OrderNumber) diffs.Add("OrderNumber");
			if (x.ClinicalIndicators != y.ClinicalIndicators) diffs.Add("ClinicalIndicators");
			if (x.OrderingDoctor != y.OrderingDoctor) diffs.Add("OrderingDoctor");
			if (x.EstimatedProcedureDuration != y.EstimatedProcedureDuration) diffs.Add("EstimatedProcedureDuration");

			int fpX = 0;
			int fpY = 0;
			if (x.Notifications.Any()) fpX = fpX ^ x.Notifications.Select(n => n.GetFingerprint()).ToList().Aggregate((accFp, nextFp) => nextFp + accFp);
			if (y.Notifications.Any()) fpY = fpY ^ y.Notifications.Select(n => n.GetFingerprint()).ToList().Aggregate((accFp, nextFp) => nextFp + accFp);
			if (fpX != fpY) diffs.Add("Notifications");
			fpX = 0;
			fpY = 0;
			if (x.Notes.Any()) fpX = fpX ^ x.Notes.Select(n => n.GetFingerprint()).ToList().Aggregate((accFp, nextFp) => nextFp + accFp);
			if (y.Notes.Any()) fpY = fpY ^ y.Notes.Select(n => n.GetFingerprint()).ToList().Aggregate((accFp, nextFp) => nextFp + accFp);
			if (fpX != fpY) diffs.Add("Notes");
			fpX = 0;
			fpY = 0;
			if (x.Updates.Any()) fpX = fpX ^ x.Updates.Select(n => n.GetFingerprint()).ToList().Aggregate((accFp, nextFp) => nextFp + accFp);
			if (y.Updates.Any()) fpY = fpY ^ y.Updates.Select(n => n.GetFingerprint()).ToList().Aggregate((accFp, nextFp) => nextFp + accFp);
			if (fpX != fpY) diffs.Add("Updates");
			return (diffs);
		}
	}
}

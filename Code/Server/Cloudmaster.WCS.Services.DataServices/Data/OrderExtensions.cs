using System;
using System.Collections.Generic;
using System.Linq;
using WCS.Data.EF;

namespace WCS.Services.DataServices.Data
{
	public static class OrderExtensions
	{
		public static Order Convert(this WCS.Data.EF.Order o, bool isRadiationRisk)
		{
			var order = new Order();
			order.OrderId = o.orderId;
			order.OrderNumber = o.orderNumber;
			order.DepartmentCode = o.Department.code;
			order.DepartmentName = o.Department.name;
			order.ProcedureCode = o.Procedure.code;
			order.ProcedureDescription = o.Procedure.description;
            order.ProcedureTime = o.procedureTime;
			order.Duration = o.Procedure.durationMinutes.HasValue ? TimeSpan.FromMinutes(o.Procedure.durationMinutes.Value) : new TimeSpan(0, 1, 0, 0);
			order.Status = (OrderStatus)Enum.Parse(typeof(OrderStatus), o.OrderStatus.status);
			order.ClinicalIndicators = o.clinicalIndicator ?? String.Empty;
            order.IsHidden = o.isHidden;
            order.Acknowledged = o.acknowledged;
            order.CompletedTime = o.completedTime;
			order.Admission = o.Admission.Convert(isRadiationRisk);
            order.EstimatedProcedureDuration = o.estimatedProcedureDuration;
		    order.History = o.history;
		    order.Diagnosis = o.diagnosis;
            order.CurrentCardiologist = o.currentCardiologist;
		    order.RequiresSupervision = o.requiresSupervision;
            order.RequiresMedicalRecords = o.requiresMedicalRecords;
            order.RequiresFootwear = o.requiresFootwear;
			if (o.OrderingDoctor != null)
				order.OrderingDoctor = o.OrderingDoctor.name;
			var notes = o.Notes.ToList();
			var enumerable = notes.Where(n => n.dateCreated.HasValue && n.dateCreated.Value.Date == DateTime.Today).Select(n => n.Convert());
			order.Notes = enumerable.ToList();
			var notifications = o.Notifications.ToList();
			var notifs = notifications.Select(n => n.Convert()).ToList();
			order.Notifications = notifs.ToList();
			var updates = o.Updates.ToList();
			var enumerable1 = updates.Select(n => n.Convert());
			order.Updates = enumerable1.ToList();
			return order;
		}
	}
} 
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WCS.Services.DataServices;

namespace WCS.Services.DataServices.Data
{
	public static class UpdateExtensions
	{
		public static Update Convert(this WCS.Data.EF.Update wcsUpdate)
		{
			var update = new Update();
			if (wcsUpdate.Order != null)
				update.OrderId = wcsUpdate.Order.orderId;
			if (wcsUpdate.Bed != null)
				update.BedId = wcsUpdate.Bed.bedId;
            if (wcsUpdate.Admission != null)
                update.AdmissionId = wcsUpdate.Admission.admissionId;
            update.Type = wcsUpdate.type;
			update.Source = wcsUpdate.source;
			update.Value = wcsUpdate.value;
			update.Created = wcsUpdate.dateCreated.Value;
			return update;
		}
	}
} 
 
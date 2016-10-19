using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Cloudmaster.WCS.Classes;
using Cloudmaster.WCS.Controls;
using Cloudmaster.WCS.Model.Alerts;
using Cloudmaster.WCS.Model.Feeds;

namespace Cloudmaster.WCS.Department.Model
{
	public class AlertViewModel : AlertsBase
	{
		private ScheduleViewModel _schedule;

		public AlertViewModel(ScheduleViewModel schedule)
		{
			_schedule = schedule;
		}

		public void Synchronise()
		{
			var unassignedOrders = _schedule.Orders.Items.Where(order => order.Metadata.RequestedDateTimeOverride == null);
			var defaultWardUnassignedOrders = unassignedOrders.Where(order => String.CompareOrdinal(_schedule.DefaultWard.Code, order.Service) == 0);
			 

			if (defaultWardUnassignedOrders.Any())
			{
				string postFix = (defaultWardUnassignedOrders.Count() > 1) ? "s" : string.Empty;

				Summary = string.Format("{0} Appointment{1} Requires Attention", defaultWardUnassignedOrders.Count(), postFix);
				HasAlerts = true;
			}
			else
			{
				HasAlerts = false;
			}
		}
	}
}

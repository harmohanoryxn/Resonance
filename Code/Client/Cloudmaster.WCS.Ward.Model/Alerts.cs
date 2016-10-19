using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Cloudmaster.WCS.Classes;
using Cloudmaster.WCS.Controls.Scheduling;
using Cloudmaster.WCS.Model.Alerts;

namespace Cloudmaster.WCS.Ward.Model
{
    public class Alerts : AlertsBase
    {
        public void UpdateAlerts()
        {
            int count = 0;

            var orders = WardModel.Instance.Feeds.Orders.Items;

            foreach (var order in orders)
            {
                bool isCompleted = (order.ObservationEndDateTime.HasValue);

                if (!isCompleted)
                {
					if (AppointmentExtensions.RequiresAttention(new AppointmentViewModel(order)))
                    {
                        count++;
                    }
                }
            }

            if (count > 0)
            {
                string postFix = (count > 1) ? "s" : string.Empty;

                Summary = string.Format("{0} Alert{1} Awaiting Acknowledgement", count, postFix);
                HasAlerts = true;
            }
            else
            {
                HasAlerts = false;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cloudmaster.WCS.Azure.WcfWebRole;

namespace WCS.Services.DataServices.Reporting
{
    public class ReportGenerator
    {
		public TrackingReport GetTrackingReport(string date, IEnumerable<Order> orders)
        {
            TrackingReport report = new TrackingReport();

            return report;
        }

		public TrackingReport GetTrackingReport(string date, string department)
        {
            TrackingReport report = new TrackingReport();

            return report;
        }
    }
}
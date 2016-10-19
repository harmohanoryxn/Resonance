using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WCS.Services.DataServices;

using Cloudmaster.WCS.WebApps.Cleaning.Extensions;
using System.Globalization;

namespace Cloudmaster.WCS.WebApps.Cleaning.Models
{
    public class CleaningTableSummaryViewModel
    {
        public string CurrentTime;

        public List<CleaningTableSummaryRow> SummaryRows = new List<CleaningTableSummaryRow> ();

        public CleaningTableSummaryViewModel(IList<Bed> bedData)
        {
            CurrentTime = DateTime.Now.ToString("HH:mm tt", new CultureInfo("en-US"));

            bedData.ForEach(b =>
            {
                SummaryRows.Add(new CleaningTableSummaryRow(b));
            });

            SummaryRows = SummaryRows.OrderBy(r => r.Bed.Room.Name).ThenBy(r => r.Bed.Name).ToList();
        }
    }
}
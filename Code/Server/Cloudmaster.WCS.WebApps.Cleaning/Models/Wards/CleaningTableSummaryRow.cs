using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using WCS.Services.DataServices;

namespace Cloudmaster.WCS.WebApps.Cleaning.Models
{
    public class CleaningTableSummaryRow
    {
        public string CurrentTime;

        public Bed Bed;

        public string Name;

        public string Key;

        public string LastCleaned;
        public string NextAvailableTime;

        public bool IsOccupied = false;

        public bool IsDischargeTodayPending = false;
        public bool IsDischargeWithin3Hours = false;

        public bool IsAvailableNow = false;

        public bool HasCriticalCareIndicatorsRelevantToCleaning = false;

        public string EstimatedDischargeTime;

        public string PatientSex;
        
        public CleaningTableSummaryRow(Bed bed)
        {
            CurrentTime = DateTime.Now.ToString("HH:mm tt", new CultureInfo("en-US"));

            Bed = bed;

            Name = string.Format("{0}-{1}", bed.Room.Name, bed.Name);

            Key = string.Format("{0}{1}{2}", bed.Room.Ward, bed.Room.Name, bed.Name).ToUpper();

            var lastCleanedService = bed.Services.Where(s => s.CleaningServiceType == BedCleaningEventType.BedCleaned).OrderBy(s => s.ServiceTime).FirstOrDefault();

            if (lastCleanedService != null)
            {
                var lastCleanedDateTime = lastCleanedService.ServiceTime;

                if (lastCleanedDateTime > DateTime.Today)
                {
                    LastCleaned = lastCleanedDateTime.Value.ToString("h tt", new CultureInfo("en-US"));
                }
                else if (lastCleanedDateTime > DateTime.Today.AddDays(-1))
                {
                    LastCleaned = "Yesterday";
                }
                else
                {
                    LastCleaned = "Overdue";
                }
            }
            else
            {
                LastCleaned = "Never";
            }

            var nextAvailableDateTime = bed.AvailableTimes.OrderBy(a => a.StartTime).FirstOrDefault();

            if (nextAvailableDateTime != null)
            {
                IsAvailableNow = ((nextAvailableDateTime.StartTime < DateTime.Now) && (nextAvailableDateTime.EndTime > DateTime.Now));

                if (IsAvailableNow)
                {
                    NextAvailableTime = "Now";
                }
                else if (nextAvailableDateTime.StartTime > DateTime.Now)
                {
                    NextAvailableTime = nextAvailableDateTime.StartTime.ToString("h tt", new CultureInfo("en-US"));
                }
            }

            IsOccupied = bed.AvailableTimes.Count() > 0;

            var estimatedDischargeDate = bed.EstimatedDischargeDate;

            if ((estimatedDischargeDate.HasValue) && ((estimatedDischargeDate.Value > DateTime.Today) && (estimatedDischargeDate.Value < DateTime.Today.AddDays(1))))
            {
                IsDischargeTodayPending = true;
                EstimatedDischargeTime = estimatedDischargeDate.Value.ToString("h tt", new CultureInfo("en-US"));

                IsDischargeWithin3Hours = estimatedDischargeDate.Value.Subtract(DateTime.Now).Hours < 3;
            }
            else
            {
                IsDischargeTodayPending = false;
            }

            var criticalCareIndicators = bed.CriticalCareIndicators;

            HasCriticalCareIndicatorsRelevantToCleaning = criticalCareIndicators.HasLatexAllergy | criticalCareIndicators.IsMrsaRisk | criticalCareIndicators.IsRadiationRisk;
        }
    }
}

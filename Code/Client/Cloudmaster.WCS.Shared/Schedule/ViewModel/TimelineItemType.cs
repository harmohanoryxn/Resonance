using System;
using System.Linq;

namespace WCS.Shared.Schedule
{

    [Flags]
    public enum TimelineItemType : long
    {
        NoteIn = 1,
        NoteOut = 2,
        Order = 4,
        Notification = 8,
        NotificationAcknowlegement = 16,
        PatientArrived = 32,
        PatientLeaves = 64,
        ProcedureTimeUpdated = 128,
        OrderAssigned = 256,
        OrderCompleted = 512,
        CleaningService = 1024,
        FreeRoom = 2048,
        PatientOccupied = 4096,
        OutstandingNotification = 8192,
        BedStatus = 16384,
        BedMarkedAsDirty = 32768,
        RdifDetection = 65536,
		Discharge = 131072
    }


    public static class TimelineItemTypeExtensions
    {
        public static bool IsSet(this TimelineItemType flags, TimelineItemType flagsToTest)
        {
            return Enum.GetValues(flagsToTest.GetType()).Cast<Enum>().Any(value => flagsToTest.HasFlag(value) && flags.HasFlag(value));
        }

        public static string MakeIntoString(this int val)
        {
            return val.ToString();
        }
    }
}

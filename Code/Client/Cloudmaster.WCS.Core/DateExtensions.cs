using System;

namespace WCS.Core
{
	public static class DateExtensions
	{
		public static string ToWcsFormat(this DateTime dt)
		{
			return dt.ToUniversalTime().ToString("yyyyMMdd\\THHmmss\\Z");
		}
		public static string ToWcsTransferFormat(this DateTime dt)
		{
			return dt.ToUniversalTime().ToLongTimeString();
		}
		public static string ToWcsDisplayDateFormat(this DateTime dt)
		{
			return dt.ToString("dd/MM/yyyy");
		}
		public static string ToWcsTimeFormat(this DateTime dt)
		{
			return dt.ToUniversalTime().ToString("HH:mm");
		}
		public static DateTime ToNearestSecond(this DateTime dt)
		{
			return dt.AddTicks(-(dt.Ticks % TimeSpan.TicksPerSecond));
		}

		public static string ToNoteTimeFormat(this DateTime dt)
		{
			string when = "";
			if (DateTime.Now.Date == dt.Date)
				when = "Today";
			else if (DateTime.Now.Date.AddDays(-1) == dt.Date)
				when = "Yesterday";
			else
				when = string.Format("{0} {1}{2}", dt.DayOfWeek, dt.Day, Day(dt.Day));
			return string.Format("{0} {1}", when, dt.ToString("HH:mm"));
		}
		public static string ToHeadlineTimeFormat(this DateTime dt)
		{
			return dt.DayOfWeek + ", " + dt.Day + Day(dt.Day) + " " + dt.ToString("MMMM") + " " + dt.ToString("yyyy");
		}
		public static string ToHeadlineDateFormat(this DateTime dt)
		{
			return dt.Day + Day(dt.Day);
		}
		public static string ToDayOfWeek(this DateTime dt)
		{
			return dt.DayOfWeek.ToString();
		}

		private static string Day(int dayNumber)
		{
			if (dayNumber == 1 || dayNumber == 21 || dayNumber == 31)
				return "st";
			if (dayNumber == 2 || dayNumber == 22)
				return "nd";
			if (dayNumber == 3 || dayNumber == 23)
				return "rd";
			return "th";
		}

		public static string ToWcsTimeFormat(this TimeSpan ts)
		{
			return string.Format("{0:D2} Hrs {1:D2} Mins", ts.Hours, ts.Minutes);
		}
		public static string ToWcsHoursMinutesFormat(this TimeSpan ts)
		{
			return string.Format("{0:D2}:{1:D2}", ts.Hours, ts.Minutes);
		}

		public static string ToOrderTimeFormat(this TimeSpan ts)
		{
			return string.Format("{0:D2}:{1:D2}", ts.Hours, ts.Minutes);
		}
		public static string ToStopwatchTimeFormat(this TimeSpan ts)
		{
			if (ts.Hours == 0 && ts.Minutes == 0)
				return string.Format("Now");
			else if (ts.Hours == 0)
				return string.Format("{0:D2} {1}", ts.Minutes, ts.Minutes == 1 ? "Min" : "Mins");
			else if (ts.Minutes == 0)
				return string.Format("{0:D1} {1}", ts.Hours, ts.Hours == 1 ? "Hr" : "Hrs");
			else
				return string.Format("{0:D1} {1} {2:D2} {3}", ts.Hours, ts.Hours == 1 ? "Hr" : "Hrs", ts.Minutes, ts.Minutes == 1 ? "Min" : "Mins");
		}
		public static TimeSpan RoundToNearest15Minutes(this TimeSpan ts)
		{
			if (ts.Minutes <= 7)
				return new TimeSpan(ts.Days, ts.Hours, 0, 0);
			if (ts.Minutes <= 23)
				return new TimeSpan(ts.Days, ts.Hours, 15, 0);
			if (ts.Minutes <= 37)
				return new TimeSpan(ts.Days, ts.Hours, 30, 0);
			if (ts.Minutes <= 53)
				return new TimeSpan(ts.Days, ts.Hours, 45, 0);

			return new TimeSpan(ts.Days, ts.Hours + 1, 0, 0);
		}
		public static DateTime RoundToNearestHour(this DateTime ds)
		{
			if (ds.TimeOfDay.Minutes <= 30)
			{
				return ds.RoundDownToNearestHour();
			}
			return ds.RoundUpToNearestHour();
		}
		public static DateTime RoundUpToNearestHour(this DateTime ds)
		{
			return new DateTime(ds.Year, ds.Month, ds.Day, ds.Hour, 0, 0).AddHours(1);
		}
		public static DateTime RoundDownToNearestHour(this DateTime ds)
		{
			return new DateTime(ds.Year, ds.Month, ds.Day, ds.Hour, 0, 0);
		}

		public static DateTime MakeIntoDate(this TimeSpan ts)
		{
			var dt = DateTime.Now;
			return new DateTime(dt.Year, dt.Month, dt.Day, ts.Hours, ts.Minutes, 0);
		}
	}
}

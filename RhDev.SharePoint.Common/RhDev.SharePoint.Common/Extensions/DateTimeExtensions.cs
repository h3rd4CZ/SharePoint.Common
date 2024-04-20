using System;

namespace RhDev.SharePoint.Common.Extensions
{
    public static class DateTimeExtensions
    {
        private const string DATE_TIME_STRING_FORMAT = "d.M.yyyy (HH:mm)";
        private const string DATE_STRING_FORMAT = "d.M.yyyy";

        public static string ToDateString(this DateTime? dateTime)
        {
            if (dateTime.HasValue)
                return dateTime.Value.ToString(DATE_STRING_FORMAT);

            return String.Empty;
        }

        public static string ToDateTimeString(this DateTime? dateTime)
        {
            if (dateTime.HasValue)
                return dateTime.Value.ToString(DATE_TIME_STRING_FORMAT);

            return String.Empty;
        }

        public static string ToDateString(this DateTime dateTime)
        {
            return dateTime.ToString(DATE_STRING_FORMAT);
        }

        public static string ToDateTimeString(this DateTime dateTime)
        {
            return dateTime.ToString(DATE_TIME_STRING_FORMAT);
        }

        public static DateTime TrimToDayOnly(this DateTime dtm)
        {
            return new DateTime(dtm.Year, dtm.Month, dtm.Day, dtm.Hour, 0, 0);
        }
    }
}

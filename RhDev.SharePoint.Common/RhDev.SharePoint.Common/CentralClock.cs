using System;

namespace RhDev.SharePoint.Common
{
    [Serializable]
    public class CentralClock
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public int Hour { get; set; }
        public int Minute { get; set; }
        public int Second { get; set; }

        public static CentralClock FillFromDateTime(DateTime dtm)
        {
            var cc = new CentralClock();
            cc.FillFrom(dtm);

            return cc;
        }

        public void FillFrom(DateTime dtm)
        {
            Year = dtm.Year;
            Month = dtm.Month;
            Day = dtm.Day;
            Hour = dtm.Hour;
            Minute = dtm.Minute;
            Second = dtm.Second;
        }

        public DateTime ExportDateTime => new DateTime(Year, Month, Day, Hour, Minute, Second);

        public override string ToString()
        {
            var dtm = ExportDateTime;

            return dtm.ToString();
        }

        public string ToString(IFormatProvider provider)
        {
            var dtm = ExportDateTime;

            return dtm.ToString(provider);
        }

        public string ToLongTimeString()
        {
            var dtm = ExportDateTime;

            return dtm.ToLongTimeString();
        }

        public string ToLongDateString()
        {
            var dtm = ExportDateTime;

            return dtm.ToLongDateString();
        }
    }
}

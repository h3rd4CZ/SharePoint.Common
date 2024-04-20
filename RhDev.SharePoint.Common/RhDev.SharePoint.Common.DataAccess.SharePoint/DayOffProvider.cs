using System;
using System.Collections.Generic;
using System.Linq;
using RhDev.SharePoint.Common.DataAccess.Repository;
using RhDev.SharePoint.Common.DataAccess.Repository.Entities;
using RhDev.SharePoint.Common.DataAccess.SharePoint.Repository;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint
{
    public class DayOffProvider : IDayOffProvider
    {
        private readonly ICommonRepositoryFactory commonRepositoryFactory;

        public DayOffProvider(ICommonRepositoryFactory commonRepositoryFactory)
        {
            this.commonRepositoryFactory = commonRepositoryFactory;
        }

        public int GetDayOffsCountFromDateRange(CentralClock from, CentralClock to, IList<DayOfWeek> exceptList = null, int lcid = 1029, string dayOffProviderUrl = null)
        {
            var count = 0;

            var fromDate = from.ExportDateTime;
            var toDate = to.ExportDateTime;

            for (var date = fromDate; date <= toDate; date = date.AddDays(1))
            {
                if (!Equals(null, exceptList) && exceptList.Contains(date.DayOfWeek))
                    continue;

                if (IsDayOff(CentralClock.FillFromDateTime(date), lcid, dayOffProviderUrl))
                    count++;
            }

            return count;
        }

        public bool IsDayOff(CentralClock clock, int lcid = 1029, string dayOffProviderUrl = null)
        {
            var date = clock.ExportDateTime;

            var repo = commonRepositoryFactory.GetDayOffRepository(dayOffProviderUrl);

            var allDayOffs = repo.GetDayOffsByLcid(lcid);

            var datePublicHolidays = allDayOffs.Where(x => x.Date.Day == date.Day && x.Date.Month == date.Month).ToList();

            if (datePublicHolidays.Count == 0) return false;

            foreach (var datePublicHoliday in datePublicHolidays)
            {
                if (datePublicHoliday.Repeate)
                    return true;

                if (datePublicHoliday.Date.Year == date.Year)
                    return true;
            }

            return false;
        }
    }
}

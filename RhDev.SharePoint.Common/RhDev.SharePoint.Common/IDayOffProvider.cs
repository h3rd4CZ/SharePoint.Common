using RhDev.SharePoint.Common.Caching.Composition;
using System;
using System.Collections.Generic;

namespace RhDev.SharePoint.Common
{
    public interface IDayOffProvider : IAutoRegisteredService
    {
        bool IsDayOff(CentralClock clock, int lcid = 1029, string dayOffProviderUrl = null);
        int GetDayOffsCountFromDateRange(CentralClock from, CentralClock to, IList<DayOfWeek> exceptList, int lcid, string dayOffProviderUrl = null);
    }
}

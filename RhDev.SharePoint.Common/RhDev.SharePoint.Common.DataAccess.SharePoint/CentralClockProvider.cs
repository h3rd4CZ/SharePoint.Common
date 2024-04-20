using System;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint
{
    public class CentralClockProvider : ICentralClockProvider
    {
        public CentralClockProvider()
        {

        }

        public CentralClock Now()
        {
            var now = DateTime.Now;

            var clock = new CentralClock();
            clock.FillFrom(now);

            return clock;
        }

        public static ICentralClockProvider Get => new CentralClockProvider();
    }
}

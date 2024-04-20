using System;
using System.Runtime.InteropServices;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Logging
{
    public class CorrelationId
    {
        [DllImport("advapi32.dll")]
        public static extern uint EventActivityIdControl(uint controlCode, ref Guid activityId);
        public const uint EVENT_ACTIVITY_CTRL_GET_ID = 1;
        public static Guid GetCurrentCorrelationToken()
        {
            Guid g = Guid.Empty;
            EventActivityIdControl(EVENT_ACTIVITY_CTRL_GET_ID, ref g);
            return g;
        }
    }
}

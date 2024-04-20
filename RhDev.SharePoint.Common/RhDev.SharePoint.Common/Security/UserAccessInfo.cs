using System.Collections.Generic;

namespace RhDev.SharePoint.Common.Security
{
    public class UserAccessInfo
    {
        public bool HasAccess { get; set; }

        public IList<string> Groups { get; set; }
    }
}

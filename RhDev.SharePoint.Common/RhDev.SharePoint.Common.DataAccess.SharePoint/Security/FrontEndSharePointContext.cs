using Microsoft.SharePoint;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Security
{
    public class FrontEndSharePointContext : ISharePointContext
    {
        public SPContext Instance
        {
            get { return SPContext.Current; }
        }
    }
}

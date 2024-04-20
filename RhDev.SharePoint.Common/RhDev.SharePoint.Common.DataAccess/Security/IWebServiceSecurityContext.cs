using RhDev.SharePoint.Common.Caching.Composition;

namespace RhDev.SharePoint.Common.DataAccess.Security
{
    public interface IWebServiceSecurityContext : IAutoRegisteredService
    {
        bool IsCurrentUserInGroup(string groupName);
    }
}

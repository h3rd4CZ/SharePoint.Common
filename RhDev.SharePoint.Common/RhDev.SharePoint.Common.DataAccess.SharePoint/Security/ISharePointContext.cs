using Microsoft.SharePoint;
using RhDev.SharePoint.Common.Caching.Composition;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Security
{
    public interface ISharePointContext : IAutoRegisteredService
    {
        SPContext Instance { get; }
    }
}

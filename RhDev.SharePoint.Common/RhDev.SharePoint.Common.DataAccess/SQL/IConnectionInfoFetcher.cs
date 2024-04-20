using RhDev.SharePoint.Common.Caching.Composition;

namespace RhDev.SharePoint.Common.DataAccess.SQL
{
    public interface IConnectionInfoFetcher : IAutoRegisteredService
    {
        string GetConnectionInfo(string parameter = default);
    }
}

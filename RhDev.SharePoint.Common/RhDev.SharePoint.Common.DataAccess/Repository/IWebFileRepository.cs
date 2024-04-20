using RhDev.SharePoint.Common.DataAccess.Repository.Entities;
using RhDev.SharePoint.Common.Caching.Composition;

namespace RhDev.SharePoint.Common.DataAccess.Repository
{
    public interface IWebFileRepository : IAutoRegisteredService
    {
        SolutionFileInfo GetWebFile(string webFileAbsoluteUrl);
    }
}

using RhDev.SharePoint.Common.Caching.Composition;

namespace RhDev.SharePoint.Common.DataAccess.Security
{
    public interface IDocumentPermissionsRepositoryFactory : IAutoRegisteredService
    {
        IDocumentPermissionsRepository CreateDocumentPermissionsRepository(SectionDesignation sectionDesignation, string listUrl);
    }
}

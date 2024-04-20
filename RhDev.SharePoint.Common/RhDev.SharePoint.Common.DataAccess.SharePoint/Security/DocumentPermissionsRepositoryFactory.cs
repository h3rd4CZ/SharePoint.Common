using RhDev.SharePoint.Common.DataAccess.Security;
using RhDev.SharePoint.Common.DataAccess.SharePoint.Security.Permission;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Security
{
    public class DocumentPermissionsRepositoryFactory : IDocumentPermissionsRepositoryFactory
    {
        public IDocumentPermissionsRepository CreateDocumentPermissionsRepository(SectionDesignation sectionDesignation, string listUrl)
        {
            return new DocumentPermissionsRepository(sectionDesignation.GetAddress(), listUrl);
        }
    }

}

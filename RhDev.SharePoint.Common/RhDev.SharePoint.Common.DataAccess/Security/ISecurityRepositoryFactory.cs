using RhDev.SharePoint.Common.Caching.Composition;

namespace RhDev.SharePoint.Common.DataAccess.Security
{
    public interface ISecurityRepositoryFactory : IAutoRegisteredService
    {
        IExternalCredentialsRepository CreateExternalCredentialsRepository();
        IUserInfoRepository CreateUserInfoRepository(SectionDesignation sectionDesignation);
    }
}

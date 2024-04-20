using RhDev.SharePoint.Common.Configuration;
using RhDev.SharePoint.Common.DataAccess.Security;
using RhDev.SharePoint.Common.Logging;
using RhDev.SharePoint.Common.Security;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Security
{
    public class SecurityRepositoryFactory : ISecurityRepositoryFactory
    {
        private readonly FarmConfiguration farmConfiguration;
        private readonly ITraceLogger traceLogger;

        public SecurityRepositoryFactory(FarmConfiguration farmConfiguration, ITraceLogger traceLogger)
        {
            this.farmConfiguration = farmConfiguration;
            this.traceLogger = traceLogger;
        }
        
        public IUserInfoRepository CreateUserInfoRepository(SectionDesignation sectionDesignation)
        {
            return new UserInfoRepository(sectionDesignation.GetAddress(), traceLogger);
        }

        public IExternalCredentialsRepository CreateExternalCredentialsRepository()
        {
            return new ExternalCredentialsRepository(farmConfiguration.AppSiteUrl);
        }
    }
}

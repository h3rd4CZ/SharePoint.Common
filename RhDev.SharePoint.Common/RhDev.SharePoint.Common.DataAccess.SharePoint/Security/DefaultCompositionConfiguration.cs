using RhDev.SharePoint.Common.Caching.Composition;
using RhDev.SharePoint.Common.DataAccess.Security;
using RhDev.SharePoint.Common.Security;
using StructureMap;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Security
{
    public class DefaultCompositionConfiguration : ConventionConfigurationBase
    {
        public DefaultCompositionConfiguration(ConfigurationExpression configuration, Container container)
            : base(configuration, container)
        {

        }

        public override void Apply()
        {
            base.Apply();

            For<ISharePointContext>()
                .Use<FrontEndSharePointContext>();

            For<ISecurityContext>()
                .Use<FrontEndSecurityContext>();

            For<ISecurityRepositoryFactory>()
                .Use<SecurityRepositoryFactory>();
            

        }
    }
}

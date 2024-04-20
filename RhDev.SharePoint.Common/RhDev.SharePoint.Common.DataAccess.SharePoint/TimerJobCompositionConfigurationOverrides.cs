using RhDev.SharePoint.Common.Caching.Composition;
using RhDev.SharePoint.Common.Configuration;
using RhDev.SharePoint.Common.DataAccess.SharePoint.Configuration;
using RhDev.SharePoint.Common.DataAccess.SharePoint.Security;
using RhDev.SharePoint.Common.Security;
using StructureMap;

namespace  RhDev.SharePoint.Common.DataAccess.SharePoint
{
    public class TimerJobCompositionConfigurationOverrides : CompositionConfigurationBase
    {
        public TimerJobCompositionConfigurationOverrides(ConfigurationExpression configuration, Container container)
            : base(configuration, container)
        {
        }

        public override void Apply()
        {
            For<IConfigurationCacheStrategy>().Use<WithoutCacheConfigurationCacheStrategy>();

            For<ISecurityContext>().Use<TimerJobSecurityContext>();
        }
    }
}

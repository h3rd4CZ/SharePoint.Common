using RhDev.SharePoint.Common.Caching.Composition;
using RhDev.SharePoint.Common.Security;
using StructureMap;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Security
{
    public class TimerJobCompositionConfigurationOverrides : CompositionConfigurationBase
    {
        public TimerJobCompositionConfigurationOverrides(ConfigurationExpression configuration, Container container)
            : base(configuration, container)
        {
        }

        public override void Apply()
        {
            For<ISecurityContext>().Use<TimerJobSecurityContext>();
        }
    }
}

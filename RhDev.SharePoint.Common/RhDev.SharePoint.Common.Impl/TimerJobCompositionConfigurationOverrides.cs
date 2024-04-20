using RhDev.SharePoint.Common.Caching.Composition;
using RhDev.SharePoint.Common.Configuration;
using StructureMap;

namespace RhDev.SharePoint.Common.Impl
{
    public class TimerJobCompositionConfigurationOverrides : CompositionConfigurationBase
    {
        public TimerJobCompositionConfigurationOverrides(ConfigurationExpression configuration, Container container)
            : base(configuration, container)
        {
        }

        public override void Apply()
        {
        }
    }
}

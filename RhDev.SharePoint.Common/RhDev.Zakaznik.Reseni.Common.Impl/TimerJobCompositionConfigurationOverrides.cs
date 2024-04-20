using RhDev.SharePoint.Common.Caching.Composition;
using StructureMap;

namespace $ext_safeprojectname$.Common.Impl
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

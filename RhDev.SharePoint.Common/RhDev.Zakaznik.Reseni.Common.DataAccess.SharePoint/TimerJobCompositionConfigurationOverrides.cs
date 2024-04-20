using RhDev.SharePoint.Common.Caching.Composition;
using RhDev.SharePoint.Common.Logging;
using $ext_safeprojectname$.Common.DataAccess.SharePoint.Tracing;
using StructureMap;

namespace $ext_safeprojectname$.Common.DataAccess.SharePoint
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

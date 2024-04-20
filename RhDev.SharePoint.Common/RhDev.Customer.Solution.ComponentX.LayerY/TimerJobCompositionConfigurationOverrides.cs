using RhDev.Customer.Solution.ComponentX.LayerY.Services;
using RhDev.SharePoint.Common.Caching.Composition;
using StructureMap;

namespace RhDev.Customer.Solution.ComponentX.LayerY
{
    public class TimerJobCompositionConfigurationOverrides : CompositionConfigurationBase
    {
        public TimerJobCompositionConfigurationOverrides(ConfigurationExpression configuration, Container container)
            : base(configuration, container)
        {
        }

        public override void Apply()
        {
            ConfigureAsSingleton<IFoo>();
        }
    }
}

using RhDev.SharePoint.Common.Caching.Composition;
using RhDev.SharePoint.Common.DataAccess.SQL;
using StructureMap;

namespace RhDev.Customer.Solution.ComponentX.LayerY
{
    public class DefaultCompositionConfiguration : ConventionConfigurationBase
    {
        public DefaultCompositionConfiguration(ConfigurationExpression configuration, Container container)
                : base(configuration, container)
        {
            
        }
    }
}

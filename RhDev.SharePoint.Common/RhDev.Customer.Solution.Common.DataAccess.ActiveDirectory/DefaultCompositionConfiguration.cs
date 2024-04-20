using RhDev.Customer.Solution.Common.DataAccess.ActiveDirectory.Services;
using RhDev.SharePoint.Common.Caching.Composition;
using StructureMap;

namespace RhDev.Customer.Solution.Common.DataAccess.ActiveDirectory
{
    public class DefaultCompositionConfiguration : ConventionConfigurationBase
    {
        public DefaultCompositionConfiguration(ConfigurationExpression configuration, Container container)
                : base(configuration, container)
        {
            For<SharePoint.Common.DataAccess.SharePoint.Configuration.Objects.GlobalConfiguration>().Use<GlobalConfiguration>();
        }
    }
}

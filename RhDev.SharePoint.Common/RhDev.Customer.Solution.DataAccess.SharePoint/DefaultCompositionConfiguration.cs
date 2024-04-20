using RhDev.Customer.Solution.Common.DataAccess.SharePoint.Services;
using RhDev.SharePoint.Common.Caching.Composition;
using RhDev.SharePoint.Common.Configuration;
using RhDev.SharePoint.Common.Logging;
using StructureMap;

namespace RhDev.Customer.Solution.Common.DataAccess.SharePoint
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

            For<IConfigurationCacheStrategy>().Use<WithoutCacheConfigurationCacheStrategy>();

            For<ITraceLogger>().Use<SolutionSharePointTraceLogger>();

            For<FarmConfiguration>().Use<FarmConfiguration>().Ctor<IConfigurationDataSource>().Is<SolutionFarmPropertiesDataSource>();
        }
    }
}

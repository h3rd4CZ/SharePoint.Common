using RhDev.SharePoint.Common.Caching.Composition;
using RhDev.SharePoint.Common.Configuration;
using RhDev.SharePoint.Common.Logging;
using $ext_safeprojectname$.Common.DataAccess.SharePoint.Configuration.Objects;
using $ext_safeprojectname$.Common.DataAccess.SharePoint.Tracing;
using StructureMap;

namespace $ext_safeprojectname$.Common.DataAccess.SharePoint
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

            For<RhDev.SharePoint.Common.DataAccess.SharePoint.Configuration.Objects.GlobalConfiguration>().Use<GlobalConfiguration>();

            For<FarmConfiguration>().Use<FarmConfiguration>().Ctor<IConfigurationDataSource>().Is<SolutionFarmPropertiesDataSource>();
        }
    }
}

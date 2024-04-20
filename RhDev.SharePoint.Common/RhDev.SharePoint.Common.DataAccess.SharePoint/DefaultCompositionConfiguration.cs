using RhDev.SharePoint.Common.Caching.Composition;
using RhDev.SharePoint.Common.Configuration;
using RhDev.SharePoint.Common.DataAccess.SharePoint.Configuration;
using RhDev.SharePoint.Common.DataAccess.SharePoint.Configuration.Objects;
using RhDev.SharePoint.Common.DataAccess.SharePoint.Logging;
using RhDev.SharePoint.Common.DataAccess.SharePoint.Security;
using RhDev.SharePoint.Common.Logging;
using RhDev.SharePoint.Common.Security;
using StructureMap;
using StructureMap.Web;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint
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

            ConfigureLogging();
            ConfigureConfiguration();

            For<ISecurityContext>().Use<FrontEndSecurityContext>();
        }

        private void ConfigureLogging()
        {
            For<ITraceLogger>()
                .Use<SharePointTraceLogger>();
        }

        private void ConfigureConfiguration()
        {
            For<IConfigurationDataSource>()
                .Use<ListConfigurationDataSource>();
                        
            //Opt -out configuration caching in client solutions might be switched off by uncommenting last line;
            For<IConfigurationDataSource>()
                .HybridHttpOrThreadLocalScoped()
                .DecorateAllWith((ctx, dataSource)
                        => new ConfigurationDataSourceWrapper(dataSource, ctx.GetInstance<IConfigurationCacheStrategy>(), ctx.GetInstance<ITraceLogger>()));

            For<IConfigurationCacheStrategy>().Use<WithCacheConfigurationCacheStrategy>();

            For<ConfigurationObjectConstructor>().Use<ConfigurationObjectConstructor>((configType, webUrl) =>
            {
                var config = Container.GetInstance(configType);
                ConfigurationObject co = config as ConfigurationObject;

                if (string.IsNullOrEmpty(webUrl)) return co;

                co.OnWeb(webUrl);

                return co;
            });

            For<FarmConfiguration>().Use<FarmConfiguration>()
                .Ctor<IConfigurationDataSource>().Is<FarmPropertiesDataSource>();
        }
    }
}

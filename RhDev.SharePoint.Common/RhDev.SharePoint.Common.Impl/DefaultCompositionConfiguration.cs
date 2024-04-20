using RhDev.SharePoint.Common.Caching;
using RhDev.SharePoint.Common.Caching.Composition;
using RhDev.SharePoint.Common.Configuration;
using RhDev.SharePoint.Common.DataAccess.Concurrent;
using RhDev.SharePoint.Common.Impl.Caching;
using RhDev.SharePoint.Common.Impl.Configuration;
using StructureMap;
using StructureMap.Web;

namespace RhDev.SharePoint.Common.Impl
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
                        
            For(typeof(ICache<>)).Singleton();
            For(typeof(ICache<>)).Use(typeof(DictionaryCache<>));

            For(typeof(IConfigurationManager<>)).Use(typeof(ConfigurationManager<>));

            For<IConcurrentDataAccessRepository>().Singleton();
        }
    }
}

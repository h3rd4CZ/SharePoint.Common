using RhDev.SharePoint.Common.Caching.Composition;
using RhDev.SharePoint.Common.DataAccess.SQL;
using StructureMap;

namespace RhDev.SharePoint.Common.DataAccess.Sql
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

            ConfigureAsSingleton<IConnectionInfoFetcher>();

            For(typeof(IDatabaseAccessRepositoryFactory<>)).Use(typeof(DatabaseAccessRepositoryFactory<>));
        }
    }
}

using RhDev.SharePoint.Common.Caching.Composition;
using StructureMap;

namespace $ext_safeprojectname$.Common.Impl
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
        }
    }
}

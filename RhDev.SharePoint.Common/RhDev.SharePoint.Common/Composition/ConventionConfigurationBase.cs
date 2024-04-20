using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StructureMap;

namespace RhDev.SharePoint.Common.Caching.Composition
{
    public class ConventionConfigurationBase : CompositionConfigurationBase
    {
        protected ConventionConfigurationBase(ConfigurationExpression configuration, Container container)
            : base(configuration, container)
        {

        }

        public override void Apply()
        {
            Configuration.Scan(scanner =>
            {
                scanner.Assembly(GetType().Assembly);
                scanner.Convention<ServiceAutoRegistrationConvention>();
            });
        }
    }
}
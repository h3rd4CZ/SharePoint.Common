using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StructureMap;
using StructureMap.Configuration.DSL.Expressions;

namespace RhDev.SharePoint.Common.Caching.Composition
{
    public abstract class CompositionConfigurationBase
    {
        protected Container Container { get; private set; }

        protected ConfigurationExpression Configuration { get; private set; }

        protected CompositionConfigurationBase(ConfigurationExpression configuration, Container container)
        {
            Container = container;
            Configuration = configuration;
        }

        public abstract void Apply();

        protected void ConfigureAsTransient<TService>()
        {
            For<TService>().Transient();
        }

        protected void ConfigureAsSingleton<TService>()
        {
            For<TService>().Singleton();
        }

        protected CreatePluginFamilyExpression<TService> For<TService>()
        {
            return Configuration.For<TService>();
        }

        protected GenericFamilyExpression For(Type pluginType)
        {
            return Configuration.For(pluginType);
        }
    }
}

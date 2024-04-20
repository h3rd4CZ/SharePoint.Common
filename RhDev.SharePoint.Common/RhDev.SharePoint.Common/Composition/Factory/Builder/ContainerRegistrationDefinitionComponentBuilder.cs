using RhDev.SharePoint.Common.Composition.Factory.Definitions;
using RhDev.SharePoint.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhDev.SharePoint.Common.Composition.Factory.Builder
{
    public class ContainerRegistrationDefinitionComponentBuilder : IContainerRegistrationDefinitionBuilder<ContainerRegistrationComponentDefinition>
    {
        private readonly string componentName;
        private IList<ContainerRegistrationLayerDefinition> layers;
        private ContainerRegistrationDefinitionComponentBuilder(string cn)
        {
            Guard.StringNotNullOrWhiteSpace(cn, nameof(cn));
            this.componentName = cn;
        }

        public ContainerRegistrationDefinitionComponentBuilder WithLayers(IList<ContainerRegistrationLayerDefinition> layers)
        {
            Guard.NotNull(layers, nameof(layers));

            this.layers = layers;

            return this;
        }

        public static ContainerRegistrationDefinitionComponentBuilder Get(string componentName) => new ContainerRegistrationDefinitionComponentBuilder(componentName);

        public ContainerRegistrationComponentDefinition Build()
        {
            return new ContainerRegistrationComponentDefinition
            {
                ComponentName = this.componentName,
                Layers = this.layers
            };
        }
    }
}

using RhDev.SharePoint.Common.Composition.Factory.Definitions;
using RhDev.SharePoint.Common.Utils;
using System.Collections.Generic;

namespace RhDev.SharePoint.Common.Composition.Factory.Builder
{
    public class ContainerRegistrationDefinitionBuilder : IContainerRegistrationDefinitionBuilder<ContainerRegistrationDefinition>
    {
        private readonly string solutionNameSpacePrefix;
        private IList<ContainerRegistrationComponentDefinition> components;
        private ContainerRegistrationDefinitionBuilder(string snp)
        {
            Guard.StringNotNullOrWhiteSpace(snp, nameof(snp));
            this.solutionNameSpacePrefix = snp;
        }

        public ContainerRegistrationDefinitionBuilder WithComponents(IList<ContainerRegistrationComponentDefinition> components)
        {
            Guard.NotNull(components, nameof(components));

            this.components = components;

            return this;
        }

        public static ContainerRegistrationDefinitionBuilder Get(string solutionPrefix) => new ContainerRegistrationDefinitionBuilder(solutionPrefix);

        public ContainerRegistrationDefinition Build()
        {
            return new ContainerRegistrationDefinition(this.solutionNameSpacePrefix, this.components);
        }
    }
}

using RhDev.SharePoint.Common.Utils;
using System;
using System.Collections.Generic;

namespace RhDev.SharePoint.Common.Composition.Factory.Definitions
{
    public class ContainerRegistrationDefinition : IContainerRegistrationDefinition
    {
        public const string DEFAULT_COMPOSITION_CONFIGURATION_TYPE_NAME = "DefaultCompositionConfiguration";

        public const string TIMER_JOB_COMPOSITION_CONFIGURATION_OVERRIDES_TYPE_NAME = "TimerJobCompositionConfigurationOverrides";

        public const string ASSEMBLY_FULL_NAME_FORMAT = "{0}, Version={1}, Culture=neutral, PublicKeyToken={2}";

        private readonly string solutionNamespacePrefix;
        private readonly IList<ContainerRegistrationComponentDefinition> componentDefinitions;

        public static ContainerRegistrationDefinition Empty => new ContainerRegistrationDefinition("Empty", new List<ContainerRegistrationComponentDefinition> { });

        public ContainerRegistrationDefinition(string solutionNamespacePrefix, IList<ContainerRegistrationComponentDefinition> componentDefinitions)
        {
            Guard.StringNotNullOrWhiteSpace(solutionNamespacePrefix, nameof(solutionNamespacePrefix));
            Guard.NotNull(componentDefinitions, nameof(componentDefinitions));

            this.solutionNamespacePrefix = solutionNamespacePrefix;
            this.componentDefinitions = componentDefinitions;
        }

        public IEnumerable<Type> BuildTypes(bool includeBackend)
        {
            foreach (var component in componentDefinitions)
            {
                if (!component.Isvalid()) continue;

                foreach (var layer in component.Layers)
                {
                    if (!layer.IsValid()) continue;

                    var assemblyName = $"{solutionNamespacePrefix}.{component.ComponentName}.{layer.LayerName}";

                    if(layer.HasFrontendRegistration)
                    {
                        Type frontendType = 
                            Type.GetType(string.Format(ASSEMBLY_FULL_NAME_FORMAT, $"{assemblyName}.{DEFAULT_COMPOSITION_CONFIGURATION_TYPE_NAME}, {assemblyName}", layer.Version, layer.PKT), false);

                        ValidateBuildType(frontendType, component, layer);

                        yield return frontendType;
                    }

                    if (includeBackend && layer.HasBackendRegistration)
                    {
                        Type backendType =
                            Type.GetType(string.Format(ASSEMBLY_FULL_NAME_FORMAT, $"{assemblyName}.{TIMER_JOB_COMPOSITION_CONFIGURATION_OVERRIDES_TYPE_NAME}, {assemblyName}", layer.Version, layer.PKT), false);

                        ValidateBuildType(backendType, component, layer);

                        yield return backendType;
                    }
                }
            }
        }

        private void ValidateBuildType(Type type, ContainerRegistrationComponentDefinition componentDefinition, ContainerRegistrationLayerDefinition layerDefinition)
        {
            if (!Equals(null, type)) return;

            Guard.NotNull(componentDefinition, nameof(componentDefinition));
            Guard.NotNull(layerDefinition, nameof(layerDefinition));

            throw new InvalidOperationException($"Type definition resolving failed for component : {componentDefinition}, layer : {layerDefinition}");
        }
    }
}

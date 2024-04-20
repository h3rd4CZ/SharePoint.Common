using RhDev.SharePoint.Common.Caching.Composition;
using RhDev.SharePoint.Common.Composition.Factory.Definitions;
using RhDev.SharePoint.Common.Configuration;
using RhDev.SharePoint.Common.Logging;
using RhDev.SharePoint.Common.Utils;
using StructureMap;
using StructureMap.Configuration.DSL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RhDev.SharePoint.Common.Composition.Factory
{
    public static class ApplicationContainerFactory
    {
        public static IApplicationContainerSetup Create(ContainerRegistrationDefinition containerDefinition, Action<Container> postBuildActionsFrontend = null, Action<Container> postBuildActionsBackend = null)
        {
            Guard.NotNull(containerDefinition, nameof(containerDefinition));

            var internalTypesDefinition = GetInternalDefinition;
            var solutionTypesDefinition = containerDefinition;

            var internalTypesFrontendDefinition = internalTypesDefinition.BuildTypes(false).ToList();
            var internalTypesBackendDefinition = internalTypesDefinition.BuildTypes(true).ToList();

            var solutionTypesFrontendDefinition = solutionTypesDefinition.BuildTypes(false).ToList();
            var solutionTypesBackendDefinition = solutionTypesDefinition.BuildTypes(true).ToList();

            internalTypesFrontendDefinition.AddRange(solutionTypesFrontendDefinition);
            internalTypesBackendDefinition.AddRange(solutionTypesBackendDefinition);

            var frontendContainer = BuildContainerForTypes(internalTypesFrontendDefinition);
            var backendContainer = BuildContainerForTypes(internalTypesBackendDefinition);

            var frontendApplicationContainer = new ApplicationContainer(frontendContainer);
            var backendApplicationContainer = new ApplicationContainer(backendContainer);

            if (!Equals(null, postBuildActionsFrontend)) postBuildActionsFrontend(frontendContainer);
            if (!Equals(null, postBuildActionsBackend)) postBuildActionsBackend(backendContainer);

            return new ApplicationContainerSetup(frontendApplicationContainer, backendApplicationContainer);
        }

        private static ContainerRegistrationDefinition GetInternalDefinition => CompositionDefinition.GetDefinition();

        private static Container BuildContainerForTypes(IList<Type> types, Action<Container> postBuildAction = null)
        {
            var container = new Container();

            container.Configure(c => ApplyConfiguration(c, container, types));

            if (!Equals(null, postBuildAction)) postBuildAction(container);

            return container;
        }

        private static void MatchPropertyConvention(SetterConvention convention)
        {
            convention.TypeMatches(t => typeof(IService).IsAssignableFrom(t) ||
                                        typeof(IService[]).IsAssignableFrom(t) ||
                                        typeof(ConfigurationObject).IsAssignableFrom(t));
        }



        private static void ApplyConfiguration(ConfigurationExpression configuration, Container container, IEnumerable<Type> configurationTypes)
        {
            foreach (Type configurationType in configurationTypes)
            {
                var compositionConfiguration =
                    (CompositionConfigurationBase)Activator.CreateInstance(configurationType, configuration, container);

                compositionConfiguration.Apply();
            }

            configuration.Policies.SetAllProperties(MatchPropertyConvention);
        }
    }
}

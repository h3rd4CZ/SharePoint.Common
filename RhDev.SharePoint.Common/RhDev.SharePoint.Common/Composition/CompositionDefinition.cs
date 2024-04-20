using RhDev.SharePoint.Common.Composition.Factory.Builder;
using RhDev.SharePoint.Common.Composition.Factory.Definitions;
using System.Collections.Generic;

namespace RhDev.SharePoint.Common.Composition
{
    public static class CompositionDefinition
    {
        public static ContainerRegistrationDefinition GetDefinition() => ContainerRegistrationDefinitionBuilder
                .Get(Constants.SOLUTION_PREFIX)
                .WithComponents(new List<ContainerRegistrationComponentDefinition>
                {
                    ContainerRegistrationDefinitionComponentBuilder.Get(Constants.COMPONENT_COMMON_NAME)
                        .WithLayers(
                            new List<ContainerRegistrationLayerDefinition>{
                                ContainerRegistrationDefinitionLayerBuilder.GetNativeDataAccessSharePointLayer()
                                .WithFrontendAndBackendRegistrations()
                                .WithDefaultRhDevPKTAndVersion().Build(),
                                 ContainerRegistrationDefinitionLayerBuilder.Get("DataAccess.Sql")
                                .WithFrontendRegistrations()
                               .WithDefaultRhDevPKTAndVersion().Build(),
                                ContainerRegistrationDefinitionLayerBuilder.GetNativeImplementationLayer()
                                .WithFrontendAndBackendRegistrations()
                                .WithDefaultRhDevPKTAndVersion().Build(),
                        }).Build()
                }).Build();
    }
}

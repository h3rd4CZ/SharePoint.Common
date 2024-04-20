using RhDev.SharePoint.Common;
using RhDev.SharePoint.Common.Composition.Factory.Builder;
using RhDev.SharePoint.Common.Composition.Factory.Definitions;
using System.Collections.Generic;

namespace $ext_safeprojectname$.Common.Setup
{
    public class CompositionDefinition
    {
        public static ContainerRegistrationDefinition GetDefinition() => ContainerRegistrationDefinitionBuilder
                .Get(Const.SOLUTION_PREFIX)
                .WithComponents(new List<ContainerRegistrationComponentDefinition>
                {
                    ContainerRegistrationDefinitionComponentBuilder.Get(Constants.COMPONENT_COMMON_NAME)
                        .WithLayers(
                            new List<ContainerRegistrationLayerDefinition>{
                                ContainerRegistrationDefinitionLayerBuilder.GetNativeDataAccessSharePointLayer()
                                .WithFrontendAndBackendRegistrations()
                                .WithDefaultRhDevPKTAndVersion().Build(),
                                ContainerRegistrationDefinitionLayerBuilder.GetNativeImplementationLayer()
                                .WithFrontendAndBackendRegistrations()
                                .WithDefaultRhDevPKTAndVersion().Build(),
                        }).Build()
                }).Build();
    }
}

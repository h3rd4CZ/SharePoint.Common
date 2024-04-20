using RhDev.SharePoint.Common;
using RhDev.SharePoint.Common.Composition.Factory.Builder;
using RhDev.SharePoint.Common.Composition.Factory.Definitions;
using System.Collections.Generic;

namespace RhDev.Customer.Solution.Common.DataAccess.SharePoint
{
    public class Const
    {
            public static ContainerRegistrationDefinition GetClientSolutionContainerRegistration() =>
            ContainerRegistrationDefinitionBuilder
                .Get("RhDev.Customer.Solution")
                .WithComponents(new List<ContainerRegistrationComponentDefinition>
                {
                    ContainerRegistrationDefinitionComponentBuilder.Get(Constants.COMPONENT_COMMON_NAME)
                        .WithLayers(
                            new List<ContainerRegistrationLayerDefinition>{
                                ContainerRegistrationDefinitionLayerBuilder.GetNativeDataAccessSharePointLayer()
                                .WithFrontendAndBackendRegistrations()
                                .WithDefaultRhDevPKTAndVersion()
                                .Build(),
                                ContainerRegistrationDefinitionLayerBuilder.Get("DataAccess.ActiveDirectory")
                                .WithFrontendRegistrations()
                                .WithDefaultRhDevPKTAndVersion()
                                .Build()
                        }).Build(),
                    ContainerRegistrationDefinitionComponentBuilder.Get("ComponentX")
                        .WithLayers(
                            new List<ContainerRegistrationLayerDefinition>{
                                ContainerRegistrationDefinitionLayerBuilder.Get("LayerY")
                                .WithFrontendAndBackendRegistrations()
                                .WithDefaultRhDevPKTAndVersion()
                                .Build()
                        }).Build()
                }).Build();
    }
}

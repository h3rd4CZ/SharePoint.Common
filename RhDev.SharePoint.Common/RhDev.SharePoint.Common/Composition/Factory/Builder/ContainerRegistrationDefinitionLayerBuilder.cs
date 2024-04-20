using RhDev.SharePoint.Common.Composition.Factory.Definitions;
using RhDev.SharePoint.Common.Utils;
using System;
using System.Linq;
using System.Reflection;

namespace RhDev.SharePoint.Common.Composition.Factory.Builder
{
    public class ContainerRegistrationDefinitionLayerBuilder : IContainerRegistrationDefinitionBuilder<ContainerRegistrationLayerDefinition>
    {

        private const string LAYER_DATAACCESS_SHAREPOINT_NAME = "DataAccess.SharePoint";
        private const string LAYER_IMPLEMENTATION_NAME = "Impl";

        private string layerName;
        private bool hasFrontend;
        private bool hasBackend;
        private string pkt;
        private Version version;
        private ContainerRegistrationDefinitionLayerBuilder(string ln) => layerName = ln;

        private string PktGenerator => string.Join(string.Empty, Assembly.GetExecutingAssembly().GetName().GetPublicKeyToken().Select(t => t.ToString("x")));
        private Version defaultVersion = new Version(1, 0, 0, 0);
        public ContainerRegistrationDefinitionLayerBuilder WithFrontendAndBackendRegistrations()
        {
            this.hasFrontend = true;
            this.hasBackend = true;
            return this;
        }

        public ContainerRegistrationDefinitionLayerBuilder WithFrontendRegistrations()
        {
            this.hasFrontend = true;
            return this;
        }

        public ContainerRegistrationDefinitionLayerBuilder WithBackendRegistrations()
        {
            this.hasBackend = true;
            return this;
        }

        public ContainerRegistrationDefinitionLayerBuilder WithPKT(string pkt)
        {
            Guard.StringNotNullOrWhiteSpace(pkt, nameof(pkt));

            this.pkt = pkt;
            return this;
        }

        public ContainerRegistrationDefinitionLayerBuilder WithDefaultRhDevPKTAndVersion()
        {
            this.pkt = PktGenerator;
            this.version = defaultVersion;

            return this;
        }

        public ContainerRegistrationDefinitionLayerBuilder WithDefaultRhDevPKT()
        {
            this.pkt = PktGenerator;

            return this;
        }

        public ContainerRegistrationDefinitionLayerBuilder WithVersion(Version version)
        {
            Guard.NotNull(version, nameof(version));

            this.version = version;
            return this;
        }

        public ContainerRegistrationDefinitionLayerBuilder WithDefaultVersion()
        {
            this.version = defaultVersion;
            return this;
        }

        public static ContainerRegistrationDefinitionLayerBuilder GetNativeDataAccessSharePointLayer() => new ContainerRegistrationDefinitionLayerBuilder(LAYER_DATAACCESS_SHAREPOINT_NAME);
        public static ContainerRegistrationDefinitionLayerBuilder GetNativeImplementationLayer() => new ContainerRegistrationDefinitionLayerBuilder(LAYER_IMPLEMENTATION_NAME);
        public static ContainerRegistrationDefinitionLayerBuilder Get(string layerName) => new ContainerRegistrationDefinitionLayerBuilder(layerName);

        public ContainerRegistrationLayerDefinition Build()
        {
            return new ContainerRegistrationLayerDefinition()
            {
                LayerName = this.layerName,
                HasBackendRegistration = this.hasBackend,
                HasFrontendRegistration = this.hasFrontend,
                PKT = this.pkt,
                Version = this.version
            };
        }
    }
}

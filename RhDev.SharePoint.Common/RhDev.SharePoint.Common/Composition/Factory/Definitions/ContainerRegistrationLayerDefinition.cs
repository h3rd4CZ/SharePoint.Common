using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhDev.SharePoint.Common.Composition.Factory.Definitions
{
    public class ContainerRegistrationLayerDefinition
    {
        public string LayerName { get; set; }
        public bool HasFrontendRegistration { get; set; }
        public bool HasBackendRegistration { get; set; }
        /// <summary>
        /// Public key token
        /// </summary>
        public string PKT { get; set; }
        /// <summary>
        /// DLL Version
        /// </summary>
        public Version Version { get; set; }

        public bool IsValid() => !string.IsNullOrWhiteSpace(LayerName) && !string.IsNullOrWhiteSpace(PKT) && !Equals(null, Version);

        public override string ToString() => $"{LayerName},{PKT},{Version}";
    }
}

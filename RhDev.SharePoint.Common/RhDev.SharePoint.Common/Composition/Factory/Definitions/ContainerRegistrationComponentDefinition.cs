using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhDev.SharePoint.Common.Composition.Factory.Definitions
{
    public class ContainerRegistrationComponentDefinition
    {
        public string ComponentName { get; set; }
        public IList<ContainerRegistrationLayerDefinition> Layers { get; set; }

        public bool Isvalid() => !string.IsNullOrWhiteSpace(ComponentName) && !Equals(null, Layers) && Layers.Count > 0;

        public override string ToString() => $"{ComponentName}";
    }
}

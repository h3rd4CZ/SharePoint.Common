using RhDev.SharePoint.Common.Composition.Factory;
using RhDev.SharePoint.Common.Composition.Factory.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhDev.SharePoint.Common.Test.Setup
{
    public class RootSetup
    {
        public static IApplicationContainerSetup ContainerSetup =>
            ApplicationContainerFactory.Create(new ContainerRegistrationDefinition("Test.Solution", new List<ContainerRegistrationComponentDefinition> { }));
    }
}

using RhDev.SharePoint.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhDev.SharePoint.Common.Composition.Factory
{
    public class ApplicationContainerSetup : IApplicationContainerSetup
    {
        public IApplicationContainer Frontend { get; private set; }

        public IApplicationContainer Backend { get; private set; }

        public ApplicationContainerSetup(IApplicationContainer frontendContainer, IApplicationContainer backendContainer)
        {
            Guard.NotNull(frontendContainer, nameof(frontendContainer));
            Guard.NotNull(backendContainer, nameof(backendContainer));

            this.Frontend = frontendContainer;
            this.Backend = backendContainer;
        }
    }
}

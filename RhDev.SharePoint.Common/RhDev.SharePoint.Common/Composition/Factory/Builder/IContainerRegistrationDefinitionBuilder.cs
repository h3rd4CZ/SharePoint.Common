using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhDev.SharePoint.Common.Composition.Factory.Builder
{
    public interface IContainerRegistrationDefinitionBuilder<T>
    {
        T Build();
    }
}

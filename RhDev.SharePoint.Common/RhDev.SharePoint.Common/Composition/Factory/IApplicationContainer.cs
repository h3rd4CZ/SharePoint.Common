using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhDev.SharePoint.Common.Composition.Factory
{
    public interface IApplicationContainer
    {
        T GetInstance<T>();
        object GetInstance(Type type);
        void BuildUp(object target);
    }
}

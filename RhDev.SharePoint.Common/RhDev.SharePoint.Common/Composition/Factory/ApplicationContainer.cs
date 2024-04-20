using RhDev.SharePoint.Common.Utils;
using StructureMap;
using System;

namespace RhDev.SharePoint.Common.Composition.Factory
{
    public class ApplicationContainer : IApplicationContainer
    {
        private readonly IContainer container;

        public ApplicationContainer(IContainer container)
        {
            Guard.NotNull(container, nameof(container));

            this.container = container;
        }

        public void BuildUp(object target)
        {
            container.BuildUp(target);
        }

        public T GetInstance<T>() => container.GetInstance<T>();

        public object GetInstance(Type type)
        {
            Guard.NotNull(type, nameof(type));

            return container.GetInstance(type);
        }
    }
}

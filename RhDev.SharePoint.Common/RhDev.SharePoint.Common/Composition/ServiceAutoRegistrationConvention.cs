using System;
using System.Linq;
using StructureMap.Graph;
using StructureMap.Configuration.DSL;

namespace RhDev.SharePoint.Common.Caching.Composition
{
    public class ServiceAutoRegistrationConvention : IRegistrationConvention
    {

        private static readonly Type BaseServiceInterface = typeof(IService);
        private static readonly Type BaseAutoRegisteredServiceInterface = typeof(IAutoRegisteredService);

        public void Process(Type implementation, Registry registry)
        {
            if (!IsServiceImplementation(implementation))
                return;

            var serviceInterfaces = implementation.GetInterfaces().Where(IsNotBaseServiceInterface);

            foreach (Type serviceInterface in serviceInterfaces)
                RegisterImplementationForServiceInterface(registry, implementation, serviceInterface);
        }

        private static bool IsServiceImplementation(Type implementation)
        {
            return BaseAutoRegisteredServiceInterface.IsAssignableFrom(implementation) && !implementation.IsAbstract;
        }

        private static bool IsNotBaseServiceInterface(Type serviceInterface)
        {
            return serviceInterface != BaseServiceInterface && serviceInterface != BaseAutoRegisteredServiceInterface;
        }

        private static void RegisterImplementationForServiceInterface(Registry registry, Type implementation, Type serviceInterface)
        {
            registry.For(serviceInterface).Use(implementation);
        }

    }
}

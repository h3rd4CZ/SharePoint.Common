
using RhDev.SharePoint.Common.Composition.Factory;
using RhDev.SharePoint.Common.Composition.Factory.Definitions;

namespace RhDev.SharePoint.Common.Composition
{
    public static class CommonContainerRoot
    {
        private static readonly object SyncRoot = new object();

        private static IApplicationContainerSetup containerSetup;

        public static IApplicationContainerSetup Get { 
            get
            {
                lock (SyncRoot)
                {
                    return containerSetup ?? (containerSetup = ApplicationContainerFactory.Create(ContainerRegistrationDefinition.Empty));
                }
            } }        
    }
}

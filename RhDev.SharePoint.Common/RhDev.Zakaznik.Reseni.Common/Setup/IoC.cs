using RhDev.SharePoint.Common.Composition.Factory;

namespace $ext_safeprojectname$.Common.Setup
{
    public static class IoC
    {
        private static readonly object SyncRoot = new object();

        private static IApplicationContainerSetup containerSetup;

        public static IApplicationContainerSetup Get
        {
            get
            {
                lock (SyncRoot)
                {
                    return containerSetup ?? (containerSetup = ApplicationContainerFactory.Create(CompositionDefinition.GetDefinition()));
                }
            }
        }
    }
}

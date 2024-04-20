using Microsoft.SharePoint;
using RhDev.SharePoint.Common.DataAccess.SharePoint.Installation;
using RhDev.SharePoint.Common.SystemIntegrationTest.Setup;

namespace RhDev.SharePoint.Common.SystemIntegrationTest.FeatureInstallation
{
    public class FakeFeatureReceiver : FeatureReceiverBase<SPWeb, FakeFeatureInstallation>
    {
        public FakeFeatureReceiver() : base(o => RootSetup.ContainerSetup.Backend.BuildUp(o))
        {

        }
    }
}

using System;
using System.Runtime.InteropServices;
using Microsoft.SharePoint.Administration;
using RhDev.SharePoint.Common.Composition;
using RhDev.SharePoint.Common.DataAccess.SharePoint.Installation;

namespace RhDev.SharePoint.Features.Farm
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("e8f92fcf-b344-443b-b77d-ea8f6a36f4ac")]
    public class FarmEventReceiver : FeatureReceiverBase<SPWebService, TraceLoggerFeatureInstallation>
    {
        public FarmEventReceiver() : base(o => CommonContainerRoot.Get.Backend.BuildUp(o))
        {
        }
    }
}

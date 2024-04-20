using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using RhDev.SharePoint.Common.Composition;
using RhDev.SharePoint.Common.DataAccess.SharePoint.Installation;

namespace RhDev.SharePoint.Features.Web
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("2a36a380-a362-43a9-adcc-79c3d0a09ec0")]
    public class WebEventReceiver : FeatureReceiverBase<SPWeb, CommonWebFeatureInstallation>
    {
        public WebEventReceiver() : base(o => CommonContainerRoot.Get.Backend.BuildUp(o))
        {
        }
    }
}

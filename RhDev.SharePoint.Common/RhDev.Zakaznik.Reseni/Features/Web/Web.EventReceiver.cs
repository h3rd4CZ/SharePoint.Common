using System;
using System.Runtime.InteropServices;
using Microsoft.SharePoint;
using RhDev.SharePoint.Common.DataAccess.SharePoint.Installation;
using $ext_safeprojectname$.Common.DataAccess.SharePoint.Installation;
using $ext_safeprojectname$.Common.Setup;

namespace $ext_safeprojectname$.Features.Web
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("$guid7$")]
    public class WebEventReceiver : FeatureReceiverBase<SPWeb, WebInstallation>
    {
        public WebEventReceiver() : base(o => IoC.Get.Frontend.BuildUp(o))
        {
        }
    }
}

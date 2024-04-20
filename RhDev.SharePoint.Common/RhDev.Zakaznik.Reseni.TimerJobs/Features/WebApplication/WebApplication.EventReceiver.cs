using System;
using System.Runtime.InteropServices;
using Microsoft.SharePoint.Administration;
using RhDev.SharePoint.Common.DataAccess.SharePoint.Installation;
using $ext_safeprojectname$.Common.DataAccess.SharePoint.Installation.Jobs;
using $ext_safeprojectname$.Common.Setup;

namespace $ext_safeprojectname$.TimerJobs.Features.WebApplication
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("$guid3$")]
    public class WebApplicationEventReceiver : FeatureReceiverBase<SPWebApplication, SolutionJobInstallation>
    {
        public WebApplicationEventReceiver() : base(o => IoC.Get.Backend.BuildUp(o))
        {
        }
    }
}

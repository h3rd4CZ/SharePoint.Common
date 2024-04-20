using Microsoft.SharePoint;
using RhDev.SharePoint.Common.DataAccess.SharePoint.Installation;

namespace $ext_safeprojectname$.Common.DataAccess.SharePoint.Installation
{
    public class WebInstallation : FeatureInstallation<SPWeb>
    {
        protected override void DoExecuteInstallation(SPWeb scope)
        {
            
        }

        protected override void DoExecuteUninstallation(SPWeb scope)
        {

        }
    }
}

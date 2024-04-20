using Microsoft.SharePoint.Administration;
using RhDev.SharePoint.Common.DataAccess.SharePoint.Installation;
using $ext_safeprojectname$.Common.DataAccess.SharePoint.Installation.Jobs.Installations;

namespace $ext_safeprojectname$.Common.DataAccess.SharePoint.Installation.Jobs
{
    public class SolutionJobInstallation : FeatureInstallation<SPWebApplication>
    {
        private readonly EmptyJobInstallation emptyJobInstallation;

        public SolutionJobInstallation(EmptyJobInstallation emptyJobInstallation)
        {
            this.emptyJobInstallation = emptyJobInstallation;
        }

        protected override void DoExecuteInstallation(SPWebApplication scope)
        {
            emptyJobInstallation.ExecuteInstallation(scope);
        }


        protected override void DoExecuteUninstallation(SPWebApplication scope)
        {
            emptyJobInstallation.ExecuteUninstallation(scope);
        }
    }
}

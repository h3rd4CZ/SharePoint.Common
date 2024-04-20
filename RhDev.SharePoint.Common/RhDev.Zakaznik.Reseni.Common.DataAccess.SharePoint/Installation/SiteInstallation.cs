using Microsoft.SharePoint;
using RhDev.SharePoint.Common.DataAccess.SharePoint.Installation;
using $ext_safeprojectname$.Common.DataAccess.SharePoint.Installation.Membership;

namespace $ext_safeprojectname$.Common.DataAccess.SharePoint.Installation
{
    public class SiteInstallation : FeatureInstallation<SPSite>
    {
        private readonly SiteGroupMembershipSetup siteGroupMembershipSetup;

        public SiteInstallation(SiteGroupMembershipSetup siteGroupMembershipSetup)
        {
            this.siteGroupMembershipSetup = siteGroupMembershipSetup;
        }

        protected override void DoExecuteInstallation(SPSite scope)
        {
            siteGroupMembershipSetup.EnsureSiteGroups(scope.RootWeb);
        }
    }
}

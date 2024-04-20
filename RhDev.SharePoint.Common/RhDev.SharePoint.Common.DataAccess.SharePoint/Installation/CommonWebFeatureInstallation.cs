using Microsoft.SharePoint;
using RhDev.SharePoint.Common.DataAccess.SharePoint.Repository;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Installation
{
    public class CommonWebFeatureInstallation : FeatureInstallation<SPWeb>
    {
        protected override void DoExecuteInstallation(SPWeb scope)
        {
            ActivateDependentFeature(scope.Site, Config.Features.Site.ID);
        }
    }
}

using Microsoft.SharePoint;
using RhDev.SharePoint.Common.Security;
using System.Linq;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Security.Installation
{
    public class GroupInstallationHelper
    {
        public void CreateGroup(string webUrl, GroupDefinitionBase gd)
        {
            using (SPSite site = new SPSite(webUrl))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    // ensure SP group
                    SPGroup group = web.SiteGroups.Cast<SPGroup>().SingleOrDefault(g => g.Name == gd.Name);
                    if (group == null)
                    {
                        web.SiteGroups.Add(gd.Name, web.SiteAdministrators[0], null, gd.Description);
                    }                    
                }
            }
        }
    }
}

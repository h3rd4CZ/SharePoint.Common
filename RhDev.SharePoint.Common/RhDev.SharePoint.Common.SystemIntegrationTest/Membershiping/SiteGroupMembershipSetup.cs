using Castle.Components.DictionaryAdapter;
using Microsoft.SharePoint;
using RhDev.SharePoint.Common.DataAccess.SharePoint.Security.Installation;
using RhDev.SharePoint.Common.Security;
using RhDev.SharePoint.Common.SystemIntegrationTest.Membershiping.Groups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhDev.SharePoint.Common.SystemIntegrationTest.Membershiping
{
    public class SiteGroupMembershipSetup : BaseUserGroupsSetup
    {
        public SiteGroupMembershipSetup(IHierarchicalGroupProvider hierarchicalGroup) : base(hierarchicalGroup)
        {
        }

        protected override IDictionary<ApplicationGroup, Func<SPWeb, SPRoleDefinition>> membershipSetup =>
            new Dictionary<ApplicationGroup, Func<SPWeb, SPRoleDefinition>>
            {
                { SystemIntegrationGroups.Reader, w => w.RoleDefinitions.GetByType(SPRoleType.Reader) },
                { SystemIntegrationGroups.Writer, w => w.RoleDefinitions.GetByType(SPRoleType.Contributor) },
                { SystemIntegrationGroups.Manipulant, w => w.RoleDefinitions.GetByType(SPRoleType.WebDesigner) }
            };
    }
}

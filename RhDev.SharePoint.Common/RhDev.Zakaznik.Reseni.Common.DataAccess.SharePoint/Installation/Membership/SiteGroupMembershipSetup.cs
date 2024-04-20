using Microsoft.SharePoint;
using RhDev.SharePoint.Common.DataAccess.SharePoint.Security.Installation;
using RhDev.SharePoint.Common.Security;
using System;
using System.Collections.Generic;

namespace $ext_safeprojectname$.Common.DataAccess.SharePoint.Installation.Membership
{
    public class SiteGroupMembershipSetup : BaseUserGroupsSetup
    {
        public SiteGroupMembershipSetup(IHierarchicalGroupProvider hierarchicalGroup) : base(hierarchicalGroup)
        {
        }

        protected override IDictionary<ApplicationGroup, Func<SPWeb, SPRoleDefinition>> membershipSetup =>
            new Dictionary<ApplicationGroup, Func<SPWeb, SPRoleDefinition>>
            {
                { Setup.Membership.ApplicationGroups.Reader, w => w.RoleDefinitions.GetByType(SPRoleType.Reader) }
            };
    }
}

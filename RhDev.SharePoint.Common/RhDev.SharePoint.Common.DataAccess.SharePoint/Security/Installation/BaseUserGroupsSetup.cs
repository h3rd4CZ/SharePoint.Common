using System.Collections.Generic;
using System.Linq;
using RhDev.SharePoint.Common.Security;
using Microsoft.SharePoint;
using RhDev.SharePoint.Common.Caching.Composition;
using System;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Security.Installation
{
    public abstract class BaseUserGroupsSetup : IService
    {
        private readonly IHierarchicalGroupProvider hierarchicalGroup;
        private readonly GroupInstallationHelper groupInstallationHelper;

        protected abstract IDictionary<ApplicationGroup, Func<SPWeb, SPRoleDefinition>> membershipSetup { get; }
        
        public BaseUserGroupsSetup( IHierarchicalGroupProvider hierarchicalGroup)
        {
            this.hierarchicalGroup = hierarchicalGroup;
            groupInstallationHelper = new GroupInstallationHelper();
        }
        
        public void EnsureSiteGroups(SPWeb web)
        {
            
            foreach (var ms in membershipSetup)
            {
                var gd = hierarchicalGroup.GetDefinition(web.Title, ms.Key);
                groupInstallationHelper.CreateGroup(web.Url, gd);
                EnsureGroupPermissions(web, gd, ms.Value(web));
            }
        }
        
        private void EnsureGroupPermissions(SPWeb web, GroupDefinitionBase groupDefinition, SPRoleDefinition roleDefinition)
        {
            AddRoleAssignment(web, groupDefinition, roleDefinition);
        }

        private static void AddRoleAssignment(SPWeb web, GroupDefinitionBase groupDefinition, SPRoleDefinition roleDefinition)
        {
            if (!web.IsRootWeb && !web.HasUniqueRoleAssignments)
                web.BreakRoleInheritance(false);

            SPRoleAssignment roleAssignment = new SPRoleAssignment(web.SiteGroups[groupDefinition.Name]);

            if (web.RoleAssignments.Cast<SPRoleAssignment>().Any(assignment => assignment.Member.Name.Equals(roleAssignment.Member.Name)))
                return;

            roleAssignment.RoleDefinitionBindings.Add(roleDefinition);
            web.RoleAssignments.Add(roleAssignment);

            web.Update();
        }
    }
}

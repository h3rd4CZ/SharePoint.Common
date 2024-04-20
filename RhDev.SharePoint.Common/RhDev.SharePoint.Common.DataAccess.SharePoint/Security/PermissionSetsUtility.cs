using System;
using System.Linq;
using Microsoft.SharePoint;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Security
{
    public static class PermissionSetsUtility
    {
        public static void EnsureRoleDefinition(SPWeb web, PermissionSetDefinition definition)
        {
            if (!web.HasUniqueRoleDefinitions)
                web.RoleDefinitions.BreakInheritance(true, true);

            SPRoleDefinition roleDefinition = web.RoleDefinitions.Cast<SPRoleDefinition>().SingleOrDefault(
                    d => String.Equals(d.Name, definition.Name, StringComparison.OrdinalIgnoreCase));

            if (roleDefinition == null)
            {
                roleDefinition = new SPRoleDefinition();
                roleDefinition.Name = definition.Name;
                roleDefinition.BasePermissions = definition.Permissions;

                web.RoleDefinitions.Add(roleDefinition);
            }
            else
            {
                roleDefinition.BasePermissions = definition.Permissions;
                roleDefinition.Update();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using RhDev.SharePoint.Common.DataAccess.SharePoint.Repository;
using Microsoft.SharePoint;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Security.Permission
{
    public abstract class DocumentPermissionsRepositoryBase : RepositoryBase
    {
        protected DocumentPermissionsRepositoryBase(string webUrl, string listUrl)
            : base(webUrl, ListFetcher.ForRelativeUrl(listUrl))
        {
        }

        protected void SetPermissions(int itemId, string[] permissions, string[] groupNames)
        {
            UsingSpListElevated(list =>
            {
                SPWeb web = list.ParentWeb;
                SPListItem item = list.GetItemById(itemId);

                RolesUtility.SetReadOnly(item);

                foreach (string groupName in groupNames)
                {
                    if (string.IsNullOrEmpty(groupName)) continue;

                    if (!RolesUtility.SiteGroupExists(web, groupName))
                        throw new InvalidOperationException(String.Format(
                            "Group {0} does not exist on the web {1}", groupName, web.Name));

                    SPGroup group = RolesUtility.GetGroup(web, groupName);
                    var spRoleDefinitions = GetSPRoleDefinitions(permissions, web);

                    RolesUtility.AddPermissions(item, group, true, spRoleDefinitions);
                }
            });
        }

        protected void SetPermissions(int itemId, string[] permissions, int userId, string groupName)
        {
            UsingSpListElevated(list =>
            {
                SPWeb web = list.ParentWeb;
                SPListItem item = list.GetItemById(itemId);

                RolesUtility.SetReadOnly(item);

                //User
                SPFieldUserValue userValue = new SPFieldUserValue(web, userId, null);

                if (userValue.User == null)
                    throw new InvalidOperationException(
                        String.Format("User with ID {0} does not exist on the web {1}", userId, web.Name));

                var roleDefinitions = GetSPRoleDefinitions(permissions, web);
                RolesUtility.AddPermissions(item, userValue, true, roleDefinitions);

                //Group
                if (string.IsNullOrEmpty(groupName)) return;

                if (!RolesUtility.SiteGroupExists(web, groupName))
                    throw new InvalidOperationException(String.Format(
                        "Group {0} does not exist on the web {1}", groupName, web.Name));

                SPGroup group = RolesUtility.GetGroup(web, groupName);
                var spRoleDefinitions = GetSPRoleDefinitions(permissions, web);

                RolesUtility.AddPermissions(item, group, true, spRoleDefinitions);
            });
        }



        protected void SetPermissions(int itemId, string[] permissions, int userId)
        {
            if (userId == 0) return;

            UsingSpListElevated(list =>
            {
                SPWeb web = list.ParentWeb;
                SPListItem item = list.GetItemById(itemId);

                RolesUtility.SetReadOnly(item);

                SPFieldUserValue userValue = new SPFieldUserValue(web, userId, null);

                if (userValue.User == null)
                    throw new InvalidOperationException(
                        String.Format("User with ID {0} does not exist on the web {1}", userId, web.Name));

                var roleDefinitions = GetSPRoleDefinitions(permissions, web);
                RolesUtility.AddPermissions(item, userValue, true, roleDefinitions);
            });
        }
        
        protected void SetPermissions(int itemId, IDictionary<int, string> userPermissions, IDictionary<string, string> groupPermissions)
        {
            if (userPermissions == null) throw new ArgumentNullException("userPermissions");

            UsingSpListElevated(list =>
            {
                SPListItem item = list.GetItemById(itemId);
                SPWeb web = list.ParentWeb;
                RolesUtility.SetReadOnly(item);

                foreach (var permission in userPermissions)
                {
                    SPFieldUserValue userValue = new SPFieldUserValue(web, permission.Key, null);

                    if (userValue.User == null)
                        throw new InvalidOperationException(
                            String.Format("User with ID {0} does not exist on the web {1}", permission.Key,
                                web.Name));

                    var roleDefinitions = GetSPRoleDefinitions(new[] {permission.Value}, web);
                    RolesUtility.AddPermissions(item, userValue, true, roleDefinitions);
                }

                foreach (var permission in groupPermissions)
                {
                    var groupName = permission.Key;
                    if (string.IsNullOrEmpty(groupName)) continue;

                    if (!RolesUtility.SiteGroupExists(web, groupName))
                        throw new InvalidOperationException(String.Format(
                            "Group {0} does not exist on the web {1}", groupName, web.Name));

                    SPGroup group = RolesUtility.GetGroup(web, groupName);
                    var roleDefinitions = GetSPRoleDefinitions(new[] { permission.Value }, web);
                    RolesUtility.AddPermissions(item, group, true, roleDefinitions);
                }
            });
        }

        protected void SetPermissions(int itemId, string[] permissions, int[] userIds)
        {
            UsingSpListElevated(list =>
            {
                SPWeb web = list.ParentWeb;
                SPListItem item = list.GetItemById(itemId);

                RolesUtility.SetReadOnly(item);

                foreach (int userId in userIds)
                {
                    if (userId < 1) continue;

                    SPFieldUserValue userValue = new SPFieldUserValue(web, userId, null);

                    if (userValue.User == null)
                        throw new InvalidOperationException(
                            String.Format("User with ID {0} does not exist on the web {1}", userId, web.Name));

                    var roleDefinitions = GetSPRoleDefinitions(permissions, web);
                    RolesUtility.AddPermissions(item, userValue, true, roleDefinitions);
                }
            });
        }

        protected void SetPermissions(int itemId, string[] permissions, int[] userIds, string[] groupNames)
        {
            UsingSpListElevated(list =>
            {
                SPWeb web = list.ParentWeb;
                SPListItem item = list.GetItemById(itemId);

                RolesUtility.SetReadOnly(item);

                foreach (int userId in userIds)
                {
                    if (userId < 1) continue;

                    SPFieldUserValue userValue = new SPFieldUserValue(web, userId, null);

                    if (userValue.User == null)
                        throw new InvalidOperationException(
                            String.Format("User with ID {0} does not exist on the web {1}", userId, web.Name));

                    var roleDefinitions = GetSPRoleDefinitions(permissions, web);
                    RolesUtility.AddPermissions(item, userValue, true, roleDefinitions);
                }

                foreach (string groupName in groupNames)
                {
                    if (string.IsNullOrEmpty(groupName)) continue;

                    if (!RolesUtility.SiteGroupExists(web, groupName))
                        throw new InvalidOperationException(String.Format(
                            "Group {0} does not exist on the web {1}", groupName, web.Name));

                    SPGroup group = RolesUtility.GetGroup(web, groupName);
                    var spRoleDefinitions = GetSPRoleDefinitions(permissions, web);

                    RolesUtility.AddPermissions(item, group, true, spRoleDefinitions);
                }
            });
        }

        protected SPRoleAssignmentCollection RoleAssignments(int itemId)
        {
            SPRoleAssignmentCollection res = null;
            UsingSpListElevated(list =>
            {
                var itm = list.GetItemById(itemId);

                res = itm.RoleAssignments;
            });

            return res;
        }

        protected void InheritPermissions(int itemId)
        {
            UsingSpListElevated(list =>
            {
                SPListItem item = list.GetItemById(itemId);
                item.ResetRoleInheritance();
            });
        }

        private static SPRoleDefinition[] GetSPRoleDefinitions(string[] permissionSets, SPWeb web)
        {
            IList<SPRoleDefinition> spRoleDefinitions = new List<SPRoleDefinition>();

            foreach (string roleDefinitionName in permissionSets)
                spRoleDefinitions.Add(web.RoleDefinitions[roleDefinitionName]);

            return spRoleDefinitions.ToArray();
        }

        protected bool HasEditPermissions(int itemId)
        {
            return UsingSpList(list =>
            {
                SPListItem item = list.GetItemById(itemId);
                return item.DoesUserHavePermissions(SPBasePermissions.EditListItems);
            });
        }
    }
}

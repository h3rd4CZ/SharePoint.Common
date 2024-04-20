using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration.Claims;
using Microsoft.SharePoint.Utilities;
using RhDev.SharePoint.Common.DataAccess.SharePoint.Repository;
using RhDev.SharePoint.Common.Utils;
using NSubstitute;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Security
{
    public enum WebRoleType
    {
        Unknown = 0,
        Contributor,
        Reader,
        FullControl,
    }

    public static class RolesUtility
    {
        public const string DEFAULT_FORMS_PROVIDER_GROUP = "c:0-.f|ldaproles|{0}";
        public const string DEFAULT_FORMS_PROVIDER_USER = "i:0#.f|ldapmembers|{0}";
        
        public static string EncodeClaim(string loginName, bool isGroup)
        {
            if (loginName.Contains("\\"))
                loginName = loginName.Split(new[] { "\\" }, StringSplitOptions.RemoveEmptyEntries)[1];

            return string.Format( isGroup ? DEFAULT_FORMS_PROVIDER_GROUP : DEFAULT_FORMS_PROVIDER_USER, loginName);
        }

        public static string DecodeClaim(string encodedLogin)
        {
            if (string.IsNullOrEmpty(encodedLogin)) throw new ArgumentNullException("encodedLogin");

            SPClaimProviderManager mgr = SPClaimProviderManager.Local;
            if (mgr != null)
            {
                return mgr.DecodeClaim(encodedLogin).Value;
            }
            throw new InvalidOperationException("Local claim manager was not found");
        }

        /// <summary>
        /// Odebere všechna oprávnění a ponechá dědičnost.
        /// </summary>
        /// <param name="listItem"></param>
        public static void SetReadOnly(SPSecurableObject listItem)
        {
            listItem.BreakRoleInheritance(true);

            foreach (SPRoleAssignment assignment in listItem.RoleAssignments)
            {
                var readerDefinitions = assignment.RoleDefinitionBindings.OfType<SPRoleDefinition>().Where(r => r.Type != SPRoleType.Reader);

                foreach (SPRoleDefinition roleDef in readerDefinitions)
                {
                    assignment.RoleDefinitionBindings.Remove(roleDef);
                }

                //assignment.RoleDefinitionBindings.RemoveAll();

                assignment.Update();
            }
        }

        public static void SetNoAccess(SPSecurableObject listItem)
        {
            BreakPermissionInheritance(listItem, false);

            foreach (SPRoleAssignment assignment in listItem.RoleAssignments)
            {
                assignment.RoleDefinitionBindings.RemoveAll();
                assignment.Update();
            }
        }

        public static void BreakPermissionInheritance(SPSecurableObject securableObject, bool copyRoleAssignments)
        {
            if (securableObject.HasUniqueRoleAssignments)
                return;

            securableObject.BreakRoleInheritance(copyRoleAssignments);
        }

        public static void RemoveRoleDefinition(SPListItem listItem, string roleDefinitionName)
        {
            BreakPermissionInheritance(listItem, true);
            SPRoleDefinition roleDefinition = listItem.Web.RoleDefinitions[roleDefinitionName];

            foreach (SPRoleAssignment assignment in listItem.RoleAssignments)
            {
                if (assignment.RoleDefinitionBindings.Contains(roleDefinition))
                    assignment.RoleDefinitionBindings.Remove(roleDefinition);

                assignment.Update();
            }
        }

        public static bool SiteGroupExists(SPWeb web, string groupName)
        {
            foreach (SPGroup group in web.SiteGroups)
            {
                if (!@group.Name.Equals(groupName, StringComparison.OrdinalIgnoreCase))
                    continue;

                return true;
            }

            return false;
        }

        public static bool GroupExists(SPWeb web, string groupName)
        {
            foreach (SPGroup group in web.Groups)
            {
                if (!@group.Name.Equals(groupName, StringComparison.OrdinalIgnoreCase))
                    continue;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Assigns permissions to a SharePoint group in a web.
        /// </summary>
        /// <param name="web">Web of the object.</param>
        /// <param name="group">Group to assign permission to,.</param>
        /// <param name="roleDefinitions">Role definitions to assign permissions for.</param>
        public static void AssignGroupToWeb(SPWeb web, SPGroup group, params SPRoleDefinition[] roleDefinitions)
        {
            SPRoleAssignment assignment = new SPRoleAssignment(group);

            foreach (SPRoleDefinition roleDefinition in roleDefinitions)
                assignment.RoleDefinitionBindings.Add(roleDefinition);

            web.RoleAssignments.Add(assignment);
        }

        /// <summary>
        /// Applies role-based security to a user or group for a particular list item.  
        /// This overload is useful if you're iterating through the values of a SharePoint User or Group column.
        /// </summary>
        /// <param name="listItemToAddTo">The particular List Item to apply the security to.</param>
        /// <param name="userOrGroupToAdd">The SPFieldUserValue type from a SharePoint User or Group column.</param>
        /// <param name="copyInherited">TRUE to copy inherited role assignment; FALSE to break only.</param>
        /// <param name="rolesToGrant">SPRoleDefinition items to apply to the user or group being added.</param>
        public static void AddPermissions(SPListItem listItemToAddTo, SPFieldUserValue userOrGroupToAdd, bool copyInherited, params SPRoleDefinition[] rolesToGrant)
        {
            SPWeb sharePointWeb = listItemToAddTo.ParentList.ParentWeb;

            if (sharePointWeb == null || userOrGroupToAdd == null || rolesToGrant == null || rolesToGrant.Length <= 0)
                return;

            // Get SPPrincipal from the userOrGroupToAdd parameter
            SPPrincipal newItemToAdd;
            if (userOrGroupToAdd.User != null)
            {
                // We have a user
                newItemToAdd = userOrGroupToAdd.User;
            }
            else
            {
                // We have a group
                newItemToAdd = sharePointWeb.SiteGroups.GetByID(userOrGroupToAdd.LookupId);
            }

            // Call the overload that accepts an SPPrincipal object to add to the list
            AddPermissions(listItemToAddTo, newItemToAdd, copyInherited, rolesToGrant);
        }

        /// <summary>
        /// Applies role-based security to a user or group for a particular list item.  
        /// </summary>
        /// <param name="securable">Securable object (SPList, SPListItem).</param>
        /// <param name="userOrGroupToAdd">The SPFieldUserValue type from a SharePoint User or Group column.</param>
        /// <param name="copyInherited">TRUE to copy inherited role assignment; FALSE to break only.</param>
        /// <param name="rolesToGrant">SPRoleDefinition items to apply to the user or group being added.</param>
        /// <remarks>This overload is called by the other overloads to actually set the the security.</remarks>
        public static void AddPermissions(SPSecurableObject securable, SPPrincipal userOrGroupToAdd, bool copyInherited, params SPRoleDefinition[] rolesToGrant)
        {
            if (securable == null || userOrGroupToAdd == null || rolesToGrant == null || rolesToGrant.Length <= 0)
                return;

            if (!securable.HasUniqueRoleAssignments)
                securable.BreakRoleInheritance(copyInherited);

            // Create a new role assignment for the principal
            SPRoleAssignment newRoleAssignmentToAdd = new SPRoleAssignment(userOrGroupToAdd);

            // Bind the role definitionss to the  new role assignment
            foreach (SPRoleDefinition roleDefinition in rolesToGrant)
                if (roleDefinition != null)
                    newRoleAssignmentToAdd.RoleDefinitionBindings.Add(roleDefinition);

            // Add the new role assignment to the list item
            securable.RoleAssignments.Add(newRoleAssignmentToAdd);
        }

        public static bool IsCurrentUserInGroup(IUnitDesignation unitDesignation, string groupName)
        {
            try
            {
                using (var site = new SPSite(unitDesignation.GetAddress()))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        return IsCurrentUserInGroup(web, groupName);
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }

        public static bool IsCurrentUserInGroup(SPWeb web, string groupName)
        {
            SPGroup group = web.Groups[groupName];
            return web.IsCurrentUserMemberOfGroup(group.ID);
        }

        public static bool RoleDefinitionExists(SPWeb web, string definitionName)
        {
            return
                web.RoleDefinitions.Cast<SPRoleDefinition>().Any(
                    d => String.Equals(d.Name, definitionName, StringComparison.OrdinalIgnoreCase));
        }

        public static void UpdateObjectPermissions(SPWeb web, SPSecurableObject item, bool uniquePermissions, bool clearExisting, KeyValuePair<string, string[]>[] groupRoleMappings)
        {
            if (uniquePermissions)
            {
                if (!item.HasUniqueRoleAssignments)
                    item.BreakRoleInheritance(!clearExisting);

                if (clearExisting)
                    RolesUtility.SetNoAccess(item);
            }
            else
            {
                if (item.HasUniqueRoleAssignments)
                    item.ResetRoleInheritance();
            }

            foreach (KeyValuePair<string, string[]> groupRoleMapping in groupRoleMappings)
            {
                SPGroup group = RolesUtility.GetGroup(web, groupRoleMapping.Key);
                RolesUtility.UpdateObjectGroupRoles(item, group, groupRoleMapping.Value.Select(roleName => web.RoleDefinitions[roleName]).ToArray());
            }
        }

        public static void UpdateObjectPermissions(SPSecurableObject listItem, SPGroup[] groups, SPRoleDefinition[] roleDefinitions)
        {
            foreach (SPGroup group in groups)
                UpdateObjectGroupRoles(listItem, group, roleDefinitions);
        }

        public static void UpdateObjectGroupRoles(SPSecurableObject listItem, SPGroup group, SPRoleDefinition[] roleDefinitions)
        {
            RolesUtility.BreakPermissionInheritance(listItem, true);
            SPRoleAssignment roleAssignment = GetObjectRoleAssignment(listItem, group) ?? new SPRoleAssignment(group);

            foreach (SPRoleDefinition roleDefinition in roleDefinitions)
                if (roleAssignment.RoleDefinitionBindings.Cast<SPRoleDefinition>().All(d => d.Name != roleDefinition.Name))
                    roleAssignment.RoleDefinitionBindings.Add(roleDefinition);

            foreach (SPRoleDefinition roleDefinition in roleAssignment.RoleDefinitionBindings.Cast<SPRoleDefinition>())
                if (roleDefinitions.All(requested => requested.Name != roleDefinition.Name))
                    roleAssignment.RoleDefinitionBindings.Remove(roleDefinition);

            if (roleAssignment.Parent == null)
                listItem.RoleAssignments.Add(roleAssignment);
            else
                roleAssignment.Update();
        }

        public static void EnsureGroupRoleBinding(SPWeb web, SPSecurableObject obj, string groupName, string roleDefinition)
        {
            SPGroup group = GetGroup(web, groupName);
            SPRoleAssignment roleAssignment = GetObjectRoleAssignment(obj, group) ?? new SPRoleAssignment(group);
            if (!roleAssignment.RoleDefinitionBindings.Cast<SPRoleDefinition>().Any(b => b.Name == roleDefinition))
                roleAssignment.RoleDefinitionBindings.Add(web.RoleDefinitions[roleDefinition]);

            if (roleAssignment.Parent == null)
                obj.RoleAssignments.Add(roleAssignment);
            else
                roleAssignment.Update();
        }

        public static SPRoleAssignment GetObjectRoleAssignment(SPSecurableObject listItem, SPPrincipal principal)
        {
            foreach (SPRoleAssignment assignment in listItem.RoleAssignments)
                if (assignment.Member.ID == principal.ID) return assignment;

            return null;
        }

        public static SPRoleDefinition GetRoleDefinition(SPWeb web, string roleDefName)
        {
            foreach (SPRoleDefinition roleDef in web.RoleDefinitions)
                if (roleDef.Name.Equals(roleDefName, StringComparison.InvariantCultureIgnoreCase))
                    return roleDef;

            return null;
        }
        
        public static SPGroup GetGroup(SPWeb web, string name)
        {
            foreach (SPGroup group in web.SiteGroups)
                if (group.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)) return group;
            return null;
        }

        // TODO VIGM85 Section Designation tady nemá co dělat, předělat na string web url

        public static bool IsUserInGroup(SectionDesignation sectionDesignation, string userName, string groupName, int queryMaxCount)
        {
            bool userIsInGroup = false;

            SPSecurity.RunWithElevatedPrivileges(delegate
            {
                using (SPSite site = new SPSite(sectionDesignation.GetAddress()))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        SPGroup spGroup = site.RootWeb.SiteGroups[groupName];

                        if (IsUserInGroup(userName, spGroup))
                        {
                            userIsInGroup = true;
                            return;
                        }

                        userIsInGroup = IsUserInGroupADGroups(userName, queryMaxCount, spGroup, web);
                    }
                }
            });

            return userIsInGroup;
        }

        public static IList<string> UserInGroups(SectionDesignation sectionDesignation, string userName, string[] groupNames, int queryMaxCount)
        {
            IList<string> userInGroups = new List<string>();

            SPSecurity.RunWithElevatedPrivileges(delegate
            {
                using (SPSite site = new SPSite(sectionDesignation.GetAddress()))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        foreach (string groupName in groupNames)
                        {
                            SPGroup spGroup = site.RootWeb.SiteGroups[groupName];

                            if (IsUserInGroup(userName, spGroup) ||
                                IsUserInGroupADGroups(userName, queryMaxCount, spGroup, web))
                            {
                                userInGroups.Add(groupName);
                            }
                        }
                    }
                }
            });

            return userInGroups;
        }

        private static bool IsUserInGroupADGroups(string userName, int queryMaxCount, SPGroup spGroup, SPWeb web)
        {
            foreach (SPUser user in spGroup.Users)
            {
                if (!user.IsDomainGroup)
                    continue;

                bool reachedMax;
                bool isUserInAdGroup = IsUserInADGroup(web, user.LoginName, userName, queryMaxCount, out reachedMax,
                    new List<string>());

                if (reachedMax)
                    throw new PrincipalsFromADGroupQueryMaximumReachedException();

                if (!isUserInAdGroup)
                    continue;

                return true;
            }
            return false;
        }

        private static bool IsUserInGroup(string userName, SPGroup spGroup)
        {
            foreach (SPUser user in spGroup.Users)
            {
                if (user.LoginName.Equals(userName, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        public static bool IsUserInADGroup(SectionDesignation sectionDesignation, string groupName, string userName)
        {
            Guard.NotNull(sectionDesignation, nameof(sectionDesignation));
            Guard.StringNotNullOrWhiteSpace(groupName, nameof(groupName));
            Guard.StringNotNullOrWhiteSpace(userName, nameof(userName));

            using (SPSite site = new SPSite(sectionDesignation.Address))
            using (SPWeb web = site.OpenWeb())
            {
                bool isUserInGroup = IsUserInADGroup(web, groupName, userName, FrontEndSecurityContext.principalsQueryMaximum, out bool reachedMax, new List<string> { });
                
                if (reachedMax) throw new InvalidOperationException("Reached max queries detected when querying ad group");

                return isUserInGroup;
            }
        }


        private static bool IsUserInADGroup(SPWeb web, string groupName, string userName, int maxCount, out bool reachedMax, IList<string> processedGroups)
        {
            SPPrincipalInfo[] principals = SPUtility.GetPrincipalsInGroup(web, groupName, maxCount, out reachedMax);

            if (principals == null || principals.Length == 0)
                return false;

            foreach (SPPrincipalInfo principal in principals)
            {
                if (!principal.IsSharePointGroup &&
                    !principal.DisplayName.ToUpper().Equals("SYSTEM ACCOUNT") &&
                    principal.PrincipalType == SPPrincipalType.User)
                {
                    if (!String.IsNullOrEmpty(userName) && principal.LoginName.EndsWith(userName, StringComparison.OrdinalIgnoreCase))
                        return true;

                }
                else if (principal.PrincipalType == SPPrincipalType.SecurityGroup)
                {
                    if (processedGroups.Contains(principal.LoginName)) continue;

                    processedGroups.Add(principal.LoginName);

                    if (IsUserInADGroup(web, principal.LoginName, userName, maxCount, out reachedMax, processedGroups))
                        return true;

                }
            }
            return false;
        }

        public static bool IsEmptyGroup(SectionDesignation sectionDesignation, string groupName, int maxCount, bool checkSecurityGroup = true)
        {
            bool isEmptyGroup = true;

            SPSecurity.RunWithElevatedPrivileges(delegate
            {
                using (SPSite site = new SPSite(sectionDesignation.GetAddress()))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        SPGroup spGroup = site.RootWeb.SiteGroups[groupName];

                        if (Equals(null, spGroup.Users)) return;
                        
                        var hasDomainUsers = spGroup.Users.OfType<SPUser>().Where(u => u.IsDomainGroup).Count() > 0;
                        var hasSpUsers = spGroup.Users.OfType<SPUser>().Where(u => !u.IsDomainGroup).Count() > 0;
                        var hasUsers = hasDomainUsers || hasSpUsers;

                        if ((!checkSecurityGroup && hasSpUsers) || (checkSecurityGroup && (hasSpUsers || (hasDomainUsers && !IsEmptyADGroupsInGroup(maxCount, spGroup, web)))))
                        {
                            isEmptyGroup = false;
                        }
                    }
                }
            });

            return isEmptyGroup;
        }

        private static bool IsEmptyADGroupsInGroup(int maxCount, SPGroup spGroup, SPWeb web)
        {
            foreach (SPUser user in spGroup.Users)
            {
                if (!user.IsDomainGroup)
                    continue;

                bool reachedMax;
                bool isEmptyADGroup = IsEmptyADGroup(web, user.LoginName, maxCount, out reachedMax, new List<string>());

                if (reachedMax)
                    throw new PrincipalsFromADGroupQueryMaximumReachedException();

                if (isEmptyADGroup)
                    continue;

                return false;
            }
            return true;
        }

        public static bool IsEmptyADGroup(SectionDesignation sectionDesignation, string groupName)
        {
            Guard.NotNull(sectionDesignation, nameof(sectionDesignation));
            Guard.StringNotNullOrWhiteSpace(groupName, nameof(groupName));            

            using (SPSite site = new SPSite(sectionDesignation.Address))
            using (SPWeb web = site.OpenWeb())
            {
                bool isEmpty = IsEmptyADGroup(web, groupName, FrontEndSecurityContext.principalsQueryMaximum, out bool reachedMax, new List<string> { });

                if (reachedMax) throw new InvalidOperationException("Reached max queries detected when querying ad group");

                return isEmpty;
            }
        }

        private static bool IsEmptyADGroup(SPWeb web, string groupName, int maxCount, out bool reachedMax, IList<string> processedGroups)
        {
            SPPrincipalInfo[] principals = SPUtility.GetPrincipalsInGroup(web, groupName, maxCount, out reachedMax);

            if (principals == null || principals.Length == 0)
                return true;

            foreach (SPPrincipalInfo principal in principals)
            {
                if (!principal.IsSharePointGroup &&
                    !principal.DisplayName.ToUpper().Equals("SYSTEM ACCOUNT") &&
                    principal.PrincipalType == SPPrincipalType.User)
                {
                    return false;
                }

                if (principal.PrincipalType != SPPrincipalType.SecurityGroup)
                    continue;


                if (processedGroups.Contains(principal.LoginName))
                    continue;

                processedGroups.Add(principal.LoginName);

                if (!IsEmptyADGroup(web, principal.LoginName, maxCount, out reachedMax, processedGroups))
                    return false;

            }
            return true;
        }

        public static string GetRootWeb(string webUrl)
        {
            if (string.IsNullOrEmpty(webUrl)) throw new InvalidOperationException("webUrl");

            using (SPSite site = new SPSite(webUrl))
            using (SPWeb web = site.OpenWeb())
                return Equals(null, web.ParentWeb) ? string.Empty : web.ParentWeb.Url;
        }

        public static IList<string> Get1LevelSubWebs(string root)
        {
            if (string.IsNullOrEmpty(root)) throw new ArgumentNullException("root");

            using (SPSite site = new SPSite(root))
            using (SPWeb web = site.OpenWeb())
                return web.Webs.Select(w => w.Url).ToList();

        }

        public static string GetWebTitle(string webUrl)
        {
            if (string.IsNullOrEmpty(webUrl)) throw new ArgumentNullException("webUrl");

            using (SPSite site = new SPSite(webUrl))
            using (SPWeb web = site.OpenWeb())
                return web.Title;

        }

        public static string CurrentDomain()
        {
            return Environment.UserDomainName;
        }
    }
}

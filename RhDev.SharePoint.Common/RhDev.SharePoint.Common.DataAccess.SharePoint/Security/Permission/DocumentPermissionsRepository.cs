using System;
using System.Collections.Generic;
using System.Linq;
using RhDev.SharePoint.Common.DataAccess.Security;
using Microsoft.SharePoint;
using RhDev.SharePoint.Common.Utils;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Security.Permission
{
    public class DocumentPermissionsRepository : DocumentPermissionsRepositoryBase, IDocumentPermissionsRepository
    {

        public DocumentPermissionsRepository(string webUrl, string listUrl)
            : base(webUrl, listUrl)
        {
        }

        public void MakeDocumentEditable(int metadataId, params string[] groupName)
        {
            //SetPermissions(metadataId, new[] { SectionWebPermissionSets.Contribute }, groupName);
        }

        public void MakeDocumentReadOnly(int metadataId)
        {
            InheritPermissions(metadataId);
        }

        public void SetPermission(int metadataId, string[] groupNames, string permissionLevel)
        {
            SetPermissions(metadataId, new[] {permissionLevel}, groupNames);
        }

        public void SetPermission(int metadataId, int userId, string permissionLevel)
        {
            SetPermissions(metadataId, new[] {permissionLevel}, userId);
        }

        public void SetPermission(int metadataId, int userId, string group, string permissionLevel)
        {
            SetPermissions(metadataId, new[] {permissionLevel}, userId, group);
        }

        public void SetPermission(int metadataId, int[] userIds, string permissionLevel)
        {
            SetPermissions(metadataId, new[] {permissionLevel}, userIds);
        }

        public void SetPermission(int metadataId, int[] userIds, string[] groupNames, string permissionLevel)
        {
            SetPermissions(metadataId, new[] {permissionLevel}, userIds, groupNames);
        }

        public bool CanEditDocument(int metadataId)
        {
            return HasEditPermissions(metadataId);
        }

        /// <summary>
        /// Set permission for user and groups, input data are as Dic of name / Id as key, value as permission level for those users / groups
        /// </summary>
        /// <param name="metadataId"></param>
        /// <param name="userPermissions"></param>
        /// <param name="groupPermissions"></param>
        public void SetPermission(int metadataId, IDictionary<int, string> userPermissions, IDictionary<string, string> groupPermissions)
        {
            if (userPermissions == null) throw new ArgumentNullException("userPermissions");
            if (groupPermissions == null) throw new ArgumentNullException("groupPermissions");

            SetPermissions(metadataId, userPermissions, groupPermissions);
        }

        public IDictionary<string, IList<string>> GetRoleAssignments(int itemId)
        {
            var values =
                base.RoleAssignments(itemId).OfType<SPRoleAssignment>()
                    .Select(r => new KeyValuePair<string, IList<string>>(r.Member.LoginName,
                        r.RoleDefinitionBindings.OfType<SPRoleDefinition>().Select(b => b.Name).ToList()));

            return values.ToDictionary(keyValuePair => keyValuePair.Key, keyValuePair => keyValuePair.Value);
        }

        public void SetDocumentPermission(IList<PermissionSet> permission, int itemId, bool copyRoleAssignment, bool keepExistingRole, bool currentReadOnly)
        {
            UsingSpListElevated(list =>
            {
                var item = list.GetItemById(itemId);

                if (!item.HasUniqueRoleAssignments) item.BreakRoleInheritance(copyRoleAssignment);
                else
                {
                    if (!keepExistingRole)
                    {
                        item.ResetRoleInheritance();
                        item.BreakRoleInheritance(copyRoleAssignment);
                    }
                }

                if (currentReadOnly)
                {
                    var assignments = item.RoleAssignments;

                    for (var i = 0; i < assignments.Count; i++)
                    {
                        var assignment = assignments[i];
                        assignment.RoleDefinitionBindings.RemoveAll();
                        assignment.RoleDefinitionBindings.Add(list.ParentWeb.RoleDefinitions.GetByType(SPRoleType.Reader));

                        assignment.Update();
                    }
                }

                foreach (var permissionSet in permission)
                {
                    Guard.StringNotNullOrWhiteSpace(permissionSet.Level, nameof(permissionSet.Level));
                    Guard.NotNull(permissionSet.Principal, nameof(permissionSet.Principal));

                    if (!permissionSet.Principal.IsValid) throw new InvalidOperationException("Permission set principal is not valid");

                    var role = Enum.TryParse(permissionSet.Level, out SPRoleType roleType)
                        ? list.ParentWeb.RoleDefinitions.GetByType(roleType)
                        : list.ParentWeb.RoleDefinitions[permissionSet.Level];

                    var roleAssignment = permissionSet.Principal.Isgroup 
                    ? new SPRoleAssignment(list.ParentWeb.SiteGroups.GetByID(permissionSet.Principal.Id))
                    : new SPRoleAssignment( list.ParentWeb.SiteUsers.GetByID(permissionSet.Principal.Id));
                    
                    roleAssignment.RoleDefinitionBindings.Add(role);

                    item.RoleAssignments.Add(roleAssignment);
                }
            });
        }
    }
}

using System.Collections.Generic;
using RhDev.SharePoint.Common.Caching.Composition;

namespace RhDev.SharePoint.Common.DataAccess.Security
{
    public interface IDocumentPermissionsRepository : IService
    {
        void MakeDocumentEditable(int metadataId, params string[] groupName);

        void MakeDocumentReadOnly(int metadataId);

        void SetPermission(int metadataId, string[] groupName, string permissionLevel);

        void SetPermission(int metadataId, int userId, string permissionLevel);

        void SetPermission(int metadataId, int userId, string group, string permissionLevel);

        void SetPermission(int metadataId, int[] userIds, string permissionLevel);
        void SetPermission(int metadataId, int[] userIds, string[] groupNames, string permissionLevel);

        void SetPermission(int metadataId, IDictionary<int, string> userPermissions,
            IDictionary<string, string> groupPermissions);

        bool CanEditDocument(int metadataId);

        IDictionary<string, IList<string>> GetRoleAssignments(int itemId);

        void SetDocumentPermission(IList<PermissionSet> permission, int itemId, bool copyRoleAssignment, bool keepExistingRole, bool currentReadOnly);
    }
}

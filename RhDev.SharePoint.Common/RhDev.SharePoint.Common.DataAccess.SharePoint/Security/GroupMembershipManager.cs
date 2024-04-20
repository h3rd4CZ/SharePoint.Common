using System;
using System.Collections.Generic;
using System.Linq;
using RhDev.SharePoint.Common.Security;
using Microsoft.SharePoint;
using RhDev.SharePoint.Common.DataAccess.Repository.Entities;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Security
{
    public class GroupMembershipManager : IGroupMembershipManager
    {
        private readonly IHierarchicalGroupProvider _groupProvider;
        private readonly IApplicationLogManager _applicationLogManager;

        public GroupMembershipManager(
            IApplicationLogManager applicationLogManager,
            IHierarchicalGroupProvider groupProvider)
        {
            _groupProvider = groupProvider;
            _applicationLogManager = applicationLogManager;
        }
                
        public void AddUserToGroup(SectionDesignation designation, ApplicationGroup groupKind, IPrincipalInfo user)
        {
            AddUserToGroup(designation, groupKind, new List<IPrincipalInfo>() { user });
        }
        public void AddUserToGroup(SectionDesignation designation, ApplicationGroup groupKind, IList<IPrincipalInfo> users)
        {

            SPSite site = null;
            SPWeb web = null;
            SPSecurity.RunWithElevatedPrivileges(() =>
            {
                try
                {
                    site = new SPSite(designation.Address);
                    web = site.OpenWeb();
                    web.AllowUnsafeUpdates = true;

                    var groupDefinition = _groupProvider.GetDefinition(web.Title, groupKind);
                    var groupName = groupDefinition.Name;
                    SPGroup spGroup = web.SiteGroups.Cast<SPGroup>().SingleOrDefault(g => g.Name == groupName);
                    List<SPUser> spU = new List<SPUser>();
                    foreach (var u in users)
                    {
                        try
                        {
                            spU.Add(web.SiteUsers.GetByID(u.Id));
                        }
                        catch (Exception)
                        {
                            _applicationLogManager.WriteLog(
                                    string.Format("User {0} [id:{1}] not found on web {2} and will not be able to access the delegations list",
                                    u.DisplayName, u.Id, web.Title),
                                    web.Title, ApplicationLogLevel.Information);

                        }
                    }

                    AddUserToGroup(spGroup, spU);
                }
                finally
                {
                    if (web != null)
                        web.Dispose();

                    if (site != null)
                        site.Dispose();
                }
            });
        }

        private void AddUserToGroup(SPGroup group, IList<SPUser> users)
        {
            if (group == null) throw new ArgumentNullException("group");
            if (users == null) throw new ArgumentNullException("users");

            foreach (var userToAdd in users)
            {
                if (!group.Users.Cast<SPUser>().Any(u => u.LoginName.Equals(userToAdd.LoginName, StringComparison.OrdinalIgnoreCase)))
                    group.AddUser(userToAdd);
            }

            group.Update();
        }
    }
}

using System;
using System.Collections.Generic;
using RhDev.SharePoint.Common.Configuration;
using RhDev.SharePoint.Common.Security;
using Microsoft.SharePoint;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Security
{
    public class TimerJobSecurityContext : ISecurityContext
    {
        private readonly FarmConfiguration farmConfiguration;

        public string CurrentUserName
        {
            get { return CurrentUser.Name; }
        }

        public bool CurrentUserAdmin
        {
            get { return true; }
        }

        public string CurrentWebTitle
        {
            get { return "OWS"; }
        }

        public string CurrentWebUrl
        {
            get { throw new InvalidOperationException("Current web url is null in non WFE context"); }
        }

        public bool IsFrontend
        {
            get { return false; }
        }

        public string CurrentUserLoginAndWeb { get; set; }

        private UserInfo currentUser;
        public UserInfo CurrentUser
        {
            get
            {
                if (currentUser != null)
                    return currentUser;

                return (currentUser = GetSystemAccountInfo());
            }
            set { currentUser = value; }
        }

        public SectionDesignation CurrentLocation
        {
            get { throw new NotSupportedException(); }
        }

        public string[] GetCurrentUserRoles()
        {
            return new string[0];
        }

        public string[] GetUsersInGroup(string role)
        {
            throw new NotSupportedException();
        }

        public bool IsCurrentUserInGroup(string groupName)
        {
            return true;
        }

        public bool IsCurrentUserInGroupRecursive(SectionDesignation sectionDesignation, string groupName)
        {
            return true;
        }

        public bool IsCurrentUserInGroup(IUnitDesignation unitDesignation, string groupName)
        {
            return true;
        }

        public bool IsUserInGroup(SectionDesignation sectionDesignation, string userName, string groupName)
        {
            return true;
        }

        public IList<string> CurrentUserInGroups(SectionDesignation sectionDesignation, string[] groupNames)
        {
            throw new NotSupportedException();
        }

        public IList<string> UserInGroups(SectionDesignation sectionDesignation, string userName, string[] groupNames)
        {
            throw new NotSupportedException();
        }

        public bool IsEmptyGroup(SectionDesignation sectionDesignation, string groupName)
        {
            throw new NotSupportedException();
        }

        public bool IsSystem
        {
            get { return true; }
        }
                
        public TimerJobSecurityContext(FarmConfiguration farmConfiguration)
        {
            this.farmConfiguration = farmConfiguration;
        }

        private UserInfo GetSystemAccountInfo()
        {
            UserInfo result = null;
            SPSecurity.RunWithElevatedPrivileges(() =>
                {
                    using (var site = new SPSite(farmConfiguration.AppSiteUrl))
                    {
                        var systemAccount = site.SystemAccount;
                        result = new UserInfo(SectionDesignation.FromString(site.Url), systemAccount.ID, systemAccount.LoginName, systemAccount.Name, null,systemAccount.IsSiteAdmin, systemAccount.IsDomainGroup);
                    }
                });
            return result;
        }

        private UserInfo GetUserAccountInfo()
        {
            UserInfo result = null;
            SPSecurity.RunWithElevatedPrivileges(() =>
                {
                    var currentUserLoginAndWeb = CurrentUserLoginAndWeb.Split(new[] {';'}, StringSplitOptions.None);
                    using (var site = new SPSite(currentUserLoginAndWeb[1]))
                    {
                        using (var web = site.OpenWeb())
                        {
                            site.RootWeb.AllowUnsafeUpdates = true;
                            var account = web.EnsureUser(currentUserLoginAndWeb[0]);
                            result = new UserInfo(SectionDesignation.FromString(site.Url), account.ID, account.LoginName, account.Name, null, account.IsSiteAdmin, account.IsDomainGroup);
                        }
                    }
                });
            return result;
        }



        public ItemRoleDefinition GetItemRoleDefinition()
        {
            throw new NotSupportedException();
        }

        public string[] GetUserGroups(string userName)
        {
            throw new NotSupportedException();
        }

        public bool IsUserInAdGroup(SectionDesignation sectionDesignation, string userName, string groupName)
        {
            throw new NotSupportedException();
        }

        public bool IsEmptyAdGroup(SectionDesignation sectionDesignation, string groupName)
        {
            throw new NotSupportedException();
        }
    }
}
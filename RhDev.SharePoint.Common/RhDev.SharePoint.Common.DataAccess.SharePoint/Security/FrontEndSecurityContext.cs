using System;
using System.Collections.Generic;
using System.Linq;
using RhDev.SharePoint.Common.Security;
using Microsoft.SharePoint;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Security
{
    public class FrontEndSecurityContext : ISecurityContext
    {
        private readonly ISharePointContext sharePointContext;
        private readonly IHierarchicalGroupProvider hierarchicalGroupProvider;
        public const int principalsQueryMaximum = 500;

        public bool IsFrontend
        {
            get { return sharePointContext.Instance != null; }
        }

        public string CurrentUserLoginAndWeb { get; set; }

        public FrontEndSecurityContext(ISharePointContext sharePointContext,
            IHierarchicalGroupProvider hierarchicalGroupProvider)
        {
            this.sharePointContext = sharePointContext;
            this.hierarchicalGroupProvider = hierarchicalGroupProvider;
        }

        public bool CurrentUserAdmin
        {
            get
            {
                return CurrentUser.IsSiteAdmin;
            }
        }

        public string CurrentUserName
        {
            get
            {
                ValidateSharePointContext();
                return SharePointContext.Web.CurrentUser.LoginName;
            }
        }

        public string CurrentWebTitle
        {
            get
            {
                ValidateSharePointContext();
                return SharePointContext.Web.Title;
            }
        }

        public string CurrentWebUrl
        {
            get
            {
                ValidateSharePointContext();
                return SharePointContext.Web.Url;
            }
        }

        private void ValidateSharePointContext()
        {
            if (SharePointContext == null)
                throw new InvalidOperationException("No SharePoint context");

            if (SharePointContext.Web == null)
                throw new InvalidOperationException("No SharePoint web for this context");

            if (SharePointContext.Web.CurrentUser == null)
                throw new InvalidOperationException("No SharePoint user logged in this context");
        }

        public UserInfo CurrentUser
        {
            get
            {
                ValidateSharePointContext();

                SPUser currentUser = SharePointContext.Web.CurrentUser;
                return new UserInfo(CurrentLocation, currentUser.ID, currentUser.LoginName, currentUser.Name,
                    currentUser.Email, currentUser.IsSiteAdmin, CurrentUser.IsDomainGroup);
            }
        }

        public SectionDesignation CurrentLocation
        {
            get
            {
                ValidateSharePointContext();
                return SectionDesignation.FromString(SharePointContext.Web.Url);
            }
        }

        public string[] GetCurrentUserRoles()
        {
            return GetUserGroups(CurrentUserName);
        }

        public string[] GetUserGroups(string userName)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentException("userName");

            SPUser user = SharePointContext.Web.EnsureUser(userName);

            if (user == null)
                throw new InvalidOperationException("User not found");

            return user.Groups.Cast<SPGroup>().Select(g => g.Name).ToArray();
        }

        public string[] GetUsersInGroup(string role)
        {
            throw new NotImplementedException();
        }

        public bool IsCurrentUserInGroup(string groupName)
        {
            ValidateSharePointContext();
            return RolesUtility.IsCurrentUserInGroup(SharePointContext.Web, groupName);
        }

        public bool IsCurrentUserInGroup(IUnitDesignation unitDesignation, string groupName)
        {
            ValidateSharePointContext();
            return RolesUtility.IsCurrentUserInGroup(unitDesignation, groupName);
        }

        public bool IsCurrentUserInGroupRecursive(SectionDesignation sectionDesignation, string groupName)
        {
            ValidateSharePointContext();
            return RolesUtility.IsUserInGroup(sectionDesignation, CurrentUserName, groupName, principalsQueryMaximum);
        }

        public bool IsUserInGroup(SectionDesignation sectionDesignation, string userName, string groupName)
        {
            return RolesUtility.IsUserInGroup(sectionDesignation, userName, groupName, principalsQueryMaximum);
        }

        public IList<string> CurrentUserInGroups(SectionDesignation sectionDesignation, string[] groupNames)
        {
            return RolesUtility.UserInGroups(sectionDesignation, CurrentUserName, groupNames, principalsQueryMaximum);
        }

        public bool IsUserInAdGroup(SectionDesignation sectionDesignation, string userName, string groupName)
        {
            return RolesUtility.IsUserInADGroup(sectionDesignation, groupName, userName);
        }

        public bool IsEmptyAdGroup(SectionDesignation sectionDesignation, string groupName)
        {
            return RolesUtility.IsEmptyADGroup(sectionDesignation, groupName);
        }

        public IList<string> UserInGroups(SectionDesignation sectionDesignation, string userName, string[] groupNames)
        {
            return RolesUtility.UserInGroups(sectionDesignation, userName, groupNames, principalsQueryMaximum);
        }

        public bool IsEmptyGroup(SectionDesignation sectionDesignation, string groupName)
        {
            return RolesUtility.IsEmptyGroup(sectionDesignation, groupName, principalsQueryMaximum);
        }

        private SPContext SharePointContext
        {
            get { return sharePointContext.Instance; }
        }

        public bool IsSystem
        {
            get
            {
                ValidateSharePointContext();
                return SharePointContext.Web.Site.SystemAccount == SharePointContext.Web.CurrentUser;
            }
        }
                
        public ItemRoleDefinition GetItemRoleDefinition()
        {
            ItemRoleDefinition role = ItemRoleDefinition.None;

            if (string.IsNullOrEmpty(CurrentWebTitle))
                throw new InvalidOperationException("Current web title is null or empty");

            if (Equals(null, CurrentUser)) throw new InvalidOperationException("Current user is null");

            //All authenticated user is application user role
            role |= ItemRoleDefinition.User;

            //Application admin role
            var admin = hierarchicalGroupProvider.GetDefinition(CurrentWebTitle, ApplicationGroups.Administrator);

            if (RolesUtility.IsUserInGroup(SectionDesignation.FromString(CurrentWebUrl), CurrentUserName,
                admin.Name, 1)) role |= ItemRoleDefinition.Admin;
            
            return role;
        }
    }
}

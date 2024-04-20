using RhDev.SharePoint.Common.Caching.Composition;
using System.Collections.Generic;

namespace RhDev.SharePoint.Common.Security
{
    // TODO VIGM85 Zrevidovat metody a zrefaktorovat
    public interface ISecurityContext : IService
    {
        bool IsFrontend { get; }
        string CurrentUserLoginAndWeb { get; set; }
        string CurrentUserName { get; }
        string CurrentWebTitle { get; }
        string CurrentWebUrl { get; }
        bool CurrentUserAdmin { get; }
        UserInfo CurrentUser { get; }
        SectionDesignation CurrentLocation { get; }

        string[] GetCurrentUserRoles();
        string[] GetUserGroups(string userName);
        string[] GetUsersInGroup(string role);

        bool IsCurrentUserInGroup(string groupName);

        bool IsCurrentUserInGroup(IUnitDesignation unitDesignation, string groupName);

        /// <summary>
        /// POZOR, prochází všechny vnořené skupiny a BUDE TO POMALÉ u skupin s mnoha vnořenými skupinami.
        /// </summary>
        /// <param name="sectionDesignation"></param>
        /// <param name="groupName"></param>
        /// <returns></returns>
        bool IsCurrentUserInGroupRecursive(SectionDesignation sectionDesignation, string groupName);
        
        /// <summary>
        /// POZOR, prochází všechny vnořené skupiny a BUDE TO POMALÉ u skupin s mnoha vnořenými skupinami.
        /// </summary>
        /// <param name="sectionDesignation"></param>
        /// <param name="userName"></param>
        /// <param name="groupName"></param>
        /// <returns></returns>
        bool IsUserInGroup(SectionDesignation sectionDesignation, string userName, string groupName);

        /// <summary>
        /// Check against security group only
        /// </summary>
        /// <param name="sectionDesignation"></param>
        /// <param name="userName"></param>
        /// <param name="groupName"></param>
        /// <returns></returns>
        bool IsUserInAdGroup(SectionDesignation sectionDesignation, string userName, string groupName);

        /// <summary>
        /// POZOR, prochází všechny vnořené skupiny a BUDE TO POMALÉ u skupin s mnoha vnořenými skupinami.
        /// </summary>
        /// <param name="sectionDesignation"></param>
        /// <param name="groupNames"></param>
        /// <returns></returns>
        IList<string> CurrentUserInGroups(SectionDesignation sectionDesignation, string[] groupNames);

        /// <summary>
        /// POZOR, prochází všechny vnořené skupiny a BUDE TO POMALÉ u skupin s mnoha vnořenými skupinami.
        /// </summary>
        /// <param name="sectionDesignation"></param>
        /// <param name="userName"></param>
        /// <param name="groupNames"></param>
        /// <returns></returns>
        IList<string> UserInGroups(SectionDesignation sectionDesignation, string userName, string[] groupNames);

        bool IsEmptyGroup(SectionDesignation sectionDesignation, string groupName);
        
        bool IsEmptyAdGroup(SectionDesignation sectionDesignation, string groupName);

        bool IsSystem { get; }

        ItemRoleDefinition GetItemRoleDefinition();
    }
}

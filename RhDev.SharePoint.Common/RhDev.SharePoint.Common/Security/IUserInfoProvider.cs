using RhDev.SharePoint.Common.Caching.Composition;
using System.Collections.Generic;

namespace RhDev.SharePoint.Common.Security
{
    public interface IUserInfoProvider : IAutoRegisteredService
    {
        IList<UserInfo> ResolveMembers(IPrincipalInfo principalInfo, SectionDesignation sectionDesignation);
        string GetFullUserName(SectionDesignation sectionDesignation, int userId);

        UserInfo GetUserInfo(SectionDesignation sectionDesignation, int userId);

        UserInfo GetUserInfo(SectionDesignation sectionDesignation, string userName);

        UserInfo GetSystemUserInfo(SectionDesignation sectionDesignation);

        IList<UserInfo> GetUsers(SectionDesignation sectionDesignation, IList<string> userNames);

        IList<UserInfo> GetUsers(SectionDesignation sectionDesignation, IList<int> userIds);

        IList<UserInfo> GetUsers(SectionDesignation sectionDesignation, string webTitle, ApplicationGroup group);
        IEnumerable<UserInfo> ResolveSecurityGroup(SectionDesignation sectionDesignation, string loginName);

        IList<UserInfo> GetUsersUnrestricted(SectionDesignation sectionDesignation, string webTitle, ApplicationGroup group);

        UserInfo GetUserInfo(SectionDesignation sectionDesignation, string userName, bool isManualLogin);
    }
}

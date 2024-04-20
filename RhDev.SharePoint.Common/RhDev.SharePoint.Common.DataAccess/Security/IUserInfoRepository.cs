using System.Collections.Generic;
using RhDev.SharePoint.Common.Caching.Composition;

namespace RhDev.SharePoint.Common.DataAccess.Security
{
    public interface IUserInfoRepository : IService
    {
        string GetFullUserName(int userId);
        UserInfo GetUserInfo(int userId);
        UserInfo GetUserInfo(string userName);
        IList<UserInfo> GetUsers(IList<string> userNames);
        IList<UserInfo> GetUsers(IList<int> userIds);
        IList<UserInfo> GetUsers(string groupName);
        IList<UserInfo> GetUsersUnrestricted(string groupName);
        IList<UserInfo> ResolveMembers(IPrincipalInfo principalInfo);
        UserInfo GetSystemUserInfo();
        IEnumerable<UserInfo> ResolveSecurityGroup(string loginName);
        // TODO: Test EnsureUser pro manual login.. 
        UserInfo GetUserInfo(string userName, bool isManualLogin);
    }
}

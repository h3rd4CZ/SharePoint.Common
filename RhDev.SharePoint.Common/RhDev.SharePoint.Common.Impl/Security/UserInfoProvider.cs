using System.Collections.Generic;
using RhDev.SharePoint.Common.DataAccess.Security;
using RhDev.SharePoint.Common.Security;
using RhDev.SharePoint.Common.Utils;

namespace RhDev.SharePoint.Common.Impl.Security
{
    public class UserInfoProvider : IUserInfoProvider
    {
        private readonly ISecurityRepositoryFactory securityRepositoryFactory;
        private readonly IHierarchicalGroupProvider hierarchicalGroupProvider;
                
        public UserInfoProvider(ISecurityRepositoryFactory securityRepositoryFactory, IHierarchicalGroupProvider hierarchicalGroupProvider)
        {
            this.securityRepositoryFactory = securityRepositoryFactory;
            this.hierarchicalGroupProvider = hierarchicalGroupProvider;
        }
                
        private IUserInfoRepository GetUserInfoRepository(SectionDesignation sectionDesignation)
        {
            IUserInfoRepository userInfoRepository = securityRepositoryFactory.CreateUserInfoRepository(sectionDesignation);
            return userInfoRepository;
        }

        public IList<UserInfo> ResolveMembers(IPrincipalInfo principalInfo, SectionDesignation sectionDesignation)
        {
            Guard.NotNull(principalInfo, nameof(principalInfo));
            Guard.NotNull(sectionDesignation, nameof(sectionDesignation));

            var userInfoRepository = GetUserInfoRepository(sectionDesignation);

            return userInfoRepository.ResolveMembers(principalInfo);
        }

        public string GetFullUserName(SectionDesignation sectionDesignation, int userId)
        {
            var userInfoRepository = GetUserInfoRepository(sectionDesignation);
            return userInfoRepository.GetFullUserName(userId);
        }

        public UserInfo GetUserInfo(SectionDesignation sectionDesignation, int userId)
        {
            var userInfoRepository = GetUserInfoRepository(sectionDesignation);
            return userInfoRepository.GetUserInfo(userId);
        }

        public UserInfo GetUserInfo(SectionDesignation sectionDesignation, string userName)
        {
            var userInfoRepository = GetUserInfoRepository(sectionDesignation);
            return userInfoRepository.GetUserInfo(userName);
        }

        public UserInfo GetUserInfo(SectionDesignation sectionDesignation, string userName, bool isManualLogin)
        {
            if (!isManualLogin)
                return GetUserInfo(sectionDesignation, userName);

            var userInfoRepository = GetUserInfoRepository(sectionDesignation);
            return userInfoRepository.GetUserInfo(userName,true);
        }

        public UserInfo GetSystemUserInfo(SectionDesignation sectionDesignation)
        {
            var userInfoRepository = GetUserInfoRepository(sectionDesignation);
            return userInfoRepository.GetSystemUserInfo();
        }

        public IList<UserInfo> GetUsers(SectionDesignation sectionDesignation, IList<string> userNames)
        {
            var userInfoRepository = GetUserInfoRepository(sectionDesignation);
            return userInfoRepository.GetUsers(userNames);
        }

        public IList<UserInfo> GetUsers(SectionDesignation sectionDesignation, IList<int> userIds)
        {
            var userInfoRepository = GetUserInfoRepository(sectionDesignation);
            return userInfoRepository.GetUsers(userIds);
        }

        public IList<UserInfo> GetUsers(SectionDesignation sectionDesignation, string webTitle, ApplicationGroup groupDefinition)
        {
            SectionGroupDefinition group = hierarchicalGroupProvider.GetDefinition(webTitle, groupDefinition);

            var userInfoRepository = GetUserInfoRepository(sectionDesignation);
            return userInfoRepository.GetUsers(group.Name);
        }

        public IList<UserInfo> GetUsersUnrestricted(SectionDesignation sectionDesignation, string webTitle, ApplicationGroup groupDefinition)
        {
            SectionGroupDefinition group = hierarchicalGroupProvider.GetDefinition(webTitle, groupDefinition);

            var userInfoRepository = GetUserInfoRepository(sectionDesignation);
            return userInfoRepository.GetUsersUnrestricted(group.Name);
        }

        public IEnumerable<UserInfo> ResolveSecurityGroup(SectionDesignation sectionDesignation, string loginName)
        {
            Guard.NotNull(sectionDesignation, nameof(sectionDesignation));
            Guard.StringNotNullOrWhiteSpace(loginName, nameof(loginName));

            var userInfoRepository = GetUserInfoRepository(sectionDesignation);
            return userInfoRepository.ResolveSecurityGroup(loginName);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using RhDev.SharePoint.Common.DataAccess.Security;
using RhDev.SharePoint.Common.Logging;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using RhDev.SharePoint.Common.Utils;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Security
{
    public class UserInfoRepository : IUserInfoRepository
    {
        private const int PRINCIPAL_QUERY_MAX_COUNT = 1000;

        private readonly string webUrl;
        private readonly ITraceLogger traceLogger;
        
        public UserInfoRepository(string webUrl, ITraceLogger traceLogger)
        {
            this.webUrl = webUrl;
            this.traceLogger = traceLogger;
        }

        public string GetFullUserName(int userId)
        {
            UserInfo user = GetUserInfo(userId);
            return user.DisplayName;
        }

        public UserInfo GetUserInfo(int userId)
        {
            return FetchUserFromWeb(web => web.AllUsers.GetByID(userId), CreateUserInfo);
        }
                
        public UserInfo GetUserInfo(string userName, bool isManualLogin)
        {
            // TODO: GetUserInfo pro manual login?
            UserInfo userInfo = UserInfo.UnknownUser;

            SPSecurity.RunWithElevatedPrivileges(() =>
            {
                using (SPSite site = new SPSite(webUrl))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        web.AllowUnsafeUpdates = true;
                        site.AllowUnsafeUpdates = true;

                        try
                        {
                            SPUser fetchedUser = FormsAuthHelper.EnsureUser(userName, web, isManualLogin);
                            userInfo = CreateUserInfo(fetchedUser);
                        }
                        catch (Exception e)
                        {
                            traceLogger.Write(e, "(FormsAuthHelper): UserInfoRepository,GetUserInfo Unable to fetch user from web {0}", web.Url);
                        }
                    }
                }
            });

            return userInfo;
        }

        public UserInfo GetUserInfo(string userName)
        {
            return FetchUserFromWeb(web => web.EnsureUser(userName), CreateUserInfo);
        }

        public IList<UserInfo> GetUsers(IList<string> userNames)
        {
            return FetchUsersFromWeb(userNames, (web, userName) => web.EnsureUser(userName), CreateUserInfo);
        }

        public IList<UserInfo> GetUsers(IList<int> userIds)
        {
            return FetchUsersFromWeb(userIds, (web, userId) => web.AllUsers.GetByID(userId), CreateUserInfo);
        }


        public IList<UserInfo> GetUsers(string groupName)
        {
            using (SPSite site = new SPSite(webUrl))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    web.AllowUnsafeUpdates = true;
                    site.AllowUnsafeUpdates = true;
                    SPGroup group = web.Groups[groupName];

                    IList<UserInfo> userInfos =
                        GetUsersInGroup(group)
                            .Distinct(new UserInfoNameEqualityComparer())
                            .OrderBy(info => info.DisplayName)
                            .ToList();

                    CompleteUserInfo(userInfos, web);
                    return userInfos;
                }
            }
        }

        public IList<UserInfo> GetUsersUnrestricted(string groupName)
        {
            IList<UserInfo> userInfos = new List<UserInfo>();

            SPSecurity.RunWithElevatedPrivileges(() =>
            {
                foreach (UserInfo userInfo in GetUsers(groupName))
                {
                    userInfos.Add(userInfo);
                }
            });

            return userInfos;
        }

        public UserInfo GetSystemUserInfo()
        {
            using (SPSite site = new SPSite(webUrl))
            {
                SPUser systemAccount = site.SystemAccount;

                var sectionDesignation = SectionDesignation.FromString(webUrl);
                return new UserInfo(sectionDesignation, systemAccount.ID, systemAccount.LoginName, systemAccount.Name, null, systemAccount.IsSiteAdmin, systemAccount.IsDomainGroup);
            }
        }

        public IList<UserInfo> ResolveMembers(IPrincipalInfo principalInfo)
        {
            Guard.NotNull(principalInfo, nameof(principalInfo));
            var sd = SectionDesignation.FromString(webUrl);

            if (principalInfo.Isgroup) return GetUsers(principalInfo.Name);
            else if (principalInfo.IsDomainGroup) return ResolveSecurityGroup(principalInfo.Name).ToList();
            else return new List<UserInfo> { (UserInfo)principalInfo };
        }
                

        public IEnumerable<UserInfo> ResolveSecurityGroup(string loginName)
        {
            using (SPSite site = new SPSite(webUrl))
            using (SPWeb web = site.OpenWeb())
            {
                web.AllowUnsafeUpdates = true;
                site.AllowUnsafeUpdates = true;
                return ResolveSecurityGroup(web, loginName, new List<string> { });
            }
        }

        private UserInfo FetchUserFromWeb(Func<SPWeb, SPUser> userFetcher, Func<SPUser, UserInfo> userTransformer)
        {
            UserInfo userInfo = UserInfo.UnknownUser;

            SPSecurity.RunWithElevatedPrivileges(() =>
            {
                using (SPSite site = new SPSite(webUrl))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        web.AllowUnsafeUpdates = true;
                        site.AllowUnsafeUpdates = true;

                        try
                        {
                            SPUser fetchedUser = userFetcher(web);
                            userInfo = userTransformer(fetchedUser);
                        }
                        catch (Exception e)
                        {
                            traceLogger.Write(e, "Unable to fetch user from web {0}", web.Url);
                        }
                    }
                }
            });

            return userInfo;
        }
        private IList<UserInfo> FetchUsersFromWeb<TUserRequest>(IEnumerable<TUserRequest> userRequests, Func<SPWeb, TUserRequest, SPUser> userFetcher, Func<SPUser, UserInfo> userTransformer)
        {
            var users = new List<UserInfo>();

            SPSecurity.RunWithElevatedPrivileges(() =>
            {

                using (SPSite site = new SPSite(webUrl))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        web.AllowUnsafeUpdates = true;
                        site.AllowUnsafeUpdates = true;

                        foreach (TUserRequest userRequest in userRequests)
                        {
                            try
                            {
                                SPUser fetchedUser = userFetcher(web, userRequest);
                                UserInfo userInfo = userTransformer(fetchedUser);

                                users.Add(userInfo);
                            }
                            catch (Exception e)
                            {
                                traceLogger.Write(e, "Unable to fetch user from web {0} by request", web.Url,
                                    userRequest);
                            }
                        }
                    }
                }
            });

            return users;
        }

        private IList<UserInfo> GetUsersInGroup(SPGroup spGroup)
        {
            var allUsers = new List<UserInfo>();

            foreach (SPUser user in spGroup.Users)
            {
                if (user.IsDomainGroup)
                {
                    var users = ResolveSecurityGroup(spGroup.ParentWeb, user.LoginName, new List<string>());
                    allUsers.AddRange(users);
                }
                else
                    allUsers.Add(CreateUserInfo(user));
            }
            return allUsers;
        }

        private IEnumerable<UserInfo> ResolveSecurityGroup(SPWeb web, string name, IList<string> processedGroups)
        {
            bool reachedMax;

            SPPrincipalInfo[] principalsInGroup = SPUtility.GetPrincipalsInGroup(web, name, PRINCIPAL_QUERY_MAX_COUNT,
                out reachedMax);

            foreach (SPPrincipalInfo principalInfo in principalsInGroup)
            {
                if (principalInfo.PrincipalType == SPPrincipalType.SecurityGroup)
                {
                    if (processedGroups.Contains(principalInfo.LoginName))
                    {
                        continue;
                    }
                    processedGroups.Add(principalInfo.LoginName);
                    foreach (UserInfo userInfo in ResolveSecurityGroup(web, principalInfo.LoginName, processedGroups))
                        yield return userInfo;
                }
                else
                {
                    yield return CreateUserInfo(principalInfo, web);
                }
            }
        }

        private UserInfo CreateUserInfo(SPPrincipalInfo user, SPWeb web)
        {
            var userFetched = web.EnsureUser(user.LoginName);

            return CreateUserInfo(userFetched);
        }

        private static void CompleteUserInfo(IList<UserInfo> userInfos, SPWeb web)
        {
            foreach (UserInfo userInfo in userInfos)
            {
                SPUser user = web.EnsureUser(userInfo.Name);

                userInfo.Id = user.ID;
                userInfo.SectionDesignation = SectionDesignation.FromString(web.Url);
            }
        }

        private static UserInfo CreateUserInfo(SPUser user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return new UserInfo(SectionDesignation.FromString(user.ParentWeb.Url), user.ID, user.LoginName, user.Name, user.Email, user.IsSiteAdmin, user.IsDomainGroup);
        }
    }
}

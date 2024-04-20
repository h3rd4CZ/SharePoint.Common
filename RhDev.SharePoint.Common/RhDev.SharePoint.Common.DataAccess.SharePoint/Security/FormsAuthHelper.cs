using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SharePoint;
using RhDev.SharePoint.Common.Logging;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Security
{
    /// <summary>
    /// Helper to EnsureUsers with custom AuthProvider
    /// </summary>
    public static class FormsAuthHelper
    {
        
        #region const

        public const string DEFAULT_FORMS_PROVIDER_GROUP = "c:0-.f|ldaproles|{0}";
        public const string DEFAULT_FORMS_PROVIDER_USER = "i:0#.f|ldapmembers|{0}";


        public const string FORMS_AUTH_DELIMITER = "|";
        public const string FORMS_PROVIDER_MEMBERS = "ldapmembers";
        public const string FORMS_PROVIDER_ROLES = "ldaproles";
        public const string FORMS_CLAIM_PREFIX_MEMBER = "i:0#.f";
        public const string FORMS_CLAIM_PREFIX_ROLE = "c:0-.f";
        public const string LOGIN_GROUP_DELIMITER = "_";

        private static Dictionary<string, SPUserCollection> webSiteUsers = new Dictionary<string, SPUserCollection>();

        #endregion

        #region public

        /// <summary>
        /// Get form login name
        /// </summary>
        /// <param name="loginName"></param>
        /// <returns></returns>
        public static string GetLoginName(string loginName)
        {
            if (loginName.Contains(FORMS_CLAIM_PREFIX_MEMBER) || loginName.Contains(FORMS_CLAIM_PREFIX_ROLE)) return loginName;

            var loginWithoutDomain = loginName.Split(new[] { '\\' }).Last();

            var provider = loginWithoutDomain.Contains(LOGIN_GROUP_DELIMITER) ? FORMS_PROVIDER_ROLES : FORMS_PROVIDER_MEMBERS;

            var prefix = loginWithoutDomain.Contains(LOGIN_GROUP_DELIMITER) ? FORMS_CLAIM_PREFIX_ROLE : FORMS_CLAIM_PREFIX_MEMBER;

            var result = string.Concat(prefix, FORMS_AUTH_DELIMITER, provider, FORMS_AUTH_DELIMITER, loginWithoutDomain);

            return result;
        }

        /// <summary>
        /// Ensure user
        /// </summary>
        /// <param name="login">Login</param>
        /// <param name="web">web</param>
        /// <param name="isManualLogin">is manual login</param>
        /// <returns></returns>
        public static SPUser EnsureUser(string login, SPWeb web, bool isManualLogin)
        {
            

            // If login already contains form claim prefix, it will be use ase logon
            var logon = isManualLogin ? GetLoginName(login) : login;
            
            if (isManualLogin && !logon.Contains(FORMS_AUTH_DELIMITER))
                throw new Exception("Err in EnsureUser. Manual login is active but login '" + logon + "' not contains ML DELIMITER |");

            if (web.Site.WebApplication.UseClaimsAuthentication && !logon.Contains(FORMS_AUTH_DELIMITER))
                throw new Exception("Err in EnsureUser. Web application use claims authentication but login '" + logon + "' not contains ML DELIMITER |");

            SPUser user = null;
            try
            {
                // Try to get user from already existings...
                SPUserCollection userColl = null;
                if (webSiteUsers.TryGetValue(web.Url, out userColl))
                {
                    user = userColl[logon];
                }
                else
                {
                    var users = web.SiteUsers;
                    webSiteUsers.Add(web.Url, users);
                    user = users[logon];
                }
                //user = web.SiteUsers[logon];
            }
            catch
            {                
            }

            // If user is not in web SPUserCollection...
            if (user == null)
            {
                try
                {
                    web.SiteUsers.Add(logon, string.Empty, login, string.Empty);
                    user = web.SiteUsers[logon];
                }
                catch (Exception ex)
                {
                    throw new Exception("Err in EnsureUser. Cannot add user '" + logon + "' to web site users. Manual login is " + isManualLogin + ". " + ex);
                }
            }
            return user;
        }

        #endregion
    }
}

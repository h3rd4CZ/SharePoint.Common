using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SharePoint;
using Microsoft.SharePoint.BusinessData.SharedService;
using Microsoft.SharePoint.Utilities;
using RhDev.SharePoint.Common;
using RhDev.SharePoint.Common.Logging;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Extensions
{
    public static class SPListItemExtensions
    {
        public static UserInfo GetUserInfoFromUserFieldValue(this SPListItem listItem, Guid fieldId)
        {
            SPFieldUserValue userFieldValue = GetSPFieldUserValue(listItem, fieldId);
            return CreateUserInfo(userFieldValue);
        }

        public static IPrincipalInfo GetPrincipalInfoFromUserFieldValue(this SPListItem listItem, Guid fieldId)
        {
            SPFieldUserValue userFieldValue = GetSPFieldUserValue(listItem, fieldId);
            return CreatePrincipalInfo(listItem.Web, userFieldValue);
        }

        public static IList<IPrincipalInfo> GetPrincipalInfosFromUserFieldValue(this SPListItem listItem, Guid fieldId)
        {
            SPFieldUserValueCollection userFieldValues = (SPFieldUserValueCollection)listItem[fieldId];

            if (Equals(null, userFieldValues)) return null;

            return  userFieldValues.Select( u => CreatePrincipalInfo(listItem.Web, u)).ToList();
        }

        private static SPFieldUserValue GetSPFieldUserValue(SPListItem listItem, Guid fieldId)
        {
            SPField field = listItem.Fields[fieldId];

            if (!(field is SPFieldUser))
                throw new InvalidOperationException(string.Format("Field with ID {0} is not user field.", fieldId));

            object value = listItem[fieldId];

            if (value == null)
                return null;

            SPFieldUser userField = (SPFieldUser)field;
            SPFieldUserValue userFieldValue = (SPFieldUserValue)userField.GetFieldValue(value.ToString());

            return userFieldValue;
        }

        public static UserInfo CreateUserInfo(SPFieldUserValue userFieldValue)
        {
            if (userFieldValue == null)
                return null;

            SPUser user = userFieldValue.User;

            if (user == null)
                return UserInfo.UnknownUser;

            return new UserInfo(SectionDesignation.FromString(user.ParentWeb.Url), user.ID, user.LoginName, user.Name, user.Email, user.IsSiteAdmin, user.IsDomainGroup);
        }

        private static IPrincipalInfo CreatePrincipalInfo(SPWeb web, SPFieldUserValue userFieldValue)
        {
            if (userFieldValue == null)
                return null;

            SPUser user = userFieldValue.User;

            if (userFieldValue.User == null)
                return GetGroupInfoOrUnknownUserIfUserDeleted(web, userFieldValue);

            return new UserInfo(SectionDesignation.FromString(user.ParentWeb.Url), user.ID, user.LoginName, user.Name, user.Email, user.IsSiteAdmin, user.IsDomainGroup);
        }

        private static IPrincipalInfo GetGroupInfoOrUnknownUserIfUserDeleted(SPWeb web, SPFieldUserValue userFieldValue)
        {
            SPGroup group;

            try
            {
                @group = web.Groups.GetByID(userFieldValue.LookupId);
            }
            catch
            {
                return UserInfo.UnknownUser;
            }

            return new GroupInfo(userFieldValue.LookupId, @group.Name, @group.Description);
        }

        public static int? GetPrincipalId(this IPrincipalInfo principalInfo)
        {
            if (principalInfo == null)
                return null;

            return principalInfo.Id;
        }

        public static int GetRequiredPrincipalId(this IPrincipalInfo principalInfo)
        {
            if (principalInfo == null)
                throw new ArgumentNullException("principalInfo");

            return principalInfo.Id;
        }

        public static TEnum MapChoiceFieldValueToEnum<TEnum>(this SPListItem item, Guid fieldId) where TEnum : struct
        {
            var choiceField = (SPFieldChoice)item.Fields[fieldId];
            string[] choices = choiceField.Choices.Cast<string>().ToArray();

            var choiceValue = (string)item[fieldId];
            int choiceIndex = Array.IndexOf(choices, choiceValue);

            if (choiceIndex == -1)
                throw new ArgumentException(string.Format("Unable to map choice value {0} to enum {1}.", choiceValue,
                    typeof(TEnum).Name));

            int matchingEnumValue = choiceIndex;
            return (TEnum)Enum.ToObject(typeof(TEnum), matchingEnumValue);
        }

        public static string MapEnumValueToChoice(this SPListItem listItem, Guid fieldId, int enumValue)
        {
            return listItem.ParentList.MapEnumValueToChoice(fieldId, enumValue);
        }

        public static void SetUserFieldValue(this SPListItem listItem, Guid fieldId, IPrincipalInfo user)
        {
            listItem[fieldId] = new SPFieldUserValue(listItem.Web, user.Id, user.Name);
        }

        public static void SetUsersFieldValue(this SPListItem listItem, Guid fieldId, IList<IPrincipalInfo> users, bool clearCurrentUsers = true)
        {
            SPFieldUserValueCollection values = (SPFieldUserValueCollection)listItem[fieldId] ?? new SPFieldUserValueCollection();
            
            if (clearCurrentUsers) values.Clear();

            foreach (IPrincipalInfo principal in users)
                values.Add(new SPFieldUserValue(listItem.Web, principal.Id, principal.Name));

            listItem[fieldId] = values;
        }
                
        public static EntityLink GetUrlFieldValue(this SPListItem listItem, Guid fieldId)
        {
            var urlAsString = (string)listItem[fieldId];

            if (String.IsNullOrEmpty(urlAsString))
                return null;

            var urlValue = new SPFieldUrlValue(urlAsString);
            return new EntityLink(new Uri(urlValue.Url), urlValue.Description);
        }

        public static void SetUrlFieldValue(this SPListItem listItem, Guid fieldId, EntityLink link)
        {
            if (link == null)
                return;

            var urlValue = new SPFieldUrlValue(link.Uri.ToString());
            urlValue.Description = link.Description;

            listItem[fieldId] = urlValue;
        }

        public static EntityLink GetLink(this SPListItem listItem, string linkTitle)
        {
            var url = (string)listItem[SPBuiltInFieldId.EncodedAbsUrl];
            return new EntityLink(new Uri(url), linkTitle);
        }

        public static Uri GetDisplayFormUrl(this SPListItem listItem)
        {
            SPList list = listItem.ParentList;
            SPWeb web = list.ParentWeb;
            string webUrl = web.ServerRelativeUrl;

            string displayFormUrl = null;

            if (listItem.ContentType != null)
                displayFormUrl = listItem.ContentType.DisplayFormUrl;

            if (String.IsNullOrEmpty(displayFormUrl))
                displayFormUrl = list.Forms[PAGETYPE.PAGE_DISPLAYFORM].Url;

            bool isLayouts = displayFormUrl.StartsWith("_layouts/", StringComparison.InvariantCultureIgnoreCase);

            displayFormUrl = String.Format("{0}/{1}?ID={2}", webUrl, displayFormUrl, listItem.ID);

            if (isLayouts)
                displayFormUrl = String.Format("{0}&List={1}", displayFormUrl, SPEncode.UrlEncode(list.ID.ToString()));

            return new Uri(displayFormUrl, UriKind.Relative);
        }

        public static UserInfo GetUserInfoFromQualifiedUserId(this SPListItem listItem, Guid fieldId)
        {
            string value = (string)listItem[fieldId];

            if (String.IsNullOrEmpty(value))
                return null;

            SPFieldUrlValue urlValue = new SPFieldUrlValue(value);
            UserInfo userInfo = null;

            SPSecurity.RunWithElevatedPrivileges(() =>
            {
                using (SPSite site = new SPSite(urlValue.Url))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        int userId = int.Parse(urlValue.Description);

                        try
                        {
                            SPUser user = web.AllUsers.GetByID(userId);
                            userInfo = new UserInfo(SectionDesignation.FromString(web.Url), user.ID, user.LoginName, user.Name, user.Email, user.IsSiteAdmin, user.IsDomainGroup);
                        }
                        catch
                        {
                            userInfo = UserInfo.UnknownUser;
                        }
                    }
                }
            });

            return userInfo;
        }

        public static SPFieldUrlValue GetQualifiedUserId(this UserInfo userInfo)
        {
            if (userInfo == null)
                return null;

            return new SPFieldUrlValue(userInfo.SectionDesignation.GetAddress()) { Description = userInfo.Id.ToString() };
        }
    }
}

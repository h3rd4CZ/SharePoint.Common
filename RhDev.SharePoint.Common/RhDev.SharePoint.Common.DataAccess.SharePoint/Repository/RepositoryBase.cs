using System;
using RhDev.SharePoint.Common.DataAccess.SharePoint.Extensions;
using Microsoft.SharePoint;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Repository
{
    public abstract class RepositoryBase
    {
        private readonly Func<SPWeb, SPList> listFetcher;

        protected string WebUrl { get; private set; }

        private bool HasExplicitContentType
        {
            get { return unverifiedContentTypeId != null; }
        }

        /// <summary>
        /// Obsahuje ID content type, u kterého ještě nebylo ověřeno, zdali je v daném seznamu dostupný.
        /// Pro ověření a získání ID je třeba použít metodu <see cref="GetVerifiedContentTypeId"/>.
        /// </summary>
        private SPContentTypeId? unverifiedContentTypeId;

        protected virtual bool RequiresElevation
        {
            get { return false; }
        }

        protected virtual bool ReadOnly
        {
            get { return false; }
        }

        protected bool IsTimerJobScope
        {
            get
            {
                return !(SPContext.Current != null && SPContext.Current.Web != null &&
                       SPContext.Current.Web.CurrentUser != null);
            }
        }

        protected RepositoryBase(string webUrl, Func<SPWeb, SPList> listFetcher, SPContentTypeId? contentTypeId = null)
        {
            WebUrl = webUrl;
            this.listFetcher = listFetcher;
            unverifiedContentTypeId = contentTypeId;
        }

        private SPContentTypeId GetVerifiedContentTypeId(SPList list)
        {
            /* Ověření Content type je třeba dělat až v rámci ukládání, protože některé metody ukládají v elevaci 
             * a při ověření už v konstruktoru by nemusel mít uživatel práva na čtení do daného webu */

            if (unverifiedContentTypeId == null)
                throw new InvalidOperationException("No explicit content type set.");

            SPContentTypeId listContentTypeId = list.ContentTypes.BestMatch(unverifiedContentTypeId.Value);
            SPContentType contentType = list.ContentTypes[listContentTypeId];

            if (contentType == null)
                throw new InvalidOperationException("Content type is not available.");

            return unverifiedContentTypeId.Value;
        }

        private void UsingSpWeb(Action<SPWeb> action)
        {
            UsingSpWeb(WebUrl, action, RequiresElevation);
        }

        private static void UsingSpWeb(string webUrl, Action<SPWeb> action, bool requiresElevation)
        {
            RunWithElevationIfRequired(() =>
                {
                    using (SPSite site = new SPSite(webUrl))
                    {
                        using (SPWeb web = site.OpenWeb())
                        {
                            web.AllowUnsafeUpdates = true;
                            action(web);
                        }
                    }
                }, requiresElevation);
        }

        protected TResult UsingSpWeb<TResult>(Func<SPWeb, TResult> action)
        {
            TResult result = default (TResult);

            RunWithElevationIfRequired(() =>
                {
                    using (SPSite site = new SPSite(WebUrl))
                    {
                        using (SPWeb web = site.OpenWeb())
                        {
                            web.AllowUnsafeUpdates = true;
                            result = action(web);
                        }
                    }
                }, RequiresElevation);

            return result;
        }

        protected void UsingSpList(Action<SPList> action, string includedUser = null)
        {
            if (string.IsNullOrEmpty(includedUser))
            {
                try
                {
                    UsingSpList(action, RequiresElevation);
                }
                catch (UnauthorizedAccessException)
                {
                    if (SPContext.Current != null && SPContext.Current.Web != null && SPContext.Current.Web.CurrentUser != null)
                    {
                        var userToken = SPContext.Current.Web.CurrentUser.UserToken;
                        UsingSpListWithIncludedUser(action, true, userToken);
                    }
                }
            }
            else
                UsingSpListWithIncludedUser(action, RequiresElevation, includedUser);
        }

        protected void UsingSpListElevated(Action<SPList> action)
        {
            UsingSpList(action, true);
        }

        protected void UsingSpListWithIncludedUser(Action<SPList> action, bool requiresElevation, SPUserToken userToken)
        {
            RunWithElevationIfRequired(() =>
            {
                using (var site = new SPSite(WebUrl, userToken))
                {
                    using (var web = site.OpenWeb())
                    {
                        web.AllowUnsafeUpdates = true;
                        var list = GetList(web);
                        action(list);
                    }
                }
            }, requiresElevation);
        }

        protected void UsingSpListWithIncludedUser(Action<SPList> action, bool requiresElevation, string includedUser)
        {
            RunWithElevationIfRequired(() =>
            {
                using (var site = new SPSite(WebUrl, EnsureUserElevated(includedUser).UserToken))
                {
                    using (var web = site.OpenWeb())
                    {
                        web.AllowUnsafeUpdates = true;
                        var list = GetList(web);
                        action(list);
                    }
                }
            }, requiresElevation);
        }

        protected void UsingSpList(Action<SPList> action, bool requiresElevation)
        {
                RunWithElevationIfRequired(() =>
                    {
                        using (var site = new SPSite(WebUrl))
                        {
                            using (var web = site.OpenWeb())
                            {
                                web.AllowUnsafeUpdates = true;
                                var list = GetList(web);
                                action(list);
                            }
                        }
                    }, requiresElevation);
        }

        protected TResult UsingSpList<TResult>(Func<SPList, TResult> action)
        {
            return UsingSpList(action, RequiresElevation);
        }

        protected TResult UsingSpListElevated<TResult>(Func<SPList, TResult> action)
        {
            return UsingSpList(action, true);
        }

        protected TResult UsingSpListWithIncludedUser<TResult>(Func<SPList, TResult> action, bool requiresElevation, SPUserToken userToken)
        {
            TResult result = default(TResult);
            RunWithElevationIfRequired(() =>
            {
                using (var site = new SPSite(WebUrl, userToken))
                {
                    using (var web = site.OpenWeb())
                    {
                        web.AllowUnsafeUpdates = true;
                        var list = GetList(web);
                        result = action(list);
                    }
                }
            }, requiresElevation);
            return result;
        }

        protected TResult UsingSpListWithIncludedUser<TResult>(Func<SPList, TResult> action, bool requiresElevation, string includedUser)
        {
            TResult result = default(TResult);
            RunWithElevationIfRequired(() =>
            {
                using (var site = new SPSite(WebUrl, EnsureUserElevated(includedUser).UserToken))
                {
                    using (var web = site.OpenWeb())
                    {
                        web.AllowUnsafeUpdates = true;
                        var list = GetList(web);
                        result = action(list);
                    }
                }
            }, requiresElevation);
            return result;
        }

        protected TResult UsingSpList<TResult>(Func<SPList, TResult> action, bool requiresElevation)
        {
            TResult result = default(TResult);
            RunWithElevationIfRequired(() =>
            {
                using (var site = new SPSite(WebUrl))
                {
                    using (var web = site.OpenWeb())
                    {
                        web.AllowUnsafeUpdates = true;
                        var list = GetList(web);
                        result = action(list);
                    }
                }
            }, requiresElevation);

            return result;
        }

        private static void RunWithElevationIfRequired(SPSecurity.CodeToRunElevated action, bool elevationRequired)
        {
            if (elevationRequired)
                SPSecurity.RunWithElevatedPrivileges(action);
            else
                action();
        }

        protected SPList GetList(SPWeb web)
        {
            return listFetcher(web);
        }

        protected SPListItem CreateNewItem(SPList list)
        {
            SPListItem listItem = list.AddItem();

            if (HasExplicitContentType) 
                SetContentType(listItem);

            return listItem;
        }

        protected void SetContentType(SPListItem listItem)
        {
            if (HasExplicitContentType)
                listItem[SPBuiltInFieldId.ContentTypeId] = GetVerifiedContentTypeId(listItem.ParentList);
        }

        public void Delete(int entityId)
        {
            Delete(entityId, RequiresElevation);
        }

        protected void DeleteElevated(int entityId)
        {
            Delete(entityId, true);
        }

        private void Delete(int entityId, bool requiresElevation)
        {
            CheckMutability();

            UsingSpList(list =>
            {

                SPListItem item = list.GetItemById(entityId);

                if (item == null)
                    throw new EntityNotFoundException(entityId.ToString());

                OnBeforeDelete(item);
                item.Delete();

            }, requiresElevation);
        }

        protected virtual void OnBeforeDelete(SPListItem item)
        {
        }

        protected void CheckMutability()
        {
            if (ReadOnly)
                throw new NotSupportedException();
        }

        public LocationInfo GetLocation(int entityId)
        {
            return UsingSpList(list => GetLocation(list, entityId), true);
        }

        private LocationInfo GetLocation(SPList list, int entityId)
        {
            SPListItem listItem = list.GetItemById(entityId);

            if (listItem == null)
                throw new EntityNotFoundException(
                    String.Format("List item with ID {0} not found in list {1} on web {2}", entityId, list.Title,
                                  WebUrl));

            string displayFormUrl = listItem.GetDisplayFormUrl().ToString();
            return new LocationInfo(displayFormUrl, listItem.Title);
        }

        private SPUser EnsureUserElevated(string userLogin)
        {
            SPUser user = null;
            SPSecurity.RunWithElevatedPrivileges(() =>
            {
                using (SPSite eleSite = new SPSite(WebUrl))
                {
                    using (SPWeb eleWeb = eleSite.OpenWeb())
                    {
                        user = eleWeb.EnsureUser(userLogin);
                    }
                }
            });

            return user;
        }
    }
}
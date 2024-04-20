using System;
using Microsoft.SharePoint;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Repository
{
    public static class ListProvider
    {
        
        public static SPList GetListByGuid(SPWeb web, Guid id)
        {
            if (web == null) throw new ArgumentNullException("web");

            if(Equals(Guid.Empty, id)) throw  new InvalidOperationException("List guid is empty");

            return web.Lists[id];
        }
        
        /// <summary>
        /// POZOR, načítání seznamu podle názvu je POMALÉ, používat pouze v nejnutnějších případech,
        /// kdy není dopředu známo ListId (v takovém případě je třeba použít metodu <see cref="GetListById"/>).
        /// </summary>
        /// <exception cref="SPListNotFoundException"></exception> 
        public static SPList GetListByTitle(SPWeb web, string listTitle)
        {
            SPList list = web.Lists.TryGetList(listTitle);

            if (list == null)
                throw new SPListNotFoundException(
                    String.Format(
                        "List could not be found title '{0}' on web '{1}'. The list may have been deleted.",
                        listTitle, web.Url));

            return list;
        }

        public static SPList GetListByRelativeUrl(SPWeb web, string url)
        {
            if(string.IsNullOrEmpty(url)) throw  new ArgumentNullException("url");

            SPList list = null;

            try
            {
                list = web.GetList(string.Format("{0}{1}", web.Url, url));
            }
            catch { }

            if (list == null)
                throw new SPListNotFoundException(
                    String.Format(
                        "List could not be found url '{0}' on web '{1}'. The list may have been deleted.",
                        url, web.Url));

            return list;
        }
    }
}

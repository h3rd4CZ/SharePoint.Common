using System;
using Microsoft.SharePoint;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Repository
{
    public static class ListFetcher
    {
        public static Func<SPWeb, SPList> ForGuid(Guid listId)
        {
            return web => ListProvider.GetListByGuid(web, listId);
        }
        
        /// <summary>
        /// POZOR, načítání seznamu podle názvu je POMALÉ, používat pouze v nejnutnějších případech,
        /// kdy není dopředu známo ListId (v takovém případě je třeba použít metodu <see cref="ForId"/>).
        /// </summary>
        /// <param name="listTitle">Název seznamu.</param>
        /// <returns></returns>
        public static Func<SPWeb, SPList> ForTitle(string listTitle)
        {
            return web => ListProvider.GetListByTitle(web, listTitle);
        }

        public static Func<SPWeb, SPList> ForRelativeUrl(string url)
        {
            return web => ListProvider.GetListByRelativeUrl(web, url);
        }
    }
}

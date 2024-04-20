using Microsoft.SharePoint;
using RhDev.SharePoint.Common.DataAccess.Repository;
using RhDev.SharePoint.Common.DataAccess.Repository.Entities;
using RhDev.SharePoint.Common.DataAccess.SharePoint.Repository.CAML;
using System;
using System.Collections.Generic;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Repository
{
    public class DayOffConfigurationRepository : EntityRepositoryBase<DayOffConfigurationItem>, IDayOffRepository
    {
        public DayOffConfigurationRepository(string webUrl)
            : base(webUrl, ListFetcher.ForRelativeUrl(Config.Lists.DAYOFFURL))
        {
        }

        protected override void LoadData(SPListItem listItem, DayOffConfigurationItem entity)
        {
            base.LoadData(listItem, entity);

            if(!Equals(null, listItem[Config.Fields.DAYOFFLIST_DATE]))
                entity.Date = (DateTime)listItem[Config.Fields.DAYOFFLIST_DATE];
            entity.Lcid = (double)listItem[Config.Fields.DAYOFFLIST_LCID];
            entity.Repeate = (bool)listItem[Config.Fields.DAYOFFLIST_REPEAT];
        }

        protected override void CreateData(SPListItem listItem, DayOffConfigurationItem entity)
        {
            base.CreateData(listItem, entity);

            listItem[Config.Fields.DAYOFFLIST_DATE] = entity.Date;
            listItem[Config.Fields.DAYOFFLIST_REPEAT] = entity.Repeate;
            listItem[Config.Fields.DAYOFFLIST_LCID] = entity.Lcid;
        }

        protected override void UpdateData(SPListItem listItem, DayOffConfigurationItem entity)
        {
            CreateData(listItem, entity);
        }

        public IList<DayOffConfigurationItem> GetDayOffsByLcid(int lcid)
        {
            var lcidFilter = CAMLFilters.Equal(Config.Fields.DAYOFFLIST_LCID, lcid, CAMLType.Integer);
            var query = CAMLQueryBuilder.BuildQuery(lcidFilter);

            return QueryEntities(query);
        }
    }
}

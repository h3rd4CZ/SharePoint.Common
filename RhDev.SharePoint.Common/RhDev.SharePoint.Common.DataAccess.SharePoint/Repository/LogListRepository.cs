using System;
using System.Collections.Generic;
using Microsoft.SharePoint;
using RhDev.SharePoint.Common.DataAccess.Repository;
using RhDev.SharePoint.Common.DataAccess.Repository.Entities;
using RhDev.SharePoint.Common.DataAccess.SharePoint.Repository.CAML;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Repository
{
    public class LogListRepository : EntityRepositoryBase<LogItem>, IApplicationLogRepository<LogItem>
    {
        private const string DEFAULT_LOGTITLE = "Log";

        protected override bool RequiresElevation => true;

        protected override bool IsFolderStructurable => true;

        public LogListRepository(string webUrl)
            : base(webUrl, ListFetcher.ForRelativeUrl(Config.Lists.LOGURL))
        {

        }

        protected override void CreateData(SPListItem listItem, LogItem entity)
        {
            base.CreateData(listItem, entity);

            listItem[Config.Fields.LOGLIST_MESSAGE] = entity.Message;
            listItem[Config.Fields.LOGLIST_LEVEL] = entity.Level.ToString();
            listItem[Config.Fields.LOGLIST_SOURCE] = entity.Source;
        }

        public void WriteLog(LogItem item)
        {
            if (string.IsNullOrWhiteSpace(item.Title)) item.Title = DEFAULT_LOGTITLE;

            Create(item);
        }

        public new void RemoveEmptyFolders()
        {
            base.RemoveEmptyFolders();
        }

        public IList<LogItem> GetLogsOlderThen(int days, CentralClock now)
        {
            if (now == null) throw new ArgumentNullException(nameof(now));
            if (days <= 0) throw new ArgumentOutOfRangeException(nameof(days));

            DateTime deadlineDate = now.ExportDateTime.Subtract(new TimeSpan(days, 0, 0, 0));

            var createdLessThenFilter = CAMLFilters.Less(SPBuiltInFieldId.Created, deadlineDate, false);
            
            var query = CAMLQueryBuilder.BuildQuery(createdLessThenFilter, SPQueryScope.Recursive);

            return QueryEntities(query);
        }
    }
}

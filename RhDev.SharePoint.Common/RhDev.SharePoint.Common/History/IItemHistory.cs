using RhDev.SharePoint.Common.Caching.Composition;
using System.Collections.Generic;

namespace RhDev.SharePoint.Common.History
{
    public interface IItemHistory : IAutoRegisteredService
    {
        IList<DocumentHistoryEntry> GetHistory(string webUrl, string listUrl, int itemId);

        IList<DocumentHistoryEntry> GetHistory(HistoryEntityBase historyEntity);

        void WriteHistoryEvent(HistoryEntityBase historyEntity, string historyEvent, UserInfo user, string comment);

        void WriteHistoryEvent(HistoryEntityBase historyEntity, DocumentHistoryEvent historyEvent, UserInfo user, string comment);
    }
}

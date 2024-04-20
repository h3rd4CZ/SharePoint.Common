using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using RhDev.SharePoint.Common.History;
using RhDev.SharePoint.Common.DataAccess;
using RhDev.SharePoint.Common.Extensions;
using RhDev.SharePoint.Common.Serialization;

namespace RhDev.SharePoint.Common.Impl.History
{
    public class ItemHistory : IItemHistory
    {
        private readonly ICommonRepositoryFactory _repositoryFactory;
        private readonly ICentralClockProvider _centralClockProvider;

        public ItemHistory(ICommonRepositoryFactory repositoryFactory, ICentralClockProvider centralClockProvider)
        {
            _repositoryFactory = repositoryFactory;
            _centralClockProvider = centralClockProvider;
        }

        public IList<DocumentHistoryEntry> GetHistory(string webUrl, string listUrl, int itemId)
        {
            throw new NotImplementedException();
        }

        public IList<DocumentHistoryEntry> GetHistory(HistoryEntityBase historyEntity)
        {
            return GetRawHistory(historyEntity).Entries.Select(e => (DocumentHistoryEntry) e).ToList();
        }

        public void WriteHistoryEvent(HistoryEntityBase historyEntity, string historyEvent, UserInfo user, string comment)
        {
            DocumentHistoryEntry entry = new DocumentHistoryEntry()
            {
                DateOccured = _centralClockProvider.Now(),
                Description = comment,
                HistoryEvent = historyEvent,
                UserName = user.DisplayName
            };

            WriteEntryToHistory(historyEntity, entry);
        }

        public void WriteHistoryEvent(HistoryEntityBase historyEntity, DocumentHistoryEvent historyEvent, UserInfo user, string comment)
        {
            var translatedEvent = TranslateEvent(historyEvent);

            WriteHistoryEvent(historyEntity, translatedEvent, user, comment);
        }

        private string TranslateEvent(DocumentHistoryEvent historyEvent)
        {
            if (historyEvent == 0) return "UNKNOWN EVENT";

            return historyEvent.GetEnumDescription();
        }

        private void WriteEntryToHistory(HistoryEntityBase metadata, DocumentHistoryEntry entry)
        {
            DocumentHistorySerializable currHistory = GetRawHistory(metadata);

            currHistory.AddEntry(entry);

            metadata.History = Serialize(currHistory);
        }

        private DocumentHistorySerializable GetRawHistory(HistoryEntityBase historyEntity)
        {
            string currHistory = historyEntity.History;

            return !String.IsNullOrEmpty(currHistory) ? Deserialize(currHistory) : GetEmptyHistory();
        }

        private DocumentHistorySerializable GetEmptyHistory()
        {
            return new DocumentHistorySerializable();
        }

        private static DocumentHistorySerializable Deserialize(string xml)
        {
            using (StringReader sr = new StringReader(xml))
                return XmlSerialization.Deserialize<DocumentHistorySerializable>(sr);            
        }

        private static string Serialize(DocumentHistorySerializable docHistory)
        {
            using (StringWriter sw = new StringWriter())
            {
                XmlSerialization.Serialize<DocumentHistorySerializable>(sw, docHistory);
                return sw.ToString();
            }
        }
    }
}

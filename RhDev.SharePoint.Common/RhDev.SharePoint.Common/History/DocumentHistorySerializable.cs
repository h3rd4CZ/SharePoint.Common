using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace RhDev.SharePoint.Common.History
{
    [Serializable]
    [XmlInclude(typeof(DocumentHistoryEntry))]

    public class DocumentHistorySerializable
    {
        private List<DocumentHistoryEntryBase> entries = new List<DocumentHistoryEntryBase>();

        public DocumentHistorySerializable()
        {
        }

        public DocumentHistorySerializable(DocumentHistoryEntryBase entry)
        {
            AddEntry(entry);
        }

        public List<DocumentHistoryEntryBase> Entries
        {
            get { return entries; }
            set { entries = value; }
        }

        public void AddEntry(DocumentHistoryEntryBase entry)
        {
            entries.Add(entry);
        }
    }
}

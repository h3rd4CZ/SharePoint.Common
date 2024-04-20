using System;
using System.Collections.Generic;

namespace RhDev.SharePoint.Common.History
{
    [Serializable]
    public abstract class DocumentHistoryEntryBase
    {
        public CentralClock DateOccured { get; set; }

        public override bool Equals(object obj)
        {
            var historyEntry = (obj as DocumentHistoryEntryBase);
            if (historyEntry == null)
                return false;

            return DateOccured.Equals(historyEntry.DateOccured);
        }

        public override int GetHashCode()
        {
            return -483118074 + EqualityComparer<CentralClock>.Default.GetHashCode(DateOccured);
        }
    }
}

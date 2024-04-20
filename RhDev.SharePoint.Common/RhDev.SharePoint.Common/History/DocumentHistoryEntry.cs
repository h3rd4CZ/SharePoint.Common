using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RhDev.SharePoint.Common.History
{
    [Serializable]
    public class DocumentHistoryEntry : DocumentHistoryEntryBase
    {
        public string UserName { get; set; }

        public string UserInRole { get; set; }

        public string HistoryEvent { get; set; }

        public string Description { get; set; }


        public override bool Equals(object obj)
        {
            var historyEntry = (obj as DocumentHistoryEntry);
            if (historyEntry == null)
                return false;

            if (UserName != historyEntry.UserName)
                return false;

            if (UserInRole != null)
            {
                if (!UserInRole.Equals(historyEntry.UserInRole))
                    return false;
            }

            if (!HistoryEvent.Equals(historyEntry.HistoryEvent))
                return false;
            if (Description != historyEntry.Description)
                return false;

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            var hashCode = 887941734;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(UserName);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(UserInRole);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(HistoryEvent);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Description);
            return hashCode;
        }
    }
}

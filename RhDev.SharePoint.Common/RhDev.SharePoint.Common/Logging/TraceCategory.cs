using System;

namespace RhDev.SharePoint.Common.Logging
{
    [Serializable]
    public class TraceCategory
    {                
        public string Area { get; private set; }

        public string Category { get; private set; }
        public TraceSeverity TraceSeverity { get; }
        public EventSeverity EventSeverity { get; }

        public TraceCategory(string area, string category) 
            : this(area, category, TraceSeverity.Medium, EventSeverity.Information) { }
        
        public TraceCategory(string area, string category, TraceSeverity traceSeverity, EventSeverity eventSeverity)
        {
            Area = area;
            Category = category;

            TraceSeverity = traceSeverity;
            EventSeverity = eventSeverity;
        }
                
        public override string ToString()
        {
            return String.Format("{0}/{1}", Area, Category);
        }
    }
}

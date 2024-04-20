namespace RhDev.SharePoint.Common.Logging
{
    public class TraceCategories
    {
        public static TraceCategory General => new TraceCategory(Constants.DefaultAreaName, Constants.GeneralCategoryName);
        public static TraceCategory FrontEnd => new TraceCategory(Constants.DefaultAreaName, Constants.FrontEndCategoryName);
        public static TraceCategory Common => new TraceCategory(Constants.DefaultAreaName, Constants.CommonCategoryName);
        public static TraceCategory Security => new TraceCategory(Constants.DefaultAreaName, Constants.SecurityCategoryName, TraceSeverity.High, EventSeverity.Warning);
        public static TraceCategory WebServices => new TraceCategory(Constants.DefaultAreaName, Constants.WebServicesCategoryName, TraceSeverity.Monitorable, EventSeverity.Verbose);
        public static TraceCategory Integration => new TraceCategory(Constants.DefaultAreaName, Constants.Integration);
    }
}

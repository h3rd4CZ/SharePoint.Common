using RhDev.SharePoint.Common.Logging;

namespace $ext_safeprojectname$.Common.Setup.Tracing
{
    public class TraceCategories
    {
        public static TraceCategory Database => new TraceCategory(Const.SOLUTION_DISPLAY, "Database", TraceSeverity.Medium, EventSeverity.Verbose);
        public static TraceCategory Service => new TraceCategory(Const.SOLUTION_DISPLAY, "Service");
    }
}

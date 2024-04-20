using RhDev.SharePoint.Common.Logging;
using System.Collections.Generic;

namespace $ext_safeprojectname$.Common.Setup.Tracing
{
    public static class TraceConfiguration
    {
        public static DiagnosticsServiceConfiguration GetDiagnosticsServiceConfiguration =>
            new DiagnosticsServiceConfiguration(Const.SOLUTION_DISPLAY, new List<TraceCategory>
            {
                TraceCategories.Database,
                TraceCategories.Service
            });
    }
}

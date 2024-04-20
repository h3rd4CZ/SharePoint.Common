using RhDev.SharePoint.Common.DataAccess.SharePoint.Logging;
using RhDev.SharePoint.Common.Logging;
using $ext_safeprojectname$.Common.Setup.Tracing;

namespace $ext_safeprojectname$.Common.DataAccess.SharePoint.Tracing
{
    public class SolutionTraceLogger : SharePointTraceLogger
    {
        public override DiagnosticsServiceConfiguration GetConfiguration => TraceConfiguration.GetDiagnosticsServiceConfiguration;
    }
}

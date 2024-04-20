using Microsoft.SharePoint.Administration;
using RhDev.SharePoint.Common.DataAccess.SharePoint.Installation;
using RhDev.SharePoint.Common.Logging;

namespace $ext_safeprojectname$.Common.DataAccess.SharePoint.Installation.Tracing
{
    public class SolutionTracingInstallation : FeatureInstallation<SPWebService>
    {
        private readonly ITraceLogger traceLogger;

        public SolutionTracingInstallation(ITraceLogger traceLogger)
        {
            this.traceLogger = traceLogger;
        }

        protected override void DoExecuteInstallation(SPWebService scope)
        {
            
        }

        protected override void DoExecuteUninstallation(SPWebService scope)
        {

        }
    }
}

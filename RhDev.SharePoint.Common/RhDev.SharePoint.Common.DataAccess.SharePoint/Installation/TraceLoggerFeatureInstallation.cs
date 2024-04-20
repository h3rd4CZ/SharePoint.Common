using Microsoft.SharePoint.Administration;
using RhDev.SharePoint.Common.Logging;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Installation
{
    public class TraceLoggerFeatureInstallation : FeatureInstallation<SPWebService>
    {
        private readonly ITraceLogger traceLogger;

        public TraceLoggerFeatureInstallation(ITraceLogger traceLogger)
        {
            this.traceLogger = traceLogger;
        }

        protected override void DoExecuteInstallation(SPWebService webService)
        {
            traceLogger.Register();
        }

        protected override void DoExecuteUninstallation(SPWebService scope)
        {
            traceLogger.Unregister();
        }
    }
}

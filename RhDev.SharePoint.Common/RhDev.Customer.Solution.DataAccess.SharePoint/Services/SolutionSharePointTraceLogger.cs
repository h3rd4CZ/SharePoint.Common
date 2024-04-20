using RhDev.SharePoint.Common.DataAccess.SharePoint.Logging;
using RhDev.SharePoint.Common.Logging;

namespace RhDev.Customer.Solution.Common.DataAccess.SharePoint.Services
{
    public class SolutionSharePointTraceLogger : SharePointTraceLogger
    {
        public override DiagnosticsServiceConfiguration GetConfiguration => Setup.Constants.GetDiagnosticsServiceConfiguration;
    }
}

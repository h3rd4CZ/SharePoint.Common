using RhDev.SharePoint.Common.DataAccess.SharePoint.Logging;
using RhDev.SharePoint.Common.Logging;
using System.Collections.Generic;

namespace RhDev.SharePoint.Common.Test.SystemIntegrationTest
{
    public class SolutionSharePointTraceLogger : SharePointTraceLogger
    {
        public override DiagnosticsServiceConfiguration GetConfiguration => new DiagnosticsServiceConfiguration
        {
            Name = "SuperArea",
            AvailableCategories = new List<TraceCategory>
                {
                    new TraceCategory("SuperArea", "CategoryX"),
                    new TraceCategory("SuperArea", "CategoryY")
                }
        };
    }
}

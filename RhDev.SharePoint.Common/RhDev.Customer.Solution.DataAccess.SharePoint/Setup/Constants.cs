using RhDev.SharePoint.Common.Logging;
using System.Collections.Generic;

namespace RhDev.Customer.Solution.Common.DataAccess.SharePoint.Setup
{
    public class Constants
    {
        public const string SOLUTION_NAME = "CustomerSolutionNext";

        public static class TraceCategories 
        {
            public static TraceCategory Legacy = new TraceCategory(SOLUTION_NAME, "Legacy", TraceSeverity.High, EventSeverity.Verbose);
            public static TraceCategory Integration = new TraceCategory(SOLUTION_NAME, "Integration");
        }

        public static DiagnosticsServiceConfiguration GetDiagnosticsServiceConfiguration =>
            new DiagnosticsServiceConfiguration(SOLUTION_NAME, new List<TraceCategory>
            {
                TraceCategories.Legacy,
                TraceCategories.Integration
            });
    }
}

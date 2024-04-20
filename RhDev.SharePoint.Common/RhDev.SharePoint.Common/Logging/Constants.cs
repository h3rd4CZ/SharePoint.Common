using System.Collections.Generic;

namespace RhDev.SharePoint.Common.Logging
{
    public static class Constants
    {
        /// <summary>
        /// Constant that defines the path separator for categories (Area/Category).
        /// </summary>
        
        public const char CategoryPathSeparator = '/';

        public const string DefaultAreaName = Common.Constants.SOLUTION_NAME;

        public const string GeneralCategoryName = "General";
        public const string FrontEndCategoryName = "FrontEnd";
        public const string CommonCategoryName = "Common";
        public const string SecurityCategoryName = "Security";
        public const string WebServicesCategoryName = "WebServices";
        public const string Integration = "Integration";

        public static DiagnosticsServiceConfiguration GetDiagnosticsServiceConfiguration =>
            new DiagnosticsServiceConfiguration(DefaultAreaName, new List<TraceCategory>
            {
                TraceCategories.General,
                TraceCategories.FrontEnd,
                TraceCategories.Common,
                TraceCategories.Security,
                TraceCategories.WebServices,
                TraceCategories.Integration
            });
    }
}

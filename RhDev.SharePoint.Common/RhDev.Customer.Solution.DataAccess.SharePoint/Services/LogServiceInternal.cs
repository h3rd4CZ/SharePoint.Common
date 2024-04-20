using RhDev.SharePoint.Common.Caching.Composition;
using RhDev.SharePoint.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhDev.Customer.Solution.Common.DataAccess.SharePoint.Services
{
    public class LogServiceInternal : ServiceBase
    {
        protected override TraceCategory TraceCategory => Setup.Constants.TraceCategories.Legacy;

        public LogServiceInternal(ITraceLogger traceLogger) : base(traceLogger)
        {
        }
    }
}

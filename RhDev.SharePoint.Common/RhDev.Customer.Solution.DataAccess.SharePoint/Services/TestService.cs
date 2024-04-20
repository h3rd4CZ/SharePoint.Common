using RhDev.SharePoint.Common.Caching.Composition;
using RhDev.SharePoint.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhDev.Customer.Solution.Common.DataAccess.SharePoint.Services
{
    public class TestService : LogServiceInternal, ITestService
    {
        public TestService(ITraceLogger traceLogger) : base(traceLogger)
        {
        }

        public void DoAction()
        {
            WriteTrace("Test msg from SVC");

            WriteUnexpectedTrace(new StackOverflowException("Stack has overflowed"), "Quiet funny message here please...");
        }
    }
}

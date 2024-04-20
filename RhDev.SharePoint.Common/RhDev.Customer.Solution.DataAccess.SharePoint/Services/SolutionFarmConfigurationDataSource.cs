using RhDev.SharePoint.Common.DataAccess.SharePoint.Configuration;
using RhDev.SharePoint.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhDev.Customer.Solution.Common.DataAccess.SharePoint.Services
{
    public class SolutionFarmPropertiesDataSource : FarmPropertiesDataSource
    {
        public SolutionFarmPropertiesDataSource(ITraceLogger traceLogger) : base(traceLogger)
        {
        }

        protected override string FarmConfigPrefix => "Customer_Solution";
    }
}

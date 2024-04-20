using RhDev.SharePoint.Common.DataAccess.SharePoint.Configuration;
using RhDev.SharePoint.Common.Logging;
using $ext_safeprojectname$.Common.Setup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace $ext_safeprojectname$.Common.DataAccess.SharePoint
{
    public class SolutionFarmPropertiesDataSource : FarmPropertiesDataSource
    {

        protected override string FarmConfigPrefix => Const.SOLUTION_DISPLAY;

        public SolutionFarmPropertiesDataSource(ITraceLogger traceLogger) : base(traceLogger)
        {
        }
    }
}

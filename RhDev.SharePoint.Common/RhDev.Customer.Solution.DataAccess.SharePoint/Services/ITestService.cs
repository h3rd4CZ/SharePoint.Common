using RhDev.SharePoint.Common.Caching.Composition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RhDev.Customer.Solution.Common.DataAccess.SharePoint.Services
{
    public interface ITestService : IAutoRegisteredService
    {
        void DoAction();
    }
}

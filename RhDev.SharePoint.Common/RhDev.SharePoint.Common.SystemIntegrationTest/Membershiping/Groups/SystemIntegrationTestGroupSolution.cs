using RhDev.SharePoint.Common.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhDev.SharePoint.Common.SystemIntegrationTest.Membershiping.Groups
{
    public abstract class SystemIntegrationTestGroupSolution : ApplicationGroup
    {
        public const string SOLUTION_NAME = "Test-Integration";
        public override string ApplicationName => SOLUTION_NAME;
    }
}

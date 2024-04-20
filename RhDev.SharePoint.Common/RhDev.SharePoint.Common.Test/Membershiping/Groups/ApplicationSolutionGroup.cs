using RhDev.SharePoint.Common.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhDev.SharePoint.Common.Test.Membershiping.Groups
{
    public abstract class ApplicationSolutionGroup : ApplicationGroup
    {
        public const string SOLUTION_NAME = "Test solution";
        public override string ApplicationName => SOLUTION_NAME;
    }
}

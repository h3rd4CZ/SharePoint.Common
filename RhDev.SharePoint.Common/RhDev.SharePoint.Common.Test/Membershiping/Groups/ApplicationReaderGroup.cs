using RhDev.SharePoint.Common.Test.Membershiping.Groups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhDev.SharePoint.Common.Test.Membershiping.Groups
{
    public class ApplicationReaderGroup : ApplicationSolutionGroup
    {
        public override string Name => "Reader";
        public override string Description => "Reader can read";
    }
}

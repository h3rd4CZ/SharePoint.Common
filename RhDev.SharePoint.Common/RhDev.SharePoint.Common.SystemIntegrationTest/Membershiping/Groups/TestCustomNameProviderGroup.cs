using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhDev.SharePoint.Common.SystemIntegrationTest.Membershiping.Groups
{
    public class TestCustomNameProviderGroup : SystemIntegrationTestGroupSolution
    {
        public override string Name => "Manipulant";

        public override string Description => "Může manipulovat";

        public override Func<string, string> CustomNameProvider => w => $"Custom manipulant {w}";
    }
}

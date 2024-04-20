using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RhDev.SharePoint.Common.Composition;
using RhDev.SharePoint.Common.Security;
using RhDev.SharePoint.Common.Test.Membershiping.Groups;
using RhDev.SharePoint.Common.Test.Setup;
using StructureMap;

namespace RhDev.SharePoint.Common.Test.Membershiping
{
    [TestClass]
    public class MembershipTest
    {
        [TestMethod]
        public void GroupNameTest()
        {
            var container = RootSetup.ContainerSetup;

            var hgp = container.Backend.GetInstance<IHierarchicalGroupProvider>();

            var reader = new ApplicationReaderGroup();

            var gd = hgp.GetDefinition("Web1", reader);

            Assert.AreEqual($"{ApplicationSolutionGroup.SOLUTION_NAME} {reader.Name} Web1", gd.Name);
        }

        [TestMethod]
        public void GroupNameTestWithCustomGroupNameProvider()
        {
            var container = RootSetup.ContainerSetup;

            var hgp = container.Backend.GetInstance<IHierarchicalGroupProvider>();

            var writer = new ApplicationWriterGroup();

            var gd = hgp.GetDefinition("Web1", writer);

            Assert.AreEqual(ApplicationWriterGroup.GroupNameCustom, gd.Name);
        }
    }
}

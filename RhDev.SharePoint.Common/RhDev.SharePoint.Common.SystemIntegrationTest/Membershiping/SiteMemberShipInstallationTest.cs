using FluentAssertions;
using Microsoft.SharePoint;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RhDev.SharePoint.Common.Composition.Factory;
using RhDev.SharePoint.Common.Composition.Factory.Definitions;

namespace RhDev.SharePoint.Common.SystemIntegrationTest.Membershiping
{
    [TestClass]
    public class SiteMemberShipInstallationTest
    {
        [TestMethod]
        public void InstallSiteGroupMemberShip()
        {
            var container = ApplicationContainerFactory.Create(ContainerRegistrationDefinition.Empty);

            var gms = container.Backend.GetInstance<SiteGroupMembershipSetup>();

            gms.Should().NotBeNull();

            using (SPSite site = new SPSite("http://rh-sps16/sites/common"))
            using (SPWeb web = site.OpenWeb()) gms.EnsureSiteGroups(web);
        }
    }
}

using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RhDev.SharePoint.Common.Composition.Factory;
using RhDev.SharePoint.Common.Composition.Factory.Definitions;
using RhDev.SharePoint.Common.Configuration;
using RhDev.SharePoint.Common.DataAccess.SharePoint.Configuration.Objects;
using RhDev.SharePoint.Common.Test;
using System;

namespace RhDev.SharePoint.Common.SystemIntegrationTest.Confguration
{
    [TestClass]
    public class ConfugurationTest : UnitTestOf<TestConfigurationObject>
    {
        [TestMethod]
        public void WriteToConfigShouldNotEndsUpWithDisaster()
        {
            var container = ApplicationContainerFactory.Create(ContainerRegistrationDefinition.Empty, 
                postBuildActionsBackend: c => 
                {
                    c.Inject(typeof(TestConfigurationObject), new TestConfigurationObject(c.GetInstance<IConfigurationDataSource>()));
                });

            var fc = container.Backend.GetInstance<FarmConfiguration>();
            fc.AppSiteUrl = Const.SYSTEMINTEGRATIONTEST_URL;

            var co = container.Backend.GetInstance<TestConfigurationObject>();

            Assert.IsNotNull(co, nameof(co));

            co.Counter = "22";

            co.Counter.Should().Be("22");
        }

        [TestMethod]
        public void UsingConfigurationFactoryWithoutFallbackUrlShouldThrowsException()
        {
            var container = ApplicationContainerFactory.Create(ContainerRegistrationDefinition.Empty);

            var cm = container.Frontend.GetInstance<IConfigurationManager<TestConfigurationObject>>();

            cm.Should().NotBeNull();

            Assert.ThrowsException<InvalidOperationException>(() =>cm.GetConfiguration(c => c.Counter));
        }


        [TestMethod]
        public void UsingConfigurationFactoryWithFallbackUrlShouldNotThrowsException()
        {
            var container = ApplicationContainerFactory.Create(ContainerRegistrationDefinition.Empty);

            var cm = container.Frontend.GetInstance<IConfigurationManager<TestConfigurationObject>>();

            cm.Should().NotBeNull();

            var counter = cm.GetConfiguration(c => c.Counter, Const.SYSTEMINTEGRATIONTEST_URL);
        }


        [TestMethod]
        public void ReadWriteConnectionStringFromGlobalConfigShouldBeSafe()
        {
            var container = ApplicationContainerFactory.Create(ContainerRegistrationDefinition.Empty);

            var fc = container.Frontend.GetInstance<FarmConfiguration>();
            fc.AppSiteUrl = Const.SYSTEMINTEGRATIONTEST_URL;

            var cm = container.Frontend.GetInstance<GlobalConfiguration>();
            var css = "abcd";
            cm.ConnectionString = css;

            var cs = cm.ConnectionString;

            cs.Should().BeEquivalentTo(css);
        }
    }
}

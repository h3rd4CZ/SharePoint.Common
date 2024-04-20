using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RhDev.Customer.Solution.Common.DataAccess.ActiveDirectory.Services;
using RhDev.Customer.Solution.Common.DataAccess.SharePoint.Services;
using RhDev.Customer.Solution.ComponentX.LayerY.Services;
using RhDev.SharePoint.Common.Composition.Factory;
using RhDev.SharePoint.Common.Composition.Factory.Definitions;
using RhDev.SharePoint.Common.Configuration;
using RhDev.SharePoint.Common.DataAccess.Repository.Entities;
using RhDev.SharePoint.Common.Logging;
using RhDev.SharePoint.Common.Test.Configuration;
using System.Linq;

namespace RhDev.SharePoint.Common.Test.Composition
{
    [TestClass]
    public class ClientSolutionCompositionTest
    {

        ContainerRegistrationDefinition GetClientSolutionContainerRegistration() =>
            RhDev.Customer.Solution.Common.DataAccess.SharePoint.Const.GetClientSolutionContainerRegistration();

        [TestMethod]
        public void BuildClientSolutionCompositionWithServiceTest()
        {
            var containerDefinition = GetClientSolutionContainerRegistration();

            var container = ApplicationContainerFactory.Create(containerDefinition);

            var testService = container.Frontend.GetInstance<ITestService>();

            Assert.IsNotNull(testService);
        }

        [TestMethod]
        public void CheckSolutionWithoutCacheConfigurationCacheStrategyIsBeingApplied()
        {
            var containerDefinition = GetClientSolutionContainerRegistration();

            FakeListConfigurationDataSource fds = default;

            var container = ApplicationContainerFactory.Create(containerDefinition, postBuildActionsFrontend: c =>
            c.Inject(typeof(IConfigurationDataSource),
                fds = new FakeListConfigurationDataSource(new ConsoleTraceLogger())));

            var fakeConfig = container.Frontend.GetInstance<FakeConfigurationObject>();

            fds.ConfigurationRepository.Add(new ApplicationConfiguration { Key = fakeConfig.FakePropertyKey.ToString(), Value = "AAA" });

            Assert.AreEqual("AAA", fakeConfig.MyProperty);

            fds.ConfigurationRepository.FirstOrDefault(c => c.Key == fakeConfig.FakePropertyKey.ToString()).Value = "BBB";

            Assert.AreEqual("BBB", fakeConfig.MyProperty);

        }

        [TestMethod]
        public void CheckSolutionWithCacheConfigurationCacheStrategyInBackendContainerIsBeingApplied()
        {
            var containerDefinition = GetClientSolutionContainerRegistration();

            FakeListConfigurationDataSource fds = default;

            var container = ApplicationContainerFactory.Create(containerDefinition, postBuildActionsBackend: c =>
            c.Inject(typeof(IConfigurationDataSource),
                fds = new FakeListConfigurationDataSource(new ConsoleTraceLogger())));

            var fakeConfig = container.Backend.GetInstance<FakeConfigurationObject>();

            fds.ConfigurationRepository.Add(new ApplicationConfiguration { Key = fakeConfig.FakePropertyKey.ToString(), Value = "AAA" });

            Assert.AreEqual("AAA", fakeConfig.MyProperty);

            fds.ConfigurationRepository.FirstOrDefault(c => c.Key == fakeConfig.FakePropertyKey.ToString()).Value = "BBB";

            Assert.AreEqual("AAA", fakeConfig.MyProperty);

        }

        [TestMethod]
        public void TestClientSolutionLogging()
        {
            var containerDefinition = GetClientSolutionContainerRegistration();
            var container = ApplicationContainerFactory.Create(containerDefinition);

            var tl = container.Frontend.GetInstance<ITraceLogger>();

            Assert.IsNotNull(tl);
            Assert.AreEqual(tl.GetType(), typeof(SolutionSharePointTraceLogger));
        }

        [TestMethod]
        public void TestLayersAndComponentsInContainerRegistration()
        {
            var containerDefinition = GetClientSolutionContainerRegistration();
            var container = ApplicationContainerFactory.Create(containerDefinition);

            Assert.IsNotNull(container.Frontend);
            Assert.IsNotNull(container.Backend);

            var adSvcFrontend = container.Frontend.GetInstance<IActiveDirectoryService>();
            var adSvcBackend = container.Frontend.GetInstance<IActiveDirectoryService>();

            Assert.IsNotNull(adSvcFrontend);
            Assert.IsNotNull(adSvcBackend);

            Assert.AreEqual(typeof(ActiveDirectoryService), adSvcBackend.GetType());

            var fooFrontend = container.Frontend.GetInstance<IFoo>();
            var fooBackend = container.Backend.GetInstance<IFoo>();

            Assert.IsNotNull(fooFrontend);
            Assert.IsNotNull(fooBackend);

            Assert.AreEqual(typeof(Foo), fooBackend.GetType());

            var fooFrontend1 = container.Frontend.GetInstance<IFoo>();
            var fooFrontend2 = container.Frontend.GetInstance<IFoo>();

            Assert.IsFalse(ReferenceEquals(fooFrontend1, fooFrontend2));
            
            //IFoo is registered as singleton in backend container
            var fooBackend1 = container.Backend.GetInstance<IFoo>();
            var fooBackend2 = container.Backend.GetInstance<IFoo>();

            Assert.IsTrue(ReferenceEquals(fooBackend1, fooBackend2));
        }

        [TestMethod]
        public void GlobalConfigurationObjectShouldOverrideRhDevGlobalConfiguration()
        {
            var containerDefinition = GetClientSolutionContainerRegistration();
            var container = ApplicationContainerFactory.Create(containerDefinition);

            var globalConfig = container.Frontend.GetInstance<DataAccess.SharePoint.Configuration.Objects.GlobalConfiguration>();

            globalConfig.Should().NotBeNull();

            globalConfig.GetType().Should().Be(typeof(Customer.Solution.Common.DataAccess.ActiveDirectory.Services.GlobalConfiguration));
        }
    }
}

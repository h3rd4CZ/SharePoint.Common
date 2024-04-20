using Microsoft.VisualStudio.TestTools.UnitTesting;
using RhDev.SharePoint.Common.Composition.Factory;
using RhDev.SharePoint.Common.Composition.Factory.Definitions;
using RhDev.SharePoint.Common.Configuration;
using RhDev.SharePoint.Common.DataAccess.Repository.Entities;
using RhDev.SharePoint.Common.Logging;
using System.Collections.Generic;
using System.Linq;

namespace RhDev.SharePoint.Common.Test.Configuration
{
    [TestClass]
    public class ListConfigurationDataSourceTest
    {
                
        [TestMethod]
        public void TestConfigurationCachedInFrontendContainer()
        {
            FakeListConfigurationDataSource fds = default;

            var container = ApplicationContainerFactory.Create(new ContainerRegistrationDefinition("Test.Solution", new List<ContainerRegistrationComponentDefinition> { }),
                postBuildActionsFrontend: c => {
                    c.Inject(typeof(ITraceLogger), new ConsoleTraceLogger());
                    c.Inject(typeof(IConfigurationDataSource),
                    fds = new FakeListConfigurationDataSource(c.GetInstance<ITraceLogger>()));
                });

            var fakeConfig = container.Backend.GetInstance<FakeConfigurationObject>();

            var fko = container.Frontend.GetInstance<FakeConfigurationObject>();
            
            fds.ConfigurationRepository.Add(new ApplicationConfiguration { Key = fko.FakePropertyKey.ToString(), Value = "AAA" });

            Assert.AreEqual("AAA", fko.MyProperty);
                        
            fds.ConfigurationRepository.FirstOrDefault(c => c.Key == fko.FakePropertyKey.ToString()).Value = "BBB";

            Assert.AreEqual("AAA", fko.MyProperty);
        }

        [TestMethod]
        public void TestConfigurationNotCachedInBackendContainer()
        {
            FakeListConfigurationDataSource fds = default;

            var container = ApplicationContainerFactory.Create(new ContainerRegistrationDefinition("Test.Solution", new List<ContainerRegistrationComponentDefinition> { }),
                postBuildActionsBackend: c =>
                {
                    c.Inject(typeof(ITraceLogger), new ConsoleTraceLogger());
                    c.Inject(typeof(IConfigurationDataSource),
                        fds = new FakeListConfigurationDataSource(c.GetInstance<ITraceLogger>()));
                });

            var fko = container.Backend.GetInstance<FakeConfigurationObject>();
            
            fds.ConfigurationRepository.Add(new ApplicationConfiguration { Key = fko.FakePropertyKey.ToString(), Value = "AAA" });

            Assert.AreEqual("AAA", fko.MyProperty);

            fds.ConfigurationRepository.FirstOrDefault(c => c.Key == fko.FakePropertyKey.ToString()).Value = "BBB";

            Assert.AreEqual("BBB", fko.MyProperty);
        }

        [TestMethod]
        public void WriteListConfiguration()
        {
            FakeListConfigurationDataSource fds = default;

            var container = ApplicationContainerFactory.Create(new ContainerRegistrationDefinition("Test.Solution", new List<ContainerRegistrationComponentDefinition> { }),
                postBuildActionsBackend: c =>
                {
                    c.Inject(typeof(ITraceLogger), new ConsoleTraceLogger());
                    c.Inject(typeof(IConfigurationDataSource),
                        fds = new FakeListConfigurationDataSource(c.GetInstance<ITraceLogger>()));
                });

            var obj = container.Backend.GetInstance<FakeConfigurationObject>();
                        
            var prop = obj.MyProperty;

            Assert.AreEqual(prop, string.Empty);

            var propVal = "ABCD";

            obj.MyProperty = propVal;

            Assert.AreEqual(propVal, obj.MyProperty);
        }
    }
}

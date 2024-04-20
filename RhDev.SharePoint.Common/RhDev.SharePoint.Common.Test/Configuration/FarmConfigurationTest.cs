using Microsoft.VisualStudio.TestTools.UnitTesting;
using RhDev.SharePoint.Common.Composition.Factory;
using RhDev.SharePoint.Common.Composition.Factory.Definitions;
using RhDev.SharePoint.Common.Configuration;
using RhDev.SharePoint.Common.Logging;
using System.Collections.Generic;

namespace RhDev.SharePoint.Common.Test.Configuration
{
    [TestClass]
    public class FarmConfigurationTest
    {
        private IApplicationContainerSetup GetContainer()
        {
            return ApplicationContainerFactory.Create(new ContainerRegistrationDefinition("Test.Solution", new List<ContainerRegistrationComponentDefinition> { }), postBuildActionsFrontend: c =>
                 c.Configure(
                     cc =>
                     {
                         cc.For<ITraceLogger>().Use<ConsoleTraceLogger>();
                         cc.For<FarmConfiguration>().Use<FarmConfiguration>().Ctor<IConfigurationDataSource>().Is<FakeFarmPropertiesDataSource>();
                     }
                ));
        }

        [TestMethod]
        public void CompositionTest()
        {
            var container = GetContainer();

            Assert.IsNotNull(container);
        }

        [TestMethod]
        public void WriteFarmConfig()
        {
            var container = GetContainer();

            var fc = container.Frontend.GetInstance<FarmConfiguration>();

            var appSiteAUrl = "https://abcd";

            fc.AppSiteUrl = appSiteAUrl;
            fc.SaveChanges();
                        
            Assert.AreEqual(fc.AppSiteUrl, appSiteAUrl);
        }
    }
}

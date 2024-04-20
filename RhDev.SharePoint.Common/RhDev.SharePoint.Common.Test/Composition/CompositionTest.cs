using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RhDev.SharePoint.Common.Composition.Factory;
using RhDev.SharePoint.Common.Composition.Factory.Definitions;
using RhDev.SharePoint.Common.DataAccess;
using RhDev.SharePoint.Common.DataAccess.Repository;
using RhDev.SharePoint.Common.DataAccess.SharePoint.Configuration.Objects;
using RhDev.SharePoint.Common.DataAccess.SharePoint.Logging;
using RhDev.SharePoint.Common.DataAccess.SharePoint.Repository;
using RhDev.SharePoint.Common.DataAccess.SharePoint.Security;
using RhDev.SharePoint.Common.Logging;
using RhDev.SharePoint.Common.Security;
using System.Diagnostics;

namespace RhDev.SharePoint.Common.Test.Composition
{
    [TestClass]
    public class CompositionTest
    {

        private IApplicationContainerSetup GetDefaultContainer() =>
                    ApplicationContainerFactory.Create(ContainerRegistrationDefinition.Empty);

        public IHierarchicalGroupProvider Hgp { get; set; }

        [TestMethod]
        public void BuildTargetTest()
        {
            var defaultContainer = GetDefaultContainer();

            defaultContainer.Backend.BuildUp(this);

            Assert.IsNotNull(Hgp);
        }

        [TestMethod]
        public void BuildContainerForLibraryShouldNotEndsUpWithDisaster()
        {
            var sw = new Stopwatch();
            sw.Start();
            var defaultContainer = GetDefaultContainer();
            sw.Stop();

            Debug.WriteLine($"RhDev :: Container built in : {sw.ElapsedMilliseconds} ms");

            Assert.IsNotNull(defaultContainer);

            Assert.IsNotNull(defaultContainer.Backend);
            Assert.IsNotNull(defaultContainer.Frontend);
        }

        [TestMethod]
        public void BuildContainerAndTestCommonServices()
        {
            var container = GetDefaultContainer();

            var sc = container.Frontend.GetInstance<ISecurityContext>();
            Assert.AreEqual(typeof(FrontEndSecurityContext), sc.GetType());

            var scBackend = container.Backend.GetInstance<ISecurityContext>();
            Assert.AreEqual(typeof(TimerJobSecurityContext), scBackend.GetType());
                        
            Assert.AreEqual(typeof(SharePointTraceLogger), container.Backend.GetInstance<ITraceLogger>().GetType());
            Assert.AreEqual(typeof(SharePointTraceLogger), container.Frontend.GetInstance<ITraceLogger>().GetType());
        }

        [TestMethod]
        public void GlobalConfigurationTestObject()
        {
            var container = GetDefaultContainer();
            var wfe = container.Frontend;
            var bac = container.Backend;

            var wfeConfig = wfe.GetInstance<GlobalConfiguration>();
            var bacConfig = bac.GetInstance<GlobalConfiguration>();

            wfeConfig.Should().NotBeNull();
            bacConfig.Should().NotBeNull();
        }

        [TestMethod]
        public void TestCommonRepositoryFactory()
        {
            var container = GetDefaultContainer();
            var wfe = container.Frontend;

            var factory = wfe.GetInstance<ICommonRepositoryFactory>();

            factory.Should().NotBeNull();

            var dayOffRepository = factory.GetRepository<IDayOffRepository>("http://TESTURL");
            var dayOffRepositoryBase = factory.GetRepository<DayOffConfigurationRepository>("http://TESTURL");

            dayOffRepository.Should().NotBeNull();
            dayOffRepositoryBase.Should().NotBeNull();

            dayOffRepository.Should().BeOfType(dayOffRepository.GetType());
        }
    }
}

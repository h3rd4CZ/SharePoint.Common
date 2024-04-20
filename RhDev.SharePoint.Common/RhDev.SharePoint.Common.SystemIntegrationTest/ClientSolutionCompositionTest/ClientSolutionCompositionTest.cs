using Microsoft.VisualStudio.TestTools.UnitTesting;
using RhDev.Customer.Solution.Common.DataAccess.SharePoint.Services;
using RhDev.SharePoint.Common.Composition.Factory;
using RhDev.SharePoint.Common.Composition.Factory.Definitions;
using RhDev.SharePoint.Common.Configuration;
using RhDev.SharePoint.Common.DataAccess.SharePoint.Utils;
using RhDev.SharePoint.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhDev.SharePoint.Common.SystemIntegrationTest.ClientSolutionCompositionTest
{
    [TestClass]
    public class ClientSolutionCompositionTest
    {
        ContainerRegistrationDefinition GetClientSolutionContainerRegistration() =>
            RhDev.Customer.Solution.Common.DataAccess.SharePoint.Const.GetClientSolutionContainerRegistration();

        [TestMethod]
        public void TestCustomFarmConfigurationDataSource()
        {
            var containerDefinition = GetClientSolutionContainerRegistration();
            var container = ApplicationContainerFactory.Create(containerDefinition);

            var fc = container.Frontend.GetInstance<FarmConfiguration>();

            var siteUrl = "https://ab.cz";

            fc.AppSiteUrl = siteUrl;

            Assert.AreEqual(siteUrl, fc.AppSiteUrl);
        }

        [TestMethod]
        public void TestLibraryLoggingWithOverridedSharepointTraceLogger()
        {
            var containerDefinition = GetClientSolutionContainerRegistration();
            var container = ApplicationContainerFactory.Create(containerDefinition);

            var tl = container.Frontend.GetInstance<ITraceLogger>();

            new DisabledEventReceiversScope(tl);
        }

        [TestMethod]
        public void RegisterSolutionTraceloggerAndLogUsingIt()
        {
            var containerDefinition = GetClientSolutionContainerRegistration();
            var container = ApplicationContainerFactory.Create(containerDefinition);

            var tl = container.Frontend.GetInstance<ITraceLogger>();

            tl.Unregister();
            tl.Register();

            var dc = Customer.Solution.Common.DataAccess.SharePoint.Setup.Constants.GetDiagnosticsServiceConfiguration;

            var category = new TraceCategory(dc.AvailableCategories[0].Area, dc.AvailableCategories[0].Category);

            var msg = "Test MSG from client solution";

            tl.Write(category, msg);
            tl.Event(category, msg);

            tl.Write(msg);
        }

        [TestMethod]
        public void TestLogService()
        {
            var containerDefinition = GetClientSolutionContainerRegistration();
            var container = ApplicationContainerFactory.Create(containerDefinition);

            var svc = container.Frontend.GetInstance<ITestService>();

            svc.DoAction();
        }
    }
}

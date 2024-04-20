using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RhDev.SharePoint.Common.Composition.Factory;
using RhDev.SharePoint.Common.Composition.Factory.Definitions;
using RhDev.SharePoint.Common.Logging;

namespace RhDev.SharePoint.Common.Test.SystemIntegrationTest
{
    [TestClass]
    public class LoggingTest
    {
        [TestMethod]
        public void RegisterService()
        {
            var container = ApplicationContainerFactory.Create(new ContainerRegistrationDefinition("Test.Solution", new List<ContainerRegistrationComponentDefinition> { }));

            var tl = container.Backend.GetInstance<ITraceLogger>();

            tl.Register();
        }

        [TestMethod]
        public void UnregisterService()
        {
            var container = ApplicationContainerFactory.Create(new ContainerRegistrationDefinition("Test.Solution", new List<ContainerRegistrationComponentDefinition> { }));

            var tl = container.Backend.GetInstance<ITraceLogger>();

            tl.Unregister();
        }

        [TestMethod]
        public void RegisterCustomLoggingService()
        {
            var container = ApplicationContainerFactory.Create(
                new ContainerRegistrationDefinition("Test.Solution",
                    new List<ContainerRegistrationComponentDefinition> { }), postBuildActionsBackend : c => c.Inject(typeof(ITraceLogger), new SolutionSharePointTraceLogger()));

            var tl = container.Backend.GetInstance<ITraceLogger>();
            
            tl = container.Backend.GetInstance<ITraceLogger>();

            tl.Register();
        }

        [TestMethod]
        public void LogUsingCustomTraceLogger()
        {
            var container = ApplicationContainerFactory.Create(
                new ContainerRegistrationDefinition("Test.Solution",
                    new List<ContainerRegistrationComponentDefinition> { }), postBuildActionsBackend: c => c.Inject(typeof(ITraceLogger), new SolutionSharePointTraceLogger()));

            var tl = container.Backend.GetInstance<ITraceLogger>();

            tl = container.Backend.GetInstance<ITraceLogger>();

            tl.Write(new TraceCategory("SuperArea", "CategoryX"), "Super message");
        }

        [TestMethod]
        public void LogUsingCustomTraceLoggerWithoutLogCategoryShoouldTakeDefaultSolitaCategory()
        {
            var container = ApplicationContainerFactory.Create(
                new ContainerRegistrationDefinition("Test.Solution",
                    new List<ContainerRegistrationComponentDefinition> { }), postBuildActionsBackend: c => c.Inject(typeof(ITraceLogger), new SolutionSharePointTraceLogger()));

            var tl = container.Backend.GetInstance<ITraceLogger>();

            tl = container.Backend.GetInstance<ITraceLogger>();

            tl.Write("Super message default");
        }

        [TestMethod]
        public void LogUsingCustomTraceLoggerWithoutLogCategoryWithDefaultcategorySpecifiedShouldTakeThatSolitaCategory()
        {
            var container = ApplicationContainerFactory.Create(
                new ContainerRegistrationDefinition("Test.Solution",
                    new List<ContainerRegistrationComponentDefinition> { }), postBuildActionsBackend: c => c.Inject(typeof(ITraceLogger), new SolutionSharePointTraceLogger()));

            var tl = container.Backend.GetInstance<ITraceLogger>();

            tl = container.Backend.GetInstance<ITraceLogger>();
                        
            tl.Write(TraceCategories.Integration, "Super message default with integration category");
        }
    }
}

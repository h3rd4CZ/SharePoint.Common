using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RhDev.SharePoint.Common.Composition.Factory;
using RhDev.SharePoint.Common.Composition.Factory.Definitions;
using RhDev.SharePoint.Common.Logging;

namespace RhDev.SharePoint.Common.SystemIntegrationTest.DayOffs
{
    [TestClass]
    public class DayOffTest
    {
        [TestMethod]
        public void DayOffTestShouldNotEndsUpWithDisaster()
        {
            var container = 
                ApplicationContainerFactory.Create(ContainerRegistrationDefinition.Empty, postBuildActionsFrontend: c => c.Inject(typeof(ITraceLogger), new ConsoleTraceLogger()));

            var dop = container.Frontend.GetInstance<IDayOffProvider>();
            var ccp = container.Frontend.GetInstance<ICentralClockProvider>();
            var tl = container.Frontend.GetInstance<ITraceLogger>();

            bool isDayOff = dop.IsDayOff(ccp.Now());

            tl.Write("ISDAYOFF : " + isDayOff);
        }

        [TestMethod]
        public void NumOfDayOffsThroughXMassShouldBe3()
        {
            var container =
                ApplicationContainerFactory
                .Create(ContainerRegistrationDefinition.Empty, postBuildActionsFrontend: c => c.Inject(typeof(ITraceLogger), new ConsoleTraceLogger()));

            var dop = container.Frontend.GetInstance<IDayOffProvider>();
            var ccp = container.Frontend.GetInstance<ICentralClockProvider>();

            var nod = dop
                .GetDayOffsCountFromDateRange(CentralClock.FillFromDateTime(new System.DateTime(2020, 12, 24)), CentralClock.FillFromDateTime(new System.DateTime(2020, 12, 26)), null, 1029);

            nod.Should().Be(3);
        }
    }
}

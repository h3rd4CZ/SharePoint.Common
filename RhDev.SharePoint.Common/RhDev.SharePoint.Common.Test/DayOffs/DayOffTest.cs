using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using RhDev.SharePoint.Common.DataAccess;
using RhDev.SharePoint.Common.DataAccess.SharePoint;

namespace RhDev.SharePoint.Common.Test.DayOffs
{
    [TestClass]
    public class DayOffTest : UnitTestOf<DayOffProvider>
    {
        protected override void Setup()
        {
            base.Setup();

            var crp = Mocker.Get<ICommonRepositoryFactory>();
            crp.GetDayOffRepository().ReturnsForAnyArgs(new DayOffMockList());
        }

        [TestMethod]
        public void IsTodayDayOff()
        {
            var sut = SUT;
            var result = sut.IsDayOff(CentralClock.FillFromDateTime(new System.DateTime(2020, 8, 10)));

            result.Should().BeTrue();
        }

        [TestMethod]
        public void IsTodayDayOffWithNoRepeat()
        {
            var sut = SUT;
            var result = sut.IsDayOff(CentralClock.FillFromDateTime(new System.DateTime(2020, 8, 11)));

            result.Should().BeFalse();
        }
    }
}

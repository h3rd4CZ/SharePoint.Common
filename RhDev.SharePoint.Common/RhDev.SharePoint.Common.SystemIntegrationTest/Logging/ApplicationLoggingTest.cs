using Microsoft.VisualStudio.TestTools.UnitTesting;
using RhDev.SharePoint.Common.Composition;
using RhDev.SharePoint.Common.DataAccess;

namespace RhDev.SharePoint.Common.SystemIntegrationTest.Logging
{
    [TestClass]
    public class ApplicationLoggingTest
    {
        [TestMethod]
        public void LogApplicationEntryShouldNotEndsUpWithDisaster()
        {
            var logMgr = CommonContainerRoot.Get.Backend.GetInstance<IApplicationLogManager>();

            Assert.IsNotNull(logMgr, nameof(logMgr));
            
            logMgr.WriteLog("Test message", "TEST", "http://rh-sps16/sites/common", DataAccess.Repository.Entities.ApplicationLogLevel.Debug);
        }
    }
}

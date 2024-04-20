using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RhDev.SharePoint.Common.Test
{
    public abstract class UnitTestBase
    {
        [TestInitialize]
        public void SetupTest()
        {
            OverrideSharePointTraceLoggerSingleton();
            Setup();
        }

        private static void OverrideSharePointTraceLoggerSingleton()
        {
            
        }

        protected virtual void Setup()
        {
        }
    }
}

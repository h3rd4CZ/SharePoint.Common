using Microsoft.SharePoint;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhDev.SharePoint.Common.SystemIntegrationTest.FeatureInstallation
{
    [TestClass]
    public class FeatureInstallationTest
    {
        [TestMethod]
        public void ExecuteFakeFeatureInstallation()
        {
            var fi = new FakeFeatureReceiver();

            var fp = new SPFeatureReceiverProperties();
        }
    }
}

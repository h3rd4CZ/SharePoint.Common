using Microsoft.SharePoint;
using RhDev.SharePoint.Common.DataAccess.SharePoint.Installation;
using System;

namespace RhDev.SharePoint.Common.SystemIntegrationTest.FeatureInstallation
{
    public class FakeFeatureInstallation : FeatureInstallation<SPWeb>
    {
        protected override void DoExecuteInstallation(SPWeb scope)
        {
            Console.WriteLine($"Fake feature installation started at : {DateTime.Now.ToLongTimeString()}");
        }
    }
}

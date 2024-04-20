using RhDev.SharePoint.Common.Security;

namespace RhDev.SharePoint.Common.SystemIntegrationTest.Membershiping.Groups
{
    public static class SystemIntegrationGroups
    {
        public static ApplicationGroup Reader = new TestReader();
        public static ApplicationGroup Writer = new TestWriter();
        public static ApplicationGroup Manipulant = new TestCustomNameProviderGroup();
    }
}

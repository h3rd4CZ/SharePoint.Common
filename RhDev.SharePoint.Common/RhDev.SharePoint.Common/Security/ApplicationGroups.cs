namespace RhDev.SharePoint.Common.Security
{
    public static class ApplicationGroups
    {
        public static ApplicationGroup Administrator { get; } = new ApplicationAdministratorGroup();
    }
}

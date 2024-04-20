namespace RhDev.SharePoint.Common.Configuration
{
    public class FarmConfiguration : ConfigurationObject
    {
        private const string CONFIGURATION_MODULE_NAME = "Farm";

        private static readonly ConfigurationKey AppSiteUrlKey = new ConfigurationKey(CONFIGURATION_MODULE_NAME, "AppSiteUrl");
        private static readonly ConfigurationKey NotificationRedirectKey = new ConfigurationKey(CONFIGURATION_MODULE_NAME, "NotificationRedirect");

        public virtual string AppSiteUrl
        {
            get { return DataSource.GetRequiredValue(AppSiteUrlKey).AsString; }
            set { DataSource.SetValue(AppSiteUrlKey, value); }
        }

        public virtual string NotificationRedirect
        {
            get { return DataSource.GetValue(NotificationRedirectKey, string.Empty).AsString; }
            set { DataSource.SetValue(NotificationRedirectKey, value); }
        }

        public FarmConfiguration(IConfigurationDataSource dataSource)
            : base(dataSource)
        {
        }
    }
}

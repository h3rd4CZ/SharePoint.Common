using RhDev.SharePoint.Common.Configuration;

namespace RhDev.SharePoint.Common.Impl.Caching
{
    public class ExpirationCacheConfiguration : ConfigurationObject
    {
        private const string ConfigurationModuleName = "ExpirationCacheConfiguration";

        private static readonly ConfigurationKey ExpirationDurationInMinutesKey = new ConfigurationKey(ConfigurationModuleName, "ExpireItemsAfterMinutes");

        public int ExpirationDurationInMinutes
        {
            get { return DataSource.GetValue(ExpirationDurationInMinutesKey, 60).AsInteger; }
            set { DataSource.SetValue(ExpirationDurationInMinutesKey, value); }
        }

        public ExpirationCacheConfiguration(IConfigurationDataSource dataSource)
            : base(dataSource)
        {
        }
    }
}

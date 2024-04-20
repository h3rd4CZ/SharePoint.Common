using RhDev.SharePoint.Common.Caching.Composition;

namespace RhDev.SharePoint.Common.Configuration
{
    public interface IConfigurationDataSource : IService
    {
        ConfigurationValue GetValue(ConfigurationKey key);

        ConfigurationValue GetValue(ConfigurationKey key, object defaultValue);

        ConfigurationValue GetRequiredValue(ConfigurationKey key);

        void InjectConfigWeb(string webUrl);

        void SetValue(ConfigurationKey key, object value);

        void SaveChanges();
    }
}


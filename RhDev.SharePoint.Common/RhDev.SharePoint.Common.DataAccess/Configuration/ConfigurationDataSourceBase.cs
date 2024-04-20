using RhDev.SharePoint.Common.Caching.Composition;
using RhDev.SharePoint.Common.Configuration;
using RhDev.SharePoint.Common.Logging;
using System;

namespace RhDev.SharePoint.Common.DataAccess.Configuration
{
    public abstract class ConfigurationDataSourceBase : ServiceBase, IConfigurationDataSource
    {
        protected string configWebUrl;

        public ConfigurationDataSourceBase(ITraceLogger traceLogger) : base(traceLogger) { }
        
        public ConfigurationValue GetRequiredValue(ConfigurationKey key)
        {
            ConfigurationValue value = GetValue(key);

            if (value == null)
                throw new MissingConfigurationValueException(String.Format("Configuration value with key {0} not found.", key));

            if (String.IsNullOrEmpty(value.AsString))
                throw new MissingConfigurationValueException(String.Format("Configuration value with key {0} is empty.", key));

            return value;
        }

        public ConfigurationValue GetValue(ConfigurationKey key, object defaultValue)
        {
            return GetValue(key) ?? GetDefaultValue(key, defaultValue); 
        }

        private static ConfigurationValue GetDefaultValue(ConfigurationKey key, object defaultValue)
        {
            return new ConfigurationValue(key, defaultValue);
        }

        public abstract ConfigurationValue GetValue(ConfigurationKey key);

        public virtual void SetValue(ConfigurationKey key, object value)
        {
            throw new NotSupportedException();
        }

        public virtual void SaveChanges()
        {
            throw new NotSupportedException();
        }


        public virtual void InjectConfigWeb(string webUrl)
        {
            if (string.IsNullOrEmpty(webUrl)) throw new ArgumentNullException("webUrl");

            configWebUrl = webUrl;
        }
    }
}

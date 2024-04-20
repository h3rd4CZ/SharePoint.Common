using RhDev.SharePoint.Common.Configuration;
using RhDev.SharePoint.Common.DataAccess.Configuration;
using RhDev.SharePoint.Common.Logging;
using System;
using System.Collections.Generic;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Configuration
{
    public class ConfigurationDataSourceWrapper : ConfigurationDataSourceBase
    {
        private readonly IDictionary<ConfigurationKey, ConfigurationValue> cache;
        private readonly IConfigurationCacheStrategy cacheStrategy;

        public IConfigurationDataSource WrappedDataSource { get; private set; }

        protected override TraceCategory TraceCategory => TraceCategories.Common;

        public ConfigurationDataSourceWrapper(IConfigurationDataSource dataSource, IConfigurationCacheStrategy cacheStrategy, ITraceLogger traceLogger) : base(traceLogger)
        {
            cache = new Dictionary<ConfigurationKey, ConfigurationValue>();
            WrappedDataSource = dataSource;
            this.cacheStrategy = cacheStrategy;
        }

        public override ConfigurationValue GetValue(ConfigurationKey key)
        {
            var decoratedIfAny = DecorateWithWebIfAny(key);

            if(!cacheStrategy.UsingCache) return WrappedDataSource.GetValue(key);

            lock (cache)
            {
                if (cache.ContainsKey(decoratedIfAny))
                    return cache[decoratedIfAny];
                
                var value = WrappedDataSource.GetValue(key);

                if (value != null)
                    cache[decoratedIfAny] = value;

                return value;
            }
        }
        
        public override void SetValue(ConfigurationKey key, object value)
        {
            WrappedDataSource.SetValue(key, value);

            lock (cache)
                cache.Remove(key);
        }

        public override void SaveChanges()
        {
            WrappedDataSource.SaveChanges();
        }

        private ConfigurationKey DecorateWithWebIfAny(ConfigurationKey key)
        {
            if (Equals(null, key)) throw new ArgumentNullException("key");

            if (string.IsNullOrEmpty(configWebUrl)) return key;

            var clone = (ConfigurationKey)key.Clone();
            clone.Web = configWebUrl;

            return clone;
        }

        public override void InjectConfigWeb(string webUrl)
        {
            base.InjectConfigWeb(webUrl);

            if (!string.IsNullOrEmpty(configWebUrl))
                WrappedDataSource.InjectConfigWeb(webUrl);
        }
    }
}

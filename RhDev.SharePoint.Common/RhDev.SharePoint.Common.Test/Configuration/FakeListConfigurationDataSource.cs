using RhDev.SharePoint.Common.Configuration;
using RhDev.SharePoint.Common.DataAccess.Configuration;
using RhDev.SharePoint.Common.DataAccess.Repository.Entities;
using RhDev.SharePoint.Common.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace RhDev.SharePoint.Common.Test.Configuration
{
    public class FakeListConfigurationDataSource : ConfigurationDataSourceBase
    {
        private IList<ApplicationConfiguration> configurationRepository = new List<ApplicationConfiguration>();

        public FakeListConfigurationDataSource(ITraceLogger traceLogger) :  base(traceLogger)
        {
        }

        public IList<ApplicationConfiguration> ConfigurationRepository { get => configurationRepository;  }

        private static string GetPropertyKey(ConfigurationKey key)
        {
            return key.ToString();
        }

        private static string ConvertValueToString(object value)
        {
            return Convert.ToString(value, CultureInfo.InvariantCulture);
        }

        public override ConfigurationValue GetValue(ConfigurationKey key)
        {
            //string webUrl = GetConfigWebUrl();

            string propertyKey = GetPropertyKey(key);
            ApplicationConfiguration applicationConfiguration = configurationRepository.FirstOrDefault(c => c.Key == propertyKey);

            if (applicationConfiguration == null)
                return null;

            string valueAsString = ConvertValueToString(applicationConfiguration.Value);
            var configurationValue = new ConfigurationValue(key, valueAsString);

            WriteVerboseTrace("Loaded configuration value \"{0}\" from SPList", configurationValue);
            return configurationValue;
        }

        public override void SetValue(ConfigurationKey key, object value)
        {
            //string webUrl = GetConfigWebUrl();

            string propertyKey = GetPropertyKey(key);
            string valueAsString = value != null ? ConvertValueToString(value) : null;

            WriteVerboseTrace("Setting value {0} with key {1} to SPList", value, key);

            ApplicationConfiguration applicationConfiguration = configurationRepository.FirstOrDefault(c => c.Key == propertyKey);

            if (applicationConfiguration != null)
            {
                applicationConfiguration.Value = ConvertValueToString(value);
            }
            else
            {
                applicationConfiguration = new ApplicationConfiguration(propertyKey, valueAsString, key.Module);
                configurationRepository.Add(applicationConfiguration);
            }
        }
        public override void SaveChanges()
        {

        }
    }
}

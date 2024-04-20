using System;
using System.Globalization;
using Microsoft.SharePoint.ApplicationPages.Calendar.Exchange;
using RhDev.SharePoint.Common.Configuration;
using RhDev.SharePoint.Common.DataAccess.Configuration;
using RhDev.SharePoint.Common.DataAccess.Repository.Entities;
using RhDev.SharePoint.Common.Logging;
using RhDev.SharePoint.Common.Utils;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Configuration
{
    public class ListConfigurationDataSource : ConfigurationDataSourceBase
    {
        private readonly ICommonRepositoryFactory commonRepositoryFactory;
        private readonly FarmConfiguration farmConfig;

        protected override TraceCategory TraceCategory => TraceCategories.Common;

        public ListConfigurationDataSource(ICommonRepositoryFactory commonRepositoryFactory, FarmConfiguration farmConfig, ITraceLogger traceLogger) : base(traceLogger)
        {
            this.farmConfig = farmConfig;
            this.commonRepositoryFactory = commonRepositoryFactory;
        }

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
            string webUrl = GetConfigWebUrl();

            var repository = commonRepositoryFactory.GetApplicationConfigurationRepository(webUrl);

            string propertyKey = GetPropertyKey(key);
            ApplicationConfiguration applicationConfiguration = repository.GetByKey(propertyKey);

            if (applicationConfiguration == null)
                return null;
            
            string valueAsString = ConvertValueToString(applicationConfiguration.Value);
            var configurationValue = new ConfigurationValue(key, valueAsString);

            WriteVerboseTrace("Loaded configuration value \"{0}\" from SPList", configurationValue);
            return configurationValue;
        }
        
        public override void SetValue(ConfigurationKey key, object value)
        {
            Guard.NotNull(key, nameof(key));

            string webUrl = GetConfigWebUrl();

            var repository = commonRepositoryFactory.GetApplicationConfigurationRepository(webUrl);

            string propertyKey = GetPropertyKey(key);
            string valueAsString = value != null ? ConvertValueToString(value) : null;

            WriteVerboseTrace("Setting value {0} with key {1} to SPList", value, key);

            ApplicationConfiguration applicationConfiguration = repository.GetByKey(key.ToString());

            if (applicationConfiguration != null)
            {
                applicationConfiguration.Value = ConvertValueToString(value);
                repository.UpdateConfiguration(applicationConfiguration);
            }
            else
            {
                applicationConfiguration = new ApplicationConfiguration(propertyKey, valueAsString, key.Module);
                repository.CreateConfiguration(applicationConfiguration);
            }
        }

        private string GetConfigWebUrl()
        {
            if (string.IsNullOrEmpty(configWebUrl)) return farmConfig.AppSiteUrl;

            return configWebUrl;
        }

        public override void SaveChanges()
        {
            // U SPList se zmeny projevi ihned po zavoalni SetValue
        }
    }
}

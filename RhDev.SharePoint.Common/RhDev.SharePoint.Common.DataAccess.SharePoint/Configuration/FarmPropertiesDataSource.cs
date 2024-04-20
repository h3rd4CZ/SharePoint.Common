using System;
using System.Collections;
using System.Globalization;
using Microsoft.SharePoint.Administration;
using RhDev.SharePoint.Common.Configuration;
using RhDev.SharePoint.Common.DataAccess.Configuration;
using RhDev.SharePoint.Common.Logging;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Configuration
{
    public class FarmPropertiesDataSource : ConfigurationDataSourceBase
    {
        protected override TraceCategory TraceCategory => TraceCategories.Common;

        public FarmPropertiesDataSource(ITraceLogger traceLogger) : base(traceLogger)
        {

        }

        protected virtual string FarmConfigPrefix { get; } = Constants.SOLUTION_NAME;
        private string GetPropertyKey(ConfigurationKey key)
        {
            return  $"{FarmConfigPrefix}_{key}";
        }

        private static string ConvertValueToString(object value)
        {
            return Convert.ToString(value, CultureInfo.InvariantCulture);
        }

        public override ConfigurationValue GetValue(ConfigurationKey key)
        {
            string propertyKey = GetPropertyKey(key);
            object value = SPFarm.Local.Properties[propertyKey];

            if (value == null)
                return null;

            string valueAsString = ConvertValueToString(value);
            var configurationValue = new ConfigurationValue(key, valueAsString);

            WriteVerboseTrace("Loaded configuration value \"{0}\" from Farm properties", configurationValue);
            return configurationValue;
        }

        public override void SetValue(ConfigurationKey key, object value)
        {
            string propertyKey = GetPropertyKey(key);
            string valueAsString = value != null ? ConvertValueToString(value) : null;

            WriteVerboseTrace("Setting value {0} with key {1} to Farm properties", value, key);

            Hashtable properties = SPFarm.Local.Properties;

            if (properties.ContainsKey(propertyKey))
                properties.Remove(propertyKey);

            properties.Add(propertyKey, valueAsString);
        }

        public override void SaveChanges()
        {
            WriteVerboseTrace("Updating farm properties");
            SPFarm.Local.Update();
        }
    }
}

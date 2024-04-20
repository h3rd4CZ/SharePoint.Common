using RhDev.SharePoint.Common.Configuration;

namespace RhDev.SharePoint.Common.SystemIntegrationTest.Confguration
{
    public class TestConfigurationObject : ConfigurationObject
    {
        private const string MODULE_NAME = "Test";

        private ConfigurationKey CounterKey = new ConfigurationKey(MODULE_NAME, "Counter");

        public string Counter 
        {
            get { return DataSource.GetValue(CounterKey, string.Empty).AsString; }
            set { DataSource.SetValue(CounterKey, value); }
        }

        public TestConfigurationObject(IConfigurationDataSource dataSource) : base(dataSource)
        {
        }
    }
}

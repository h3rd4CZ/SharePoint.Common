using RhDev.SharePoint.Common.Configuration;

namespace RhDev.SharePoint.Common.Test.Configuration
{
    public class FakeConfigurationObject : ConfigurationObject
    {
        private const string MODULE = "Fake";

        public ConfigurationKey FakePropertyKey = new ConfigurationKey(MODULE, "Property");

        public IConfigurationDataSource DT => DataSource;

        public string MyProperty 
        {
            get { return DataSource.GetValue(FakePropertyKey, string.Empty).AsString; }    
            set { DataSource.SetValue(FakePropertyKey, value); }
        }

        public FakeConfigurationObject(IConfigurationDataSource ds) : base(ds)
        {

        }
    }
}

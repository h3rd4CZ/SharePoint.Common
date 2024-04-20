using RhDev.SharePoint.Common.Configuration;

namespace RhDev.Customer.Solution.Common.DataAccess.ActiveDirectory.Services
{
    public class GlobalConfiguration : SharePoint.Common.DataAccess.SharePoint.Configuration.Objects.GlobalConfiguration
    {
        private static readonly ConfigurationKey LobSystemKey = new ConfigurationKey(MODULE_NAME, "LobSystemKey");

        public string LobSystem
        {
            get { return DataSource.GetValue(LobSystemKey, string.Empty).AsString; }
            set { DataSource.SetValue(LobSystemKey, value); }
        }
                
        public GlobalConfiguration(IConfigurationDataSource dataSource) : base(dataSource)
        {
        }
    }
}

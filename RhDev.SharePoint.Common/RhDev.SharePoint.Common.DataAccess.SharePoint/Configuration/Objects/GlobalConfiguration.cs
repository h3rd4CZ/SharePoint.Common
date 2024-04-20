using RhDev.SharePoint.Common.Configuration;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Configuration.Objects
{
    public class GlobalConfiguration : ConfigurationObject
    {        
        protected const string MODULE_NAME = "Global";
                      
        private static readonly ConfigurationKey ConnectionStringKey = new ConfigurationKey(MODULE_NAME, "ConnectionString");
        
        public string ConnectionString
        {
            get { return SecurityEncryptor.Decrypt(DataSource.GetValue(ConnectionStringKey, null)?.AsString); }
            set { DataSource.SetValue(ConnectionStringKey, SecurityEncryptor.Encrypt(value)); }
        }
                
        public GlobalConfiguration(IConfigurationDataSource dataSource)
            : base(dataSource)
        {
        }
    }
}

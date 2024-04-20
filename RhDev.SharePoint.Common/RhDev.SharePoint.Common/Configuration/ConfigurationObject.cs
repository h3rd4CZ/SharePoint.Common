using System;

namespace RhDev.SharePoint.Common.Configuration
{
    public class ConfigurationObject
    {
        protected string configWebUrl;

        public ConfigurationObject OnWeb(string absoluteWebUrl)
        {
            if (string.IsNullOrEmpty(absoluteWebUrl)) throw new ArgumentNullException("webUrl");

            this.configWebUrl = absoluteWebUrl;

            DataSource.InjectConfigWeb(this.configWebUrl);

            return this;
        }

        protected IConfigurationDataSource DataSource { get; private set; }

        protected ConfigurationObject(IConfigurationDataSource dataSource)
        {
            DataSource = dataSource;
        }

        public void SaveChanges()
        {
            DataSource.SaveChanges();
        }
    }
}

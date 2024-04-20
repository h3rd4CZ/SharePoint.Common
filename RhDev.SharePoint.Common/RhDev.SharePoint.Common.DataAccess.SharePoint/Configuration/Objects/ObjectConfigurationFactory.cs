using RhDev.SharePoint.Common.Configuration;
using System;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Configuration.Objects
{
    public delegate ConfigurationObject ConfigurationObjectConstructor(Type configType, string webUrl);

    public class ObjectConfigurationFactory : IObjectConfigurationFactory
    {
        private readonly ConfigurationObjectConstructor configObjectConstructor;

        public ObjectConfigurationFactory(ConfigurationObjectConstructor configurationObjectConstructor)
        {
            this.configObjectConstructor = configurationObjectConstructor;
        }

        public T GetConfigurationObject<T>() where T : ConfigurationObject
        {
            return GetConfigurationObject<T>(string.Empty);
        }

        public T GetConfigurationObject<T>(string webUrl) where T : ConfigurationObject
        {
            var configurationType = typeof (T);
            
            CheckType(configurationType);

            return (T) configObjectConstructor(configurationType, webUrl);
        }

        private void CheckType(Type configurationType)
        {
            if (!Equals(null, configurationType.BaseType) && typeof(ConfigurationObject).IsAssignableFrom(configurationType)) return;
            
            throw new InvalidOperationException("configurationType is not ConfigurationObject");
        }
    }
}

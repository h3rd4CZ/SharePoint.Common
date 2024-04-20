using RhDev.SharePoint.Common.Caching.Composition;
using RhDev.SharePoint.Common.Configuration;

namespace RhDev.SharePoint.Common
{
    public interface IObjectConfigurationFactory : IAutoRegisteredService
    {
        T GetConfigurationObject<T>() where T : ConfigurationObject;
        T GetConfigurationObject<T>(string webUrl) where T : ConfigurationObject;
    }
}

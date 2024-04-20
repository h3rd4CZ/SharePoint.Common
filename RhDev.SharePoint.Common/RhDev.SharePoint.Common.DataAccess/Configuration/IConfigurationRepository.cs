using RhDev.SharePoint.Common.Configuration;

namespace RhDev.SharePoint.Common.DataAccess.Configuration
{
    public interface IConfigurationRepository : IEntityRepository<ConfigurationValue>
    {
        ConfigurationValue GetByCode(string code);
    }
}

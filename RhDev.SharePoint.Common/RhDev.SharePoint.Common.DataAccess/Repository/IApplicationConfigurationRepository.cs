using RhDev.SharePoint.Common.DataAccess.Repository.Entities;

namespace RhDev.SharePoint.Common.DataAccess.Repository
{
    public interface IApplicationConfigurationRepository : IEntityRepository<ApplicationConfiguration>
    {
        new ApplicationConfiguration GetById(int id);

        ApplicationConfiguration GetByKey(string key);

        void UpdateConfiguration(ApplicationConfiguration applicationConfiguration);

        void CreateConfiguration(ApplicationConfiguration applicationConfiguration);
    }
}

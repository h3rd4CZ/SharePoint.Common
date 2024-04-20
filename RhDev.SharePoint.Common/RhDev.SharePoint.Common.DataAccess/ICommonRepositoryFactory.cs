using RhDev.SharePoint.Common.Caching.Composition;
using RhDev.SharePoint.Common.DataAccess.Repository;
using RhDev.SharePoint.Common.DataAccess.Repository.Entities;

namespace RhDev.SharePoint.Common.DataAccess
{
    public interface ICommonRepositoryFactory : IAutoRegisteredService
    {
        IApplicationConfigurationRepository GetApplicationConfigurationRepository(string webUrl = null);
        IApplicationLogRepository<LogItem> GetApplicationLogRepository(string webUrl = null);
        IDayOffRepository GetDayOffRepository(string webUrl = null);
        T GetRepository<T, TEntity>(string webUrl = null) where TEntity : EntityBase;
        TRepository GetRepository<TRepository>(string webUrl) where TRepository : IRepository;
    }
}

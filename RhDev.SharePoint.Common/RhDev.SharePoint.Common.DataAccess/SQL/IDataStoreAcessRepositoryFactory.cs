using RhDev.SharePoint.Common.Caching.Composition;
namespace RhDev.SharePoint.Common.DataAccess.SQL
{
    public interface IDataStoreAcessRepositoryFactory : IAutoRegisteredService
    {
        IStoreRepository<TEntity> GetStoreRepository<TEntity>() where TEntity : class;
    }
}

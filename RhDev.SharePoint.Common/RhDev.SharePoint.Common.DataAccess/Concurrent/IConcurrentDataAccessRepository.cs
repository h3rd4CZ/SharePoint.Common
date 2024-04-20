using RhDev.SharePoint.Common.Caching.Composition;
using System;

namespace RhDev.SharePoint.Common.DataAccess.Concurrent
{
    public interface IConcurrentDataAccessRepository : IAutoRegisteredService
    {
        void UseRepository<T, TEntity>(Action<T> a, string webUrl = null)
            where T : IEntityRepository<TEntity>, ISynchronizationContextService
            where TEntity : EntityBase;

        void UseService<T>(Action a) where T : ISynchronizationContextService;
    }
}

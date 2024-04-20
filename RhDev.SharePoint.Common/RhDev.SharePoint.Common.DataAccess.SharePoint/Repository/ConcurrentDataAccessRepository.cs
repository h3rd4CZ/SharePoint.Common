using RhDev.SharePoint.Common.Configuration;
using RhDev.SharePoint.Common.DataAccess.Concurrent;
using RhDev.SharePoint.Common.Utils;
using System;
using System.Collections.Generic;

namespace RhDev.SharePoint.Common.DataAccess.SharePoint.Repository
{
    public class ConcurrentDataAccessRepository : IConcurrentDataAccessRepository
    {
        private IDictionary<Type, object> lockStore = new Dictionary<Type, object>() { };
        private readonly FarmConfiguration farmConfiguration;

        public ConcurrentDataAccessRepository(FarmConfiguration farmConfiguration)
        {
            this.farmConfiguration = farmConfiguration;
        }

        public void UseRepository<T, TEntity>(Action<T> a, string webUrl = null)
            where T : IEntityRepository<TEntity>, ISynchronizationContextService
            where TEntity : EntityBase
        {

            Guard.NotNull(a, nameof(a));

            lock (GetLock<T>())
            {
                var repository = (T)Activator.CreateInstance(typeof(T), webUrl ?? GetAppSiteUrl());

                a(repository);
            }
        }

        public void UseService<T>(Action a) where T : ISynchronizationContextService
        {
            lock (GetLock<T>()) a();
        }

        private object GetLock<T>() where T : ISynchronizationContextService
        {
            EnsureLockStoreItem<T>();

            if (!lockStore.TryGetValue(typeof(T), out object o)) throw new InvalidOperationException($"There is no lock for type {typeof(T)} in lock store");

            return o;
        }

        private void EnsureLockStoreItem<T>() where T : ISynchronizationContextService
        {
            lock (this)
            {
                if (!lockStore.ContainsKey(typeof(T))) lockStore.Add(typeof(T), new object());
            }
        }

        private string GetAppSiteUrl()
        {
            return farmConfiguration.AppSiteUrl;
        }
    }
}

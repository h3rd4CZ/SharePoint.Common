using RhDev.SharePoint.Common.Caching.Composition;
using System;
using System.Data.Entity;

namespace RhDev.SharePoint.Common.DataAccess.SQL
{
    public interface IDatabaseAccessRepositoryFactory<TDatabase> : IAutoRegisteredService where TDatabase : DbContext
    {
        T RunReturnAction<T>(Func<TDatabase, T> action, string connectionInfoParameter = default);
        void RunAction(Action<TDatabase> action, string connectionInfoParameter = default);
    }
}

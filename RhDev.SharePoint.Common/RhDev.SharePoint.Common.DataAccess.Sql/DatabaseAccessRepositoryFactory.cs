using Microsoft.SharePoint;
using RhDev.SharePoint.Common.DataAccess.SQL;
using System;
using System.Data.Entity;

namespace RhDev.SharePoint.Common.DataAccess.Sql
{
    public class DatabaseAccessRepositoryFactory<TDatabase> : IDatabaseAccessRepositoryFactory<TDatabase> where TDatabase : DbContext
    {
        private readonly IConnectionInfoFetcher _connectionInfoFetcher;

        public DatabaseAccessRepositoryFactory(IConnectionInfoFetcher connInfoFetcher)
        {
            _connectionInfoFetcher = connInfoFetcher;
        }

        public T RunReturnAction<T>(Func<TDatabase, T> action, string connectionInfoParameter = default)
        {
            if (action == null) throw new ArgumentNullException("action");

            var connString = _connectionInfoFetcher.GetConnectionInfo(connectionInfoParameter);

            T result = default;

            RunApplicationIdentity(() =>
            {
                using (var db = new DatabaseAccessRepository<TDatabase>(connString))
                {
                    result = action(db.Database);
                }
            });

            return result;
        }

        public void RunAction(Action<TDatabase> action, string connectionInfoParameter = default)
        {
            if (action == null) throw new ArgumentNullException("action");

            var connString = _connectionInfoFetcher.GetConnectionInfo(connectionInfoParameter);

            RunApplicationIdentity(() =>
            {
                using (var db = new DatabaseAccessRepository<TDatabase>(connString))
                {
                    action(db.Database);
                }
            });
        }

        private void RunApplicationIdentity(Action a)
        {
            SPSecurity.RunWithElevatedPrivileges(() => { a(); });
        }
    }
}

using RhDev.SharePoint.Common.DataAccess.SQL;
using System;
using System.Data.Entity;

namespace RhDev.SharePoint.Common.DataAccess.Sql
{
    public class DatabaseAccessRepository<TDatabase> : IDatabaseAccessRepository<TDatabase> where TDatabase : DbContext
    {
        private bool Disposed = false;

        private TDatabase _db;
        public TDatabase Database => _db;

        ~DatabaseAccessRepository()
        {
            Dispose(false);
        }

        public DatabaseAccessRepository(string connString)
        {
            if (string.IsNullOrEmpty(connString)) throw new ArgumentNullException("connString");

            _db = (TDatabase)Activator.CreateInstance(typeof(TDatabase), connString);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                if (disposing)
                {
                    if (!Equals(null, _db)) _db.Dispose();
                }
                Disposed = true;
            }
        }

    }
}

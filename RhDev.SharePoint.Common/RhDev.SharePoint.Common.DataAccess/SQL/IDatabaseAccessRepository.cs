using RhDev.SharePoint.Common.Caching.Composition;
using System;
using System.Data.Entity;

namespace RhDev.SharePoint.Common.DataAccess.SQL
{
    public interface IDatabaseAccessRepository<TDatabase> : IService, IDisposable where TDatabase : DbContext
    {
        TDatabase Database { get; }
    }
}

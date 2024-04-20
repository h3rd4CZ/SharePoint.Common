using System.Collections.Generic;
using RhDev.SharePoint.Common.DataAccess.Repository.Entities;

namespace RhDev.SharePoint.Common.DataAccess.Repository
{
    public interface IApplicationLogRepository<T> : IEntityRepository<T> where T : EntityBase
    {
        void WriteLog(T item);
        void RemoveEmptyFolders();
        IList<T> GetLogsOlderThen(int days, CentralClock now);
    }
}

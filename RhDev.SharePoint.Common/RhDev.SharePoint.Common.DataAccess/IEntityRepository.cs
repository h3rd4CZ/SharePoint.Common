using System.Collections.Generic;
using RhDev.SharePoint.Common.Caching.Composition;

namespace RhDev.SharePoint.Common.DataAccess
{
    public interface IEntityRepository<TEntity> : IRepository
        where TEntity : EntityBase
    {
        TEntity GetById(int id);

        int Create(TEntity entity);

        int CreateFile(TEntity entity, string fileName, bool requiresElevation);
        int UpdateFile(TEntity entity, bool requiresElevation);

        int CreateElevated(TEntity entity);

        //void Delete(int entityId);

        void Update(TEntity entity);

        void Update(TEntity entity, bool createVersion);

        void UpdateElevated(TEntity entity);

        void UpdateElevated(TEntity entity, bool createVersion);

        IList<TEntity> GetAllEntities();

        /// <summary>
        /// Get dispForm item url (server relative)
        /// </summary>
        /// <param name="entityId">Items ID</param>
        /// <returns></returns>
        LocationInfo GetLocation(int entityId);

        EntityLink GetRelativeLink(int itemId);
        EntityLink GetAbsoluteLink(string appUrl, int itemId);
    }
}

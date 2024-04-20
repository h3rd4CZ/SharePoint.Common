using RhDev.SharePoint.Common.Caching.Composition;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace RhDev.SharePoint.Common.DataAccess
{
    public interface IStoreRepository<TStoreEntity> : IService
        where TStoreEntity : class

    {
        /// <summary>
        /// Read by id with lazy loading of related entities
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        TStoreEntity ReadById(int id, IList<Func<TStoreEntity, object>> include = null);
        
        /// <summary>
        /// Read by expression include related entities in one query if include is not null
        /// </summary>
        /// <param name="lambda"></param>
        /// <returns></returns>
        IList<TStoreEntity> Read(Expression<Func<TStoreEntity, bool>> lambda, IList<Expression<Func<TStoreEntity, object>>> include = null, bool? checkSingle = null);

        /// <summary>
        /// Read by expression include related entities in one query if include is not null
        /// </summary>
        /// <returns></returns>
        IList<TStoreEntity> ReadAll(IList<Expression<Func<TStoreEntity, object>>> include = null);

        void Create(TStoreEntity entity);
        void Create(IList<TStoreEntity> entities);
        void Update(TStoreEntity entity);
        void Delete(int id);

        void ExecuteCommand(string command);
    }
}

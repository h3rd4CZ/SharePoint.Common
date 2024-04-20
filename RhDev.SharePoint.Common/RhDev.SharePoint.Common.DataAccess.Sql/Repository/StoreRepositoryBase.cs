using RhDev.SharePoint.Common.DataAccess.SQL;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace RhDev.SharePoint.Common.DataAccess.Sql.Repository
{
    public abstract class StoreRepositoryBase<TStoreEntity, TDatabase> : DatabaseRepositoryBase<TStoreEntity, TDatabase>,
        IStoreRepository<TStoreEntity> where TStoreEntity : class where TDatabase : DbContext
    {
        protected StoreRepositoryBase(IDatabaseAccessRepositoryFactory<TDatabase> databaseAccessRepositoryFactory) : base(databaseAccessRepositoryFactory)
        {

        }

        public TStoreEntity ReadById(int id, IList<Func<TStoreEntity, object>> include = null)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));

            return UseStoreRepositoryAndReturn((db, set) =>
            {
                var entity = set.Find(id);

                if (Equals(null, entity)) throw new EntityNotFoundException(id.ToString());

                if (!Equals(null, include))

                    foreach (var incl in include)
                        incl(entity);

                return entity;

            }, false);
        }

        public IList<TStoreEntity> Read(
            Expression<Func<TStoreEntity, bool>> lambda,
            IList<Expression<Func<TStoreEntity, object>>> include = null,
            bool? checkSingle = null)
        {
            if (lambda == null) throw new ArgumentNullException(nameof(lambda));

            return UseStoreRepositoryAndReturn((db, set) =>
            {
                var q = BuildQueryable(set, include);

                var entities = q.Where(lambda).ToList();

                if (Equals(null, checkSingle) || !checkSingle.Value) return entities;

                if (entities.Count > 1) throw new InvalidOperationException("Multiple entities found");

                return entities;

            }, false);
        }

        public IList<TStoreEntity> ReadAll(IList<Expression<Func<TStoreEntity, object>>> include = null)
        {
            return UseStoreRepositoryAndReturn((db, set) =>
            {
                var q = BuildQueryable(set, include);

                var entities = q.ToList();

                return entities;

            }, false);
        }

        public void Create(TStoreEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            UseStoreRepository((db, set) =>
            {
                set.Add(entity);
            }, true);
        }

        public void Create(IList<TStoreEntity> entities)
        {
            if (entities == null) throw new ArgumentNullException(nameof(entities));
            if (entities.Count == 0)
                throw new ArgumentException("Value cannot be an empty collection.", nameof(entities));

            UseStoreRepository((db, set) =>
            {
                set.AddRange(entities);
            }, true);
        }

        public void Update(TStoreEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            UseStoreRepository((db, set) =>
            {
                db.Entry(entity).State = EntityState.Modified;
            }, true);
        }

        public void Delete(int id)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));

            UseStoreRepository((db, set) =>
            {
                var entity = set.Find(id);

                if (Equals(null, entity)) throw new EntityNotFoundException(id.ToString());

                set.Remove(entity);

            }, true);
        }

        public void ExecuteCommand(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(command));

            UseStoreRepository((db, set) =>
            {
                db.Database.ExecuteSqlCommand(command);
            }, false);
        }
    }
}

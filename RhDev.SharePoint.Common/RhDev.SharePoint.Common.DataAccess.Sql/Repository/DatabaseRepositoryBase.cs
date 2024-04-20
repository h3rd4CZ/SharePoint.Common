using RhDev.SharePoint.Common.DataAccess.SQL;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace RhDev.SharePoint.Common.DataAccess.Sql.Repository
{
    public abstract class DatabaseRepositoryBase<TStoreEntity, TDatabase> where TStoreEntity : class where TDatabase : DbContext
    {
        private const string COMMON_ENTITY_IDENTIFIER = "Id";

        public IDatabaseAccessRepositoryFactory<TDatabase> databaseAccessRepositoryFactory { get; set; }
        protected DatabaseRepositoryBase(IDatabaseAccessRepositoryFactory<TDatabase> databaseAccessRepositoryFactory)
        {
            this.databaseAccessRepositoryFactory = databaseAccessRepositoryFactory;
        }

        protected void UseStoreRepository(
            Action<TDatabase, DbSet<TStoreEntity>> a
            , bool? save = null,
            TDatabase database = null)
        {
            if (!Equals(null, database))
                UseDatabase(a, database, save);

            else
            {
                databaseAccessRepositoryFactory.RunAction(db =>
                {
                    UseDatabase(a, db, save);
                });
            }
        }

        protected void UseDatabase(
            Action<TDatabase, DbSet<TStoreEntity>> a,
            TDatabase database,
            bool? save = null)
        {
            if (database == null) throw new ArgumentNullException(nameof(database));

            var set = database.Set<TStoreEntity>();

            a(database, set);

            if (!Equals(null, save) && save.Value) database.SaveChanges();

        }

        protected TReturn UseDatabaseAndReturn<TReturn>(
            Func<TDatabase, DbSet<TStoreEntity>, TReturn> a,
            TDatabase database,
            bool? save = null)
        {
            if (database == null) throw new ArgumentNullException(nameof(database));

            TReturn ret = default;

            var set = database.Set<TStoreEntity>();

            ret = a(database, set);

            if (!Equals(null, save) && save.Value) database.SaveChanges();

            return ret;

        }

        protected TReturn UseStoreRepositoryAndReturn<TReturn>(
            Func<TDatabase, DbSet<TStoreEntity>, TReturn> a,
            bool? save = null,
            TDatabase database = null)
        {
            TReturn ret = default(TReturn);

            if (!Equals(null, database))
                ret = UseDatabaseAndReturn(a, database, save);
            else
            {
                databaseAccessRepositoryFactory.RunAction(db =>
                {
                    ret = UseDatabaseAndReturn(a, db, save);
                });
            }

            return ret;
        }

        protected void LoadRelatedIfAny<TCollection>(
            TDatabase db,
            TStoreEntity entity,
            Expression<Func<TStoreEntity, ICollection<TCollection>>> relatedCollection,
            Action<IQueryable<TCollection>> relatedExpression) where TCollection : class
        {
            if (Equals(null, entity)) return;

            if (Equals(null, relatedCollection)) return;

            if (Equals(null, relatedExpression))
                throw new InvalidOperationException(
                    "If Related collection is not null related expression must be defined");

            var query = db.Entry(entity)
                .Collection(relatedCollection)
                .Query();

            relatedExpression(query);
        }

        protected void LoadNavigationIfAny<TNavigation>(
            TDatabase db,
            TStoreEntity entity,
            Expression<Func<TStoreEntity, TNavigation>> navigationResolver) where TNavigation : class
        {
            if (Equals(null, entity)) return;

            if (Equals(null, navigationResolver)) return;

            db.Entry(entity)
                .Reference(navigationResolver)
                .Load();
        }

        protected void TurnLazyLoadingOff(TDatabase db)
        {
            if (db == null) throw new ArgumentNullException(nameof(db));

            db.Configuration.LazyLoadingEnabled = false;
        }

        protected IQueryable<TStoreEntity> BuildQueryable(DbSet<TStoreEntity> set, IList<Expression<Func<TStoreEntity, object>>> include)
        {
            IQueryable<TStoreEntity> q = !Equals(null, include) && include.Any() ? set.Include(include.First()) : default(IQueryable<TStoreEntity>);

            if (!Equals(null, include))
            {
                for (var i = 1; i < include.Count; i++)
                    q = q.Include(include[i]);
            }
            else
            {
                q = set;
            }

            return q;
        }

        protected Func<TStoreEntity, bool> BuildGetByIdLambda(int id)
        {
            var param = Expression.Parameter(typeof(TStoreEntity));
            var property = Expression.Property(param, typeof(TStoreEntity), COMMON_ENTITY_IDENTIFIER);

            var exprId = Expression.Constant(id);

            var getByIdCall = Expression.Equal(property, exprId);

            return Expression.Lambda<Func<TStoreEntity, bool>>(getByIdCall, param).Compile();

        }
    }
}

using RhDev.SharePoint.Common.DataAccess.SQL;
using RhDev.SharePoint.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhDev.SharePoint.Common.DataAccess.Sql
{
    public class DataStoreAcessRepositoryFactory : IDataStoreAcessRepositoryFactory
    {
        private readonly IAutoRegisterStoreRepository[] autoRegisterStoreRepositories;

        public DataStoreAcessRepositoryFactory(
            IAutoRegisterStoreRepository[] autoRegisterStoreRepositories)
        {
            this.autoRegisterStoreRepositories = autoRegisterStoreRepositories;
        }
        public IStoreRepository<TEntity> GetStoreRepository<TEntity>() where TEntity : class
        {
            var entityType = typeof(TEntity);

            var repository = autoRegisterStoreRepositories.FirstOrDefault(
                r =>
                    r.GetType().GetInterfaces().Any(i => i == typeof(IStoreRepository<TEntity>))
                );

            if(Equals(null, repository))
            throw new Exception($"Store repository for type : {entityType} was not found. Please make sure that the repository implements IAutoRegisterStoreRepository interface");

            return (IStoreRepository<TEntity>)repository;
        }
    }
}

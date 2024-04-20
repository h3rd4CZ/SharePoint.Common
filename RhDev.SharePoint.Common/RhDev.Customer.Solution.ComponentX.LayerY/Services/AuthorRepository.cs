using RhDev.Customer.Solution.ComponentX.LayerY.SQL;
using RhDev.SharePoint.Common.DataAccess;
using RhDev.SharePoint.Common.DataAccess.Sql.Repository;
using RhDev.SharePoint.Common.DataAccess.SQL;

namespace RhDev.Customer.Solution.ComponentX.LayerY.Services
{
    public class AuthorRepository : StoreRepositoryBase<SQL.Author, TestEntitiesDatabase>,
        IStoreRepository<Author>,
        IAutoRegisterStoreRepository
    {
        public AuthorRepository(
            IDatabaseAccessRepositoryFactory<TestEntitiesDatabase> databaseAccessRepositoryFactory) 
            : base(databaseAccessRepositoryFactory)
        {
        }
    }
}

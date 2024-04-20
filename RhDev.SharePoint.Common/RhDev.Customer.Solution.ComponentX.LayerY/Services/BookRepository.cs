using RhDev.Customer.Solution.ComponentX.LayerY.SQL;
using RhDev.SharePoint.Common.DataAccess;
using RhDev.SharePoint.Common.DataAccess.Sql.Repository;
using RhDev.SharePoint.Common.DataAccess.SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhDev.Customer.Solution.ComponentX.LayerY.Services
{
    public class BookRepository : StoreRepositoryBase<Book, TestEntitiesDatabase>,
        IStoreRepository<Book>
    {
        public BookRepository(IDatabaseAccessRepositoryFactory<TestEntitiesDatabase> databaseAccessRepositoryFactory) : base(databaseAccessRepositoryFactory)
        {
        }
    }
}

using RhDev.Customer.Solution.ComponentX.LayerY.SQL;
using RhDev.SharePoint.Common.Caching.Composition;
using RhDev.SharePoint.Common.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhDev.Customer.Solution.ComponentX.LayerY.Services
{
    public interface IStoreRepositoryFactory : IAutoRegisteredService
    {
        IStoreRepository<Author> AuthorRepository { get; }
        IStoreRepository<Book> BookRepository { get; }

    }
}

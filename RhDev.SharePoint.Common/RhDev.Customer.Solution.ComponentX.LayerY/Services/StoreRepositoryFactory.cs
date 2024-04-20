using RhDev.Customer.Solution.ComponentX.LayerY.SQL;
using RhDev.SharePoint.Common.DataAccess;

namespace RhDev.Customer.Solution.ComponentX.LayerY.Services
{
    public class StoreRepositoryFactory : IStoreRepositoryFactory
    {
        private readonly AuthorRepository authorRepository;
        private readonly BookRepository bookRepository;

        public StoreRepositoryFactory(
            AuthorRepository authorRepository,
            BookRepository bookRepository)
        {
            this.authorRepository = authorRepository;
            this.bookRepository = bookRepository;
        }
        public IStoreRepository<Author> AuthorRepository => this.authorRepository;

        public IStoreRepository<Book> BookRepository => this.bookRepository;
    }
}

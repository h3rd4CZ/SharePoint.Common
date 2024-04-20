using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute.ExceptionExtensions;
using RhDev.Customer.Solution.Common.DataAccess.ActiveDirectory.Services;
using RhDev.Customer.Solution.ComponentX.LayerY.Services;
using RhDev.Customer.Solution.ComponentX.LayerY.SQL;
using RhDev.SharePoint.Common.Composition.Factory;
using RhDev.SharePoint.Common.Composition.Factory.Definitions;
using RhDev.SharePoint.Common.Configuration;
using RhDev.SharePoint.Common.DataAccess.SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace RhDev.SharePoint.Common.SystemIntegrationTest.SQL
{
    [TestClass]
    public class SqlAccessTest
    {
        ContainerRegistrationDefinition GetClientSolutionContainerRegistration() =>
            RhDev.Customer.Solution.Common.DataAccess.SharePoint.Const.GetClientSolutionContainerRegistration();

        private void SetupConfiguration(IApplicationContainerSetup container)
        {
            var farmConfig = container.Frontend.GetInstance<FarmConfiguration>();
            farmConfig.AppSiteUrl = Const.SYSTEMINTEGRATIONTEST_URL;

            var globalConfig = container.Frontend.GetInstance<GlobalConfiguration>();
            globalConfig.ConnectionString =
                $"metadata=res://*/SQL.EntitiesModel.csdl|res://*/SQL.EntitiesModel.ssdl|res://*/SQL.EntitiesModel.msl;provider=System.Data.SqlClient;provider connection string=\"data source=(LocalDB)\\MSSQLLocalDB; attachdbfilename=c:\\DEVOPS\\RhDev\\Common\\RhDev.SharePoint.Common\\RhDev.Customer.Solution.ComponentX.LayerY\\SQL\\TestEntities.mdf; integrated security = True; connect timeout = 30; MultipleActiveResultSets=True; App=EntityFramework\"";


         
        }

        [TestMethod]
        public void TestSqlDatabaseConnection()
        {
            var containerDefinition = GetClientSolutionContainerRegistration();

            var container = ApplicationContainerFactory.Create(containerDefinition);

            SetupConfiguration(container);
            
            var sqlStore = container.Frontend.GetInstance<IStoreRepositoryFactory>();

            sqlStore.Should().NotBeNull();

            var authorRepository = sqlStore.AuthorRepository;

            var allAuthors = authorRepository.ReadAll();

            allAuthors.Should().NotBeNull();
        }

        [TestMethod]
        public void RetrieveStoreRepositoryUsingAutoregisteredStore()
        {
            var containerDefinition = GetClientSolutionContainerRegistration();
            var container = ApplicationContainerFactory.Create(containerDefinition);

            var factory = container.Frontend.GetInstance<IDataStoreAcessRepositoryFactory>();

            var authorRepo = factory.GetStoreRepository<Author>();
            authorRepo.Should().NotBeNull();

            Assert.ThrowsException<Exception>(() => factory.GetStoreRepository<Book>());
        }

        [TestMethod]
        public void ReadWriteDataTest()
        {
            var containerDefinition = GetClientSolutionContainerRegistration();
            var container = ApplicationContainerFactory.Create(containerDefinition);
            SetupConfiguration(container);

            var sqlStore = container.Frontend.GetInstance<IStoreRepositoryFactory>();
            var authorRepository = sqlStore.AuthorRepository;
            var bookRepository = sqlStore.BookRepository;

            var newAuthor = new Author()
            {
                Age = 30,
                Name = "Hans"
            };

            authorRepository.Create(newAuthor);

            var hans = authorRepository.Read(a => a.Name == "Hans");
            var firstHans = hans.FirstOrDefault();

            bookRepository.Create(new List<Book>
            {
                new Book{ Author1 = firstHans, Title = "Book1" },
                new Book{ Author1 = firstHans, Title = "Book2" }
            });

            hans = authorRepository.Read(a => a.Name == "Hans", include: new List<Expression<Func<Author, object>>> { a => a.Books });
            firstHans = hans.LastOrDefault();

            var allHansBooks = firstHans.Books;

            allHansBooks.Should().NotBeNull();
            allHansBooks.Should().HaveCount(2);

            var book1 = bookRepository.ReadAll(include: new List<Expression<Func<Book, object>>> { b => b.Author1 });

            book1.Should().NotBeNull();
            book1.Should().HaveCountGreaterOrEqualTo(1);

            book1.FirstOrDefault().Author1.Name.Should().Be("Hans");
        }
    }
}

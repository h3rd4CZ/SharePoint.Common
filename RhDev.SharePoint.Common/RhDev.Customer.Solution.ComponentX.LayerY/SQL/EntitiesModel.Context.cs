namespace RhDev.Customer.Solution.ComponentX.LayerY.SQL
{
    using RhDev.Customer.Solution.ComponentX.LayerY.SQL;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;

    public partial class TestEntitiesDatabase : DbContext
    {
        public TestEntitiesDatabase(string connString)
            : base(connString)
        {
        }
        public TestEntitiesDatabase()
            : base("name=TestEntitiesDatabase")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }

        public virtual DbSet<Author> Authors { get; set; }
        public virtual DbSet<Book> Books { get; set; }
    }
}
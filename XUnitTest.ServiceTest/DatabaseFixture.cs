using Autofac;
using System;
using XUnitTest.Data;

namespace XUnitTest.ServiceTest
{
    public class DatabaseFixture : IDisposable
    {
        private UserDbContext dbContext;
        public DatabaseFixture()
        {

            IContainer container = DependencyInjection.DICollections();
            dbContext = container.Resolve<UserDbContext>();
            //dbContext.Database.EnsureDeleted();
            //dbContext.Database.EnsureCreated();
            //var sqlFiles = Directory.GetFiles(@"Data", "*.sql");
            //foreach (var file in sqlFiles)
            //{
            //    dbContext.Database.ExecuteSqlCommand(File.ReadAllText(file));
            //}

        }

        public void Dispose()
        {

            dbContext.Database.EnsureDeleted();

        }
    }
}

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MigrationDummyConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }
    public class BloggingContextFactory : IDesignTimeDbContextFactory<BastelKatalog.Data.CatalogueContext>
    {
        public BastelKatalog.Data.CatalogueContext CreateDbContext(string[] args)
        {
            //DbContextOptionsBuilder<BastelKatalog.Data.CatalogueContext> optionsBuilder = new DbContextOptionsBuilder<BastelKatalog.Data.CatalogueContext>();
            //optionsBuilder.UseSqlite("Filename=blog.db");

            //return new BastelKatalog.Data.CatalogueContext(optionsBuilder.Options);
            return new BastelKatalog.Data.CatalogueContext();
        }
    }
}

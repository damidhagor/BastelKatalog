using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Xamarin.Essentials;

namespace BastelKatalog.Data
{
    public class CatalogueContext : DbContext
    {
        public DbSet<Item> Items { get; set; } = default!;

        public DbSet<Category> Categories { get; set; } = default!;


        public CatalogueContext()
        {
            SQLitePCL.Batteries_V2.Init();
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // On Android choose AppData directory
            string dbPath;
            if (DeviceInfo.Platform == DevicePlatform.Android)
                dbPath = Path.Combine(FileSystem.AppDataDirectory, "catalogue.db3");
            else
                dbPath = "catalogue.db3";

            optionsBuilder.UseSqlite($"Filename={dbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // AutoIncrement Item Id
            modelBuilder.Entity<Item>().HasKey(i => i.Id);
            modelBuilder.Entity<Item>().Property(i => i.Id).ValueGeneratedOnAdd();

            // AutoIncrement Category Id
            modelBuilder.Entity<Category>().HasKey(c => c.Id);
            modelBuilder.Entity<Category>().Property(c => c.Id).ValueGeneratedOnAdd();
            // One-to-many parent Category relationship
            // A Category can have 1 parent Category.
            modelBuilder.Entity<Category>()
                .HasOne(c => c.ParentCategory)
                .WithMany(c => c!.SubCategories)
                .HasForeignKey(c => c.ParentCategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

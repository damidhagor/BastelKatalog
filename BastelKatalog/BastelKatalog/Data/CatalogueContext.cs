using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Xamarin.Essentials;

namespace BastelKatalog.Data
{
    // Migrations Command: Add-Migration -Name "Changed Stock" -OutputDir "Data\Migrations" -Project BastelKatalog -StartupProject MigrationDummyConsole
    public class CatalogueContext : DbContext
    {
        public DbSet<Item> Items { get; set; } = default!;

        public DbSet<Category> Categories { get; set; } = default!;

        public DbSet<Project> Projects { get; set; } = default!;

        public DbSet<ProjectItem> ProjectItems { get; set; } = default!;


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

            optionsBuilder
                .UseSqlite($"Filename={dbPath}")
                .UseLazyLoadingProxies();
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

            // AutoIncrement Project Id
            modelBuilder.Entity<Project>().HasKey(p => p.Id);
            modelBuilder.Entity<Project>().Property(p => p.Id).ValueGeneratedOnAdd();
            // Many-to-One Project ProjectItem Relationship
            // Projects have many ProjectItems associated with them
            // An Item can be associated to one Project
            modelBuilder.Entity<Project>()
                .HasMany(p => p.Items)
                .WithOne(i => i.Project)
                .HasForeignKey(i => i.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            // AutoIncrement ProjectItem Id
            modelBuilder.Entity<ProjectItem>().HasKey(i => i.Id);
            modelBuilder.Entity<ProjectItem>().Property(i => i.Id).ValueGeneratedOnAdd();
            // One-to-Man< ProjectItem Item Relationship
            // ProjectItems have one Item associated with them
            // An Item can be associated to many ProjectItems
            modelBuilder.Entity<ProjectItem>()
                .HasOne(i => i.Item)
                .WithMany()
                .HasForeignKey(i => i.ItemId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using BastelKatalog.Data;
using Microsoft.EntityFrameworkCore;

namespace BastelKatalog.Backup
{
    public class BackupProvider : IBackupProvider
    {
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly CatalogueContext _catalogueContext;
        private readonly IBackupPathProvider _backupPathProvider;

        public BackupProvider(CatalogueContext catalogueContext, IBackupPathProvider backupPathProvider)
        {
            _jsonSerializerOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            _catalogueContext = catalogueContext;
            _backupPathProvider = backupPathProvider;
        }


        public async Task<string> CreateBackupArchiveAsync(CancellationToken cancellationToken)
        {
            var backupName = $"BastelKatalog_Backup_{DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-ffff")}";
            var backupFolder = CreateBackupFolder(backupName);
            var backupZipFilename = Path.Combine(_backupPathProvider.GetBackupPath(), $"{backupName}.zip");

            await CreateDbBackupAsync(backupFolder, cancellationToken);

            await ExportImagesAsync(backupFolder, cancellationToken);

            await CreateZipFileAsync(backupFolder, backupZipFilename, cancellationToken);

            return backupZipFilename;
        }


        private string CreateBackupFolder(string backupName)
        {
            var backupFolder = Path.Combine(_backupPathProvider.GetBackupPath(), backupName);

            Directory.CreateDirectory(backupFolder);

            return backupFolder;
        }

        private async Task CreateDbBackupAsync(string backupFolder, CancellationToken cancellationToken)
        {
            var categoriesFilename = Path.Combine(backupFolder, "categories.json");
            var itemsFilename = Path.Combine(backupFolder, "items.json");
            var projectsFilename = Path.Combine(backupFolder, "projects.json");
            var projectItemsFilename = Path.Combine(backupFolder, "projectitems.json");

            var categories = await _catalogueContext.Categories.Select(c => new Models.Category(c)).ToListAsync(cancellationToken);
            var items = await _catalogueContext.Items.Select(i => new Models.Item(i)).ToListAsync(cancellationToken);
            var projects = await _catalogueContext.Projects.Select(p => new Models.Project(p)).ToListAsync(cancellationToken);
            var projectItems = await _catalogueContext.ProjectItems.Select(i => new Models.ProjectItem(i)).ToListAsync(cancellationToken);

            using (var stream = File.Create(categoriesFilename))
                await JsonSerializer.SerializeAsync(stream, categories, _jsonSerializerOptions, cancellationToken);
            using (var stream = File.Create(itemsFilename))
                await JsonSerializer.SerializeAsync(stream, items, _jsonSerializerOptions, cancellationToken);
            using (var stream = File.Create(projectsFilename))
                await JsonSerializer.SerializeAsync(stream, projects, _jsonSerializerOptions, cancellationToken);
            using (var stream = File.Create(projectItemsFilename))
                await JsonSerializer.SerializeAsync(stream, projectItems, _jsonSerializerOptions, cancellationToken);
        }

        private async Task ExportImagesAsync(string backupFolder, CancellationToken cancellationToken)
        {
            var imageFolder = Path.Combine(backupFolder, "images");
            Directory.CreateDirectory(imageFolder);

            var imagePaths = await _catalogueContext.Items
                .Where(i => i.ImagePath != null)
                .Select(i => (string)i.ImagePath!)
                .ToListAsync(cancellationToken);

            var imageNames = imagePaths.SelectMany(p => p.Split(":", StringSplitOptions.RemoveEmptyEntries)).ToList();

            foreach (var imageName in imageNames)
            {
                var imageFilename = Path.Combine(imageFolder, imageName);
                var image = await ImageManager.GetImageAsync(imageName, cancellationToken);
                await File.WriteAllBytesAsync(imageFilename, image, cancellationToken);
            }
        }

        private async Task CreateZipFileAsync(string backupFolder, string zipFilename, CancellationToken cancellationToken)
        {
            await Task.Run(() => ZipFile.CreateFromDirectory(backupFolder, zipFilename, CompressionLevel.Optimal, true));
        }
    }
}

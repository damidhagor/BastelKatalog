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
        private readonly IFilePathProvider _backupPathProvider;
        private Action<string, float>? _progressCallback;

        public BackupProvider(CatalogueContext catalogueContext, IFilePathProvider backupPathProvider)
        {
            _jsonSerializerOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            _catalogueContext = catalogueContext;
            _backupPathProvider = backupPathProvider;
        }


        public async Task<string> CreateBackupArchiveAsync(Action<string, float>? progressCallback, CancellationToken cancellationToken)
        {
            _progressCallback = progressCallback;

            _progressCallback?.Invoke("Starte Backup ...", 0.0f);

            var backupName = $"BastelKatalog_Backup_{DateTime.Now:yyyy-MM-dd-HH-mm-ss-ffff}";
            var backupFolder = CreateBackupFolder(backupName);
            var backupZipFilename = Path.Combine(_backupPathProvider.GetCacheDirectory(), $"{backupName}.zip");

            await CreateDbBackupAsync(backupFolder, cancellationToken);

            await ExportImagesAsync(backupFolder, cancellationToken);

            await CreateZipFileAsync(backupFolder, backupZipFilename, cancellationToken);

            _progressCallback?.Invoke("Backup abgeschlossen.", 1.0f);

            return backupZipFilename;
        }


        private string CreateBackupFolder(string backupName)
        {
            var backupFolder = Path.Combine(_backupPathProvider.GetCacheDirectory(), backupName);

            Directory.CreateDirectory(backupFolder);

            return backupFolder;
        }

        private async Task CreateDbBackupAsync(string backupFolder, CancellationToken cancellationToken)
        {
            var categoriesFilename = Path.Combine(backupFolder, "categories.json");
            var itemsFilename = Path.Combine(backupFolder, "items.json");
            var projectsFilename = Path.Combine(backupFolder, "projects.json");
            var projectItemsFilename = Path.Combine(backupFolder, "projectitems.json");

            _progressCallback?.Invoke("Exportiere Kategorien ... (1/6)", 0.0f);
            var categories = await _catalogueContext.Categories.Select(c => new Models.Category(c)).ToListAsync(cancellationToken);
            using (var stream = File.Create(categoriesFilename))
                await JsonSerializer.SerializeAsync(stream, categories, _jsonSerializerOptions, cancellationToken);

            _progressCallback?.Invoke("Exportiere Items ... (2/6)", 0.15f);
            var items = await _catalogueContext.Items.Select(i => new Models.Item(i)).ToListAsync(cancellationToken);
            using (var stream = File.Create(itemsFilename))
                await JsonSerializer.SerializeAsync(stream, items, _jsonSerializerOptions, cancellationToken);

            _progressCallback?.Invoke("Exportiere Projekte ... (3/6)", 0.3f);
            var projects = await _catalogueContext.Projects.Select(p => new Models.Project(p)).ToListAsync(cancellationToken);
            using (var stream = File.Create(projectsFilename))
                await JsonSerializer.SerializeAsync(stream, projects, _jsonSerializerOptions, cancellationToken);

            _progressCallback?.Invoke("Exportiere Projektitems ... (4/6)", 0.45f);
            var projectItems = await _catalogueContext.ProjectItems.Select(i => new Models.ProjectItem(i)).ToListAsync(cancellationToken);
            using (var stream = File.Create(projectItemsFilename))
                await JsonSerializer.SerializeAsync(stream, projectItems, _jsonSerializerOptions, cancellationToken);
        }

        private async Task ExportImagesAsync(string backupFolder, CancellationToken cancellationToken)
        {
            _progressCallback?.Invoke("Exportiere Bilder ... (5/6)", 0.6f);
            await ImageManager.ExportImages(backupFolder, cancellationToken);
        }

        private async Task CreateZipFileAsync(string backupFolder, string zipFilename, CancellationToken cancellationToken)
        {
            _progressCallback?.Invoke("Erstelle Zip-Archiv ... (6/6)", 0.75f);
            await Task.Run(() => ZipFile.CreateFromDirectory(backupFolder, zipFilename, CompressionLevel.Optimal, false), cancellationToken);
        }
    }
}

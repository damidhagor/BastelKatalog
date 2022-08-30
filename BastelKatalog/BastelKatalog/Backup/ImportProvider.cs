using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using BastelKatalog.Data;

namespace BastelKatalog.Backup
{
    public class ImportProvider : IImportProvider
    {
        private readonly CatalogueContext _catalogueContext;
        private readonly IFilePathProvider _backupPathProvider;
        private Action<string, float>? _progressCallback;

        public ImportProvider(CatalogueContext catalogueContext, IFilePathProvider backupPathProvider)
        {
            _catalogueContext = catalogueContext;
            _backupPathProvider = backupPathProvider;
        }

        public async Task ImportArchiveAsync(string archiveFilename, Action<string, float>? progressCallback, CancellationToken cancellationToken)
        {
            _progressCallback = progressCallback;

            _progressCallback?.Invoke("Starte Import ...", 0.0f);

            string cacheDirectory = _backupPathProvider.GetCacheDirectory();

            _progressCallback?.Invoke("Entpacke Archiv ...", 0.2f);
            var unpackedDirectory = await UnpackZipFileAsync(archiveFilename, cacheDirectory, cancellationToken);

            var files = Directory.EnumerateFiles(cacheDirectory, "*", SearchOption.AllDirectories).ToList();

            _progressCallback?.Invoke("Lösche existierende Daten ...", 0.4f);
            await ClearExistingData(cancellationToken);

            _progressCallback?.Invoke("Importiere Bilder ...", 0.6f);
            await ImportImages(unpackedDirectory, cancellationToken);

            _progressCallback?.Invoke("Importiere Daten ...", 0.8f);
            await ImportData(unpackedDirectory, cancellationToken);

            _progressCallback?.Invoke("Import abgeschlossen.", 1.0f);
        }

        private async Task<string> UnpackZipFileAsync(string archiveFilename, string destinationDirectory, CancellationToken cancellationToken)
        {
            var unpackedDirectory = Path.Combine(destinationDirectory, Path.GetFileNameWithoutExtension(archiveFilename));
            await Task.Run(() => ZipFile.ExtractToDirectory(archiveFilename, unpackedDirectory, true), cancellationToken);
            return unpackedDirectory;
        }

        private async Task ClearExistingData(CancellationToken cancellationToken)
        {
            _catalogueContext.ProjectItems.RemoveRange(_catalogueContext.ProjectItems);
            _catalogueContext.Projects.RemoveRange(_catalogueContext.Projects);
            _catalogueContext.Items.RemoveRange(_catalogueContext.Items);
            _catalogueContext.Categories.RemoveRange(_catalogueContext.Categories);

            await _catalogueContext.SaveChangesAsync(cancellationToken);
        }

        private async Task ImportImages(string unpackedDirectory, CancellationToken cancellationToken)
        {
            var imageDirectory = Path.Combine(unpackedDirectory, "images");
            await ImageManager.ImportImages(imageDirectory, cancellationToken);
        }

        private async Task ImportData(string unpackedDirectory, CancellationToken cancellationToken)
        {
            var categoriesFilename = Path.Combine(unpackedDirectory, "categories.json");
            var itemsFilename = Path.Combine(unpackedDirectory, "items.json");
            var projectsFilename = Path.Combine(unpackedDirectory, "projects.json");
            var projectItemsFilename = Path.Combine(unpackedDirectory, "projectitems.json");

            List<Models.Category> categories;
            List<Models.Item> items;
            List<Models.Project> projects;
            List<Models.ProjectItem> projectItems;

            using (var stream = File.OpenRead(categoriesFilename))
                categories = await JsonSerializer.DeserializeAsync<List<Models.Category>>(stream, null, cancellationToken) ?? new List<Models.Category>();
            using (var stream = File.OpenRead(itemsFilename))
                items = await JsonSerializer.DeserializeAsync<List<Models.Item>>(stream, null, cancellationToken) ?? new List<Models.Item>();
            using (var stream = File.OpenRead(projectsFilename))
                projects = await JsonSerializer.DeserializeAsync<List<Models.Project>>(stream, null, cancellationToken) ?? new List<Models.Project>();
            using (var stream = File.OpenRead(projectItemsFilename))
                projectItems = await JsonSerializer.DeserializeAsync<List<Models.ProjectItem>>(stream, null, cancellationToken) ?? new List<Models.ProjectItem>();

            _catalogueContext.Categories.AddRange(categories.Select(c => c.ToDataModel()));
            _catalogueContext.Items.AddRange(items.Select(i => i.ToDataModel()));
            _catalogueContext.Projects.AddRange(projects.Select(p => p.ToDataModel()));
            _catalogueContext.ProjectItems.AddRange(projectItems.Select(pi => pi.ToDataModel()));

            await _catalogueContext.SaveChangesAsync(cancellationToken);
        }
    }
}
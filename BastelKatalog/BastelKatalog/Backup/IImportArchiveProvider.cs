using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace BastelKatalog.Backup
{
    public interface IImportArchiveProvider
    {
        Task<string> GetArchiveFilename();
    }

    public class ImportArchiveProvider : IImportArchiveProvider
    {
        public async Task<string> GetArchiveFilename()
        {
            var options = new PickOptions
            {
                PickerTitle = "Please select a backup archive"
            };

            var result = await FilePicker.PickAsync(options);

            return result == null || !result.FileName.EndsWith(".zip", StringComparison.OrdinalIgnoreCase)
                ? throw new InvalidOperationException("Es wurde kein gültiges Backup Archiv ausgewählt.")
                : result.FullPath;
        }
    }
}

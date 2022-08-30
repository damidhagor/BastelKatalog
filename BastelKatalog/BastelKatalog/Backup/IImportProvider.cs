using System;
using System.Threading;
using System.Threading.Tasks;

namespace BastelKatalog.Backup
{
    public interface IImportProvider
    {
        Task ImportArchiveAsync(string archiveFilename, Action<string, float>? progressCallback, CancellationToken cancellationToken);
    }
}
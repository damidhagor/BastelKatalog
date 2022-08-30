using System;
using System.Threading;
using System.Threading.Tasks;

namespace BastelKatalog.Backup
{
    public interface IBackupProvider
    {
        Task<string> CreateBackupArchiveAsync(Action<string, float>? progressCallback, CancellationToken cancellationToken);
    }
}
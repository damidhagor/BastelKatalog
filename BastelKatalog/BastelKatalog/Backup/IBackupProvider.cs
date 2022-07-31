using System.Threading;
using System.Threading.Tasks;

namespace BastelKatalog.Backup
{
    public interface IBackupProvider
    {
        Task<string> CreateBackupArchiveAsync(CancellationToken cancellationToken);
    }
}
using Xamarin.Essentials;

namespace BastelKatalog.Backup
{
    public class BackupPathProvider : IBackupPathProvider
    {
        public string GetBackupPath() => FileSystem.CacheDirectory;
    }
}

using Xamarin.Essentials;

namespace BastelKatalog.Backup
{
    public class FilePathProvider : IFilePathProvider
    {
        public string GetCacheDirectory() => FileSystem.CacheDirectory;
    }
}

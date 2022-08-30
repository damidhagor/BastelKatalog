using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace BastelKatalog.ViewModels
{
    public class BackupViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged = default!;
        private void NotifyPropertyChanged([CallerMemberName] string caller = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(caller));
        #endregion

        private readonly Data.CatalogueContext _CatalogueDb;

        private bool _isBackupRunning;
        public bool IsBackupRunning
        {
            get { return _isBackupRunning; }
            set
            {
                if (value != _isBackupRunning)
                {
                    _isBackupRunning = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _isImportRunning;
        public bool IsImportRunning
        {
            get { return _isImportRunning; }
            set
            {
                if (value != _isImportRunning)
                {
                    _isImportRunning = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _progressStatus = "";
        public string ProgressStatus
        {
            get { return _progressStatus; }
            set
            {
                if (value != _progressStatus)
                {
                    _progressStatus = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private float _progress = 0.0f;
        public float Progress
        {
            get { return _progress; }
            set
            {
                if (value != _progress)
                {
                    _progress = value;
                    NotifyPropertyChanged();
                }
            }
        }


        public BackupViewModel()
        {
            _CatalogueDb = DependencyService.Get<Data.CatalogueContext>();
        }

        public async Task BackupData()
        {
            IsBackupRunning = true;

            var catalogueContext = DependencyService.Resolve<Data.CatalogueContext>();
            var backupPathProvider = DependencyService.Resolve<Backup.IFilePathProvider>();
            var backupProvider = new Backup.BackupProvider(catalogueContext, backupPathProvider);

            var backupZipFilename = await backupProvider.CreateBackupArchiveAsync(UpdateProgress, default);

            await Share.RequestAsync(new ShareFileRequest
            {
                Title = "Bastelkatalog Backup",
                File = new ShareFile(backupZipFilename)
            });

            IsBackupRunning = false;
        }

        public async Task ImportData()
        {
            IsImportRunning = true;

            var importArchiveProvider = DependencyService.Resolve<Backup.IImportArchiveProvider>();
            var catalogueContext = DependencyService.Resolve<Data.CatalogueContext>();
            var backupPathProvider = DependencyService.Resolve<Backup.IFilePathProvider>();
            var importProvider = new Backup.ImportProvider(catalogueContext, backupPathProvider);

            var archiveFilename = await importArchiveProvider.GetArchiveFilename();

            await importProvider.ImportArchiveAsync(archiveFilename, UpdateProgress, default);

            IsImportRunning = false;
        }

        private void UpdateProgress(string status, float progress)
        {
            ProgressStatus = status;
            Progress = progress;
        }
    }
}

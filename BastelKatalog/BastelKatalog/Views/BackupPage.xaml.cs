using System;
using System.Threading.Tasks;
using BastelKatalog.Helper;
using BastelKatalog.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BastelKatalog.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BackupPage : ContentPage
    {
        public BackupViewModel ViewModel;


        public BackupPage()
        {
            InitializeComponent();

            BindingContext = ViewModel = new BackupViewModel();
        }

        private async void Backup_Tapped(object sender, EventArgs e)
        {
            if (await CheckForRunningBackupImport())
                return;

            bool doBackup = await DisplayAlert("Backup", "Möchten Sie ein Backup aller Bastelkatalog-Daten erstellen?", "Ja", "Nein");
            if (!doBackup)
                return;

            if (!await CheckForStoragePermissions())
                return;

            try
            {
                await ViewModel.BackupData();
            }
            catch (InvalidOperationException exc)
            {
                await DisplayAlert("Fehler", exc.Message, "Ok");
            }
        }

        private async void Import_Tapped(object sender, EventArgs e)
        {
            if (await CheckForRunningBackupImport())
                return;

            bool doImport = await DisplayAlert("Import", "Möchten Sie ein Backup importieren? ACHTUNG: Dabei werden alle existierenden Daten überschrieben!", "Ja", "Nein");
            if (!doImport)
                return;

            if (!await CheckForStoragePermissions())
                return;

            try
            {
                await ViewModel.ImportData();
                await DisplayAlert("Import", "Das Backup wurde erfolgreich importiert!", "Ok");
            }
            catch (InvalidOperationException exc)
            {
                await DisplayAlert("Fehler", exc.Message, "Ok");
            }
        }

        private async Task<bool> CheckForRunningBackupImport()
        {
            if (ViewModel.IsBackupRunning || ViewModel.IsImportRunning)
            {
                await DisplayAlert("Fehler", "Ess kann kein Backup oder Import gestartet werden, wenn bereits einer dieser Vorgänge ausgeführt wird.", "ok");
                return true;
            }

            return false;
        }

        private async Task<bool> CheckForStoragePermissions()
        {
            bool hasStorageWritePermission = await PermissionHelper.RequestStorageWritePermission();

            if (!hasStorageWritePermission)
            {
                await DisplayAlert("Fehler", "Der Vorgang kann ohne die nötigen Berechtigungen nicht durchgeführt werden.", "Ok");
                return false;
            }

            return true;
        }
    }
}
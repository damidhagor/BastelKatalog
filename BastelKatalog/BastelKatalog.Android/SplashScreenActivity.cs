using System.Threading.Tasks;
using Android.App;
using Android.OS;
using Android.Runtime;
using BastelKatalog.Backup;
using Microsoft.EntityFrameworkCore;
using Xamarin.Forms;

namespace BastelKatalog.Droid
{
    [Activity(Label = "BastelKatalog", Icon = "@drawable/icon_app", Theme = "@style/SplashScreenStyle", MainLauncher = true, NoHistory = true)]
    public class SplashScreenActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        protected override void OnResume()
        {
            base.OnResume();
            Task.Run(() => StartUp());
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void StartUp()
        {
            // Create DatabaseContext
            Data.CatalogueContext db = new Data.CatalogueContext();
            db.Database.Migrate();
            DependencyService.RegisterSingleton(db);

            DependencyService.Register<IFilePathProvider, FilePathProvider>();
            DependencyService.Register<IBackupProvider, BackupProvider>();

            // Start actual MainActivity
            StartActivity(typeof(MainActivity));
        }
    }
}
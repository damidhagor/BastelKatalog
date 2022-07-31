using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace BastelKatalog.ViewModels
{
    public class SearchViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged = default!;
        private void NotifyPropertyChanged([CallerMemberName] string caller = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(caller));
        #endregion


        private readonly Data.CatalogueContext _CatalogueDb;


        private string _SearchText = "";
        /// <summary>
        /// Search term
        /// </summary>
        public string SearchText
        {
            get { return _SearchText; }
            set
            {
                if (value != _SearchText)
                {
                    _SearchText = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private ObservableCollection<Data.Category> _Categories = new ObservableCollection<Data.Category>();
        public ObservableCollection<Data.Category> Categories
        {
            get { return _Categories; }
            set
            {
                if (value != _Categories)
                {
                    _Categories = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private Data.Category? _SelectedCategory;
        /// <summary>
        /// Selected category for search
        /// </summary>
        public Data.Category? SelectedCategory
        {
            get { return _SelectedCategory; }
            set
            {
                if (value != _SelectedCategory)
                {
                    _SelectedCategory = value;
                    NotifyPropertyChanged();
                }
            }
        }


        public SearchViewModel()
        {
            _CatalogueDb = DependencyService.Get<Data.CatalogueContext>();
        }


        public async Task LoadData()
        {
            try
            {
                SearchText = "";

                _Categories.Clear();
                List<Data.Category> categories = await _CatalogueDb.Categories.OrderBy(c => c.Name).ToListAsync();

                // Prepend "Alle" option for categories
                _Categories.Add(new Data.Category("Alle") { Id = -1 });
                foreach (Data.Category category in categories)
                    _Categories.Add(category);

                SelectedCategory = _Categories[0];
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error loading categories: {e.Message}");
            }
        }

        public async Task BackupData()
        {
            var backupProvider = DependencyService.Resolve<Backup.BackupProvider>();

            var backupZipFilename = await backupProvider.CreateBackupArchiveAsync(default);

            await Share.RequestAsync(new ShareFileRequest
            {
                Title = "Bastelkatalog Backup",
                File = new ShareFile(backupZipFilename)
            });
        }
    }
}

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using BastelKatalog.Models;
using Xamarin.Forms;

namespace BastelKatalog.ViewModels
{
    public class ShowItemViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged = default!;
        private void NotifyPropertyChanged([CallerMemberName] string caller = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(caller));
        #endregion

        private readonly Data.CatalogueContext _CatalogueDb;

        private ItemWrapper _Item;
        public ItemWrapper Item
        {
            get { return _Item; }
            set
            {
                if (value != _Item)
                {
                    _Item = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool IsPreviousImageAvailable => Item.SelectedImageIndex > 1;
        public bool IsNextImageAvailable => Item.SelectedImageIndex < Item.ImageCount;


        public ShowItemViewModel()
        {
            _CatalogueDb = DependencyService.Resolve<Data.CatalogueContext>();
            _Item = new ItemWrapper(new Data.Item(""));
        }


        public void LoadData(int itemId)
        {
            try
            {
                Item = _CatalogueDb.Items.FirstOrDefault(i => i.Id == itemId)?.ToItemWrapper()
                    ?? new ItemWrapper(new Data.Item(""));

                NotifyPropertyChanged(nameof(IsPreviousImageAvailable));
                NotifyPropertyChanged(nameof(IsNextImageAvailable));
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error loading item: {e.Message}");
            }
        }

        public void ShowPreviousImage()
        {
            int imageIndex = Item.SelectedImageIndex - 1;
            if (imageIndex > 0)
                Item.SelectedImage = Item.Images[imageIndex - 1];

            NotifyPropertyChanged(nameof(IsPreviousImageAvailable));
            NotifyPropertyChanged(nameof(IsNextImageAvailable));
        }

        public void ShowNextImage()
        {
            int imageIndex = Item.SelectedImageIndex - 1;
            if (imageIndex < Item.ImageCount)
                Item.SelectedImage = Item.Images[imageIndex + 1];

            NotifyPropertyChanged(nameof(IsPreviousImageAvailable));
            NotifyPropertyChanged(nameof(IsNextImageAvailable));
        }
    }
}

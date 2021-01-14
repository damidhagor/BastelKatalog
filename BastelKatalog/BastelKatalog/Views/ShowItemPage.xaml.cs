using System;
using BastelKatalog.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BastelKatalog.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [QueryProperty(nameof(ItemId), nameof(ItemId))]
    public partial class ShowItemPage : ContentPage
    {
        public ShowItemViewModel ViewModel { get; set; }

        private int _ItemId;
        public string ItemId
        {
            get { return _ItemId.ToString(); }
            set
            {
                if (Int32.TryParse(value, out int itemId) && itemId != _ItemId)
                    _ItemId = itemId;
                else
                    _ItemId = -1;
            }
        }


        public ShowItemPage()
        {
            InitializeComponent();

            BindingContext = ViewModel = new ShowItemViewModel();
        }

        private void OnAppearing(object sender, EventArgs e)
        {
            ViewModel.LoadData(_ItemId);
        }

        private async void Image_Tapped(object sender, EventArgs e)
        {
            await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(new Popups.ViewImagePopupPage(ViewModel.Item.Name, ViewModel.Item.Image));
        }
    }
}
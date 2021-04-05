using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using BastelKatalog.Models;
using BastelKatalog.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BastelKatalog.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [QueryProperty(nameof(IsSearch), nameof(IsSearch))]
    [QueryProperty(nameof(SearchText), nameof(SearchText))]
    [QueryProperty(nameof(SearchCategoryId), nameof(SearchCategoryId))]
    public partial class BrowseItemsPage : ContentPage
    {
        #region INotifyPropertyChanged
        private void NotifyPropertyChanged([CallerMemberName] string caller = "")
            => OnPropertyChanged(caller);
        #endregion


        public BrowseItemsViewModel ViewModel { get; set; }

        private bool _IsSearch = false;
        public bool IsSearch
        {
            get { return _IsSearch; }
            set
            {
                if (value != _IsSearch)
                {
                    _IsSearch = value;
                    NotifyPropertyChanged();
                    ViewModel.SetIsSearch(_IsSearch);
                }
            }
        }

        private string _SearchText = "";
        public string SearchText
        {
            get { return _SearchText; }
            set
            {
                if (value != _SearchText)
                {
                    _SearchText = Uri.UnescapeDataString(value);
                    NotifyPropertyChanged();
                    ViewModel.SetSearchText(_SearchText);
                }
            }
        }

        private int _SearchCategoryId = -1;
        public int SearchCategoryId
        {
            get { return _SearchCategoryId; }
            set
            {
                if (value != _SearchCategoryId)
                {
                    _SearchCategoryId = value;
                    NotifyPropertyChanged();
                    ViewModel.SetSearchCategoryId(_SearchCategoryId);
                }
            }
        }


        public BrowseItemsPage()
        {
            InitializeComponent();

            BindingContext = ViewModel = new BrowseItemsViewModel();
        }


        private async void OnAppearing(object sender, EventArgs e)
        {
            // Display Add Button only if not displaying search results
            if (!IsSearch && ToolbarItems.Count == 0)
                ToolbarItems.Add(new ToolbarItem("", "icon_add.png", () => Add()));

            await ViewModel.LoadItems();
        }

        private async void Add()
        {
            await AppShell.Current.GoToAsync(nameof(EditItemPage));
        }

        private async void Item_Tapped(object sender, EventArgs e)
        {
            if (!((sender as Grid)?.BindingContext is ItemWrapper item))
                return;

            await AppShell.Current.GoToAsync($"{nameof(ShowItemPage)}?ItemId={item.Item.Id}");
        }

        private async void Image_Tapped(object sender, EventArgs e)
        {
            if (!((sender as Image)?.BindingContext is ItemWrapper item))
                return;

            await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(new Popups.ViewImagePopupPage(item.Name, item.SelectedImage.ImageSource));
        }

        private async void AddToProject_Tapped(object sender, EventArgs e)
        {
            if (!((sender as Image)?.BindingContext is ItemWrapper item))
                return;

            await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(new Popups.ChooseProjectPopupPage((ProjectWrapper project, float neededStock) => ViewModel.AddItemToProject(item, project, neededStock)));
        }

        private async void Delete_Tapped(object sender, EventArgs e)
        {
            if (!((sender as Image)?.BindingContext is ItemWrapper item))
                return;

            if ("Ja" == await DisplayActionSheet($"Möchtest du das Item '{item.Name}' löschen?", null, null, "Ja", "Nein"))
                await ViewModel.DeleteItem(item);
        }
    }
}
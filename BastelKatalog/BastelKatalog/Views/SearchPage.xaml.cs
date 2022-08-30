using System;
using System.Threading.Tasks;
using BastelKatalog.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BastelKatalog.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SearchPage : ContentPage
    {
        public SearchViewModel ViewModel;


        public SearchPage()
        {
            InitializeComponent();

            BindingContext = ViewModel = new SearchViewModel();
        }

        private async void OnAppearing(object sender, EventArgs e)
        {
            await ViewModel.LoadData();
        }

        private async void SearchEntry_Completed(object sender, EventArgs e)
        {
            await ExecuteSearch();
        }

        private async void SearchImage_Tapped(object sender, EventArgs e)
        {
            await ExecuteSearch();
        }

        private async Task ExecuteSearch()
        {
            string route = $"{nameof(BrowseItemsPage)}";
            route += $"?{nameof(BrowseItemsPage.IsSearch)}={true}";
            route += $"&{nameof(BrowseItemsPage.SearchText)}={ViewModel.SearchText.ToLower()}";
            route += $"&{nameof(BrowseItemsPage.SearchCategoryId)}={ViewModel.SelectedCategory?.Id ?? -1}";

            await AppShell.Current.GoToAsync(route);
        }
    }
}
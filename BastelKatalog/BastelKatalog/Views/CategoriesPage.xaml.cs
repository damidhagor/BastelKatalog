using System;
using BastelKatalog.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BastelKatalog.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CategoriesPage : ContentPage
    {
        public CategoriesViewModel ViewModel { get; set; }

        public CategoriesPage()
        {
            InitializeComponent();

            BindingContext = ViewModel = new CategoriesViewModel();
        }

        private async void OnAppearing(object sender, EventArgs e)
        {
            await ViewModel.LoadData();
        }

        private async void Add_Clicked(object sender, EventArgs e)
        {
            string name = await DisplayPromptAsync(null, "Bitte Namen der Kategorie eingeben:", "Ok", "Abbrechen");
            if (!String.IsNullOrWhiteSpace(name))
                await ViewModel.AddCategory(name);
        }

        private async void Edit_Tapped(object sender, EventArgs e)
        {
            if (!((sender as Image)?.BindingContext is Data.Category category))
                return;

            string name = await DisplayPromptAsync(null, "Bitte neuen Namen der Kategorie eingeben:", "Ok", "Abbrechen");
            if (!String.IsNullOrWhiteSpace(name))
                await ViewModel.EditCategory(category, name);
        }

        private async void Delete_Tapped(object sender, EventArgs e)
        {
            if (!((sender as Image)?.BindingContext is Data.Category category))
                return;

            if ("Ja" == await DisplayActionSheet($"Soll die Kategorie '{category.Name}' wirklich gelöscht werden?", null, null, "Ja", "Nein"))
                await ViewModel.DeleteCategory(category);
        }
    }
}
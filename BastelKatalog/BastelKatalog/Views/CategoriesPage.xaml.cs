using System;
using BastelKatalog.Models;
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

        private async void AddSub_Tapped(object sender, EventArgs e)
        {
            if (!((sender as Image)?.BindingContext is CategoryWrapper category))
                return;

            string name = await DisplayPromptAsync(null, "Bitte Namen der Sub-Kategorie eingeben:", "Ok", "Abbrechen");
            if (!String.IsNullOrWhiteSpace(name))
                await ViewModel.AddSubCategory(category, name);
        }

        private async void Edit_Tapped(object sender, EventArgs e)
        {
            if (!((sender as Image)?.BindingContext is CategoryWrapper category))
                return;

            string name = await DisplayPromptAsync(null, "Bitte neuen Namen der Kategorie eingeben:", "Ok", "Abbrechen");
            if (!String.IsNullOrWhiteSpace(name))
                await ViewModel.EditCategory(category, name);
        }

        private async void Delete_Tapped(object sender, EventArgs e)
        {
            if (!((sender as Image)?.BindingContext is CategoryWrapper category))
                return;

            string message = category.Category.SubCategories.Count == 0
                            ? $"Soll die Kategorie '{category.Name}' gelöscht werden?"
                            : $"Soll die Kategorie '{category.Name}' und ihre Sub-Kategorien gelöscht werden?";

            if ("Ja" == await DisplayActionSheet(message, null, null, "Ja", "Nein"))
                await ViewModel.DeleteCategory(category);
        }
    }
}
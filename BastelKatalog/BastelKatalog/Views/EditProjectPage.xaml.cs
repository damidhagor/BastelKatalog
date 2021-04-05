using System;
using BastelKatalog.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BastelKatalog.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [QueryProperty(nameof(ProjectId), nameof(ProjectId))]
    public partial class EditProjectPage : ContentPage
    {
        public EditProjectViewModel ViewModel { get; set; }

        private int _ProjectId;
        public string ProjectId
        {
            get { return _ProjectId.ToString(); }
            set
            {
                if (Int32.TryParse(value, out int projectId))
                    _ProjectId = projectId;
                else
                    _ProjectId = -1;
            }
        }


        public EditProjectPage()
        {
            InitializeComponent();

            BindingContext = ViewModel = new EditProjectViewModel();
        }

        private void OnAppearing(object sender, EventArgs e)
        {
            ViewModel.LoadData(_ProjectId);
        }

        private void OnDisappearing(object sender, EventArgs e)
        {
            if (!ViewModel.WasSaved)
                ViewModel.RevertChanges();
        }


        private async void ItemImage_Tapped(object sender, EventArgs e)
        {
            if (!((sender as Image)?.BindingContext is Models.ProjectItemWrapper item))
                return;

            await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(new Popups.ViewImagePopupPage(item.Item.Name, item.Item.SelectedImage.ImageSource));
        }

        private void AddStock_Clicked(object sender, EventArgs e)
        {
            if (!((sender as ImageButton)?.BindingContext is Models.ProjectItemWrapper item))
                return;

            item.NeededStock++;
        }

        private void MinusStock_Clicked(object sender, EventArgs e)
        {
            if (!((sender as Button)?.BindingContext is Models.ProjectItemWrapper item))
                return;

            item.NeededStock--;
        }

        private async void Delete_Tapped(object sender, EventArgs e)
        {
            if (!((sender as Image)?.BindingContext is Models.ProjectItemWrapper item))
                return;

            if ("Ja" == await DisplayActionSheet($"Möchtest du das Item '{item.Item.Name}' aus dem Projekt entfernen?", null, null, "Ja", "Nein"))
                ViewModel.DeleteProjectItem(item);
        }

        private async void Save_Clicked(object sender, EventArgs e)
        {
            // Validate before save
            string? result = ViewModel.Validate();
            if (!String.IsNullOrWhiteSpace(result))
            {
                await DisplayAlert("Projekt kann nicht gespeichert werden", result, "Ok");
                return;
            }

            // Save
            bool saved = await ViewModel.SaveProject();

            if (saved)
                await DisplayAlert("Projekt erfolgreich gespeichert.", null, "Ok");
            else
                await DisplayAlert(null, "Das Projekt konnte nicht gespeichert werden.", "Ok");

            await AppShell.Current.GoToAsync("..");
        }
    }
}
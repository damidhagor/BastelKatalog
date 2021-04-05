using System;
using BastelKatalog.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BastelKatalog.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [QueryProperty(nameof(ProjectId), nameof(ProjectId))]
    public partial class ShowProjectPage : ContentPage
    {
        public ShowProjectViewModel ViewModel { get; set; }

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


        public ShowProjectPage()
        {
            InitializeComponent();

            BindingContext = ViewModel = new ShowProjectViewModel();
        }

        private void OnAppearing(object sender, EventArgs e)
        {
            ViewModel.LoadData(_ProjectId);
        }

        private async void Edit_Clicked(object sender, EventArgs e)
        {
            await AppShell.Current.GoToAsync($"{nameof(EditProjectPage)}?ProjectId={ViewModel.Project.Project.Id}");
        }

        private async void Item_Tapped(object sender, EventArgs e)
        {
            if (!((sender as Grid)?.BindingContext is Models.ProjectItemWrapper item))
                return;

            await AppShell.Current.GoToAsync($"{nameof(ShowItemPage)}?ItemId={item.Item.Item.Id}");
        }

        private async void Image_Tapped(object sender, EventArgs e)
        {
            if (!((sender as Image)?.BindingContext is Models.ProjectItemWrapper item))
                return;

            await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(new Popups.ViewImagePopupPage(item.Item.Name, item.Item.Image));
        }

        private async void Delete_Tapped(object sender, EventArgs e)
        {
            if (!((sender as Image)?.BindingContext is Models.ProjectItemWrapper item))
                return;

            if ("Ja" == await DisplayActionSheet($"Möchtest du das Item '{item.Item.Name}' aus dem Projekt entfernen?", null, null, "Ja", "Nein"))
                await ViewModel.DeleteProjectItem(item);
        }
    }
}
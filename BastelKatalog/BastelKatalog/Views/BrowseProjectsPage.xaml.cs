using System;
using System.Runtime.CompilerServices;
using BastelKatalog.Models;
using BastelKatalog.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BastelKatalog.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BrowseProjectsPage : ContentPage
    {
        #region INotifyPropertyChanged
        private void NotifyPropertyChanged([CallerMemberName] string caller = "")
            => OnPropertyChanged(caller);
        #endregion


        public BrowseProjectsViewModel ViewModel { get; set; }


        public BrowseProjectsPage()
        {
            InitializeComponent();

            BindingContext = ViewModel = new BrowseProjectsViewModel();
        }


        private async void OnAppearing(object sender, EventArgs e)
        {
            await ViewModel.LoadProjects();
        }

        private async void Add_Clicked(object sender, EventArgs e)
        {
            await AppShell.Current.GoToAsync(nameof(EditProjectPage));
        }

        private async void Project_Tapped(object sender, EventArgs e)
        {
            if (!((sender as Grid)?.BindingContext is ProjectWrapper project))
                return;

            await AppShell.Current.GoToAsync($"{nameof(ShowProjectPage)}?ProjectId={project.Project.Id}");
        }

        private async void Delete_Tapped(object sender, EventArgs e)
        {
            if (!((sender as Image)?.BindingContext is ProjectWrapper project))
                return;

            if ("Ja" == await DisplayActionSheet($"Möchtest du das Projekt '{project.Name}' löschen?", null, null, "Ja", "Nein"))
                await ViewModel.DeleteProject(project);
        }
    }
}
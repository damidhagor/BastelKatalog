using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using BastelKatalog.Data;
using BastelKatalog.Models;
using Microsoft.EntityFrameworkCore;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BastelKatalog.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChooseProjectPopupPage : PopupPage
    {
        #region INotifyPropertyChanged
        private void NotifyPropertyChanged([CallerMemberName] string caller = "")
            => OnPropertyChanged(caller);
        #endregion

        private readonly CatalogueContext _CatalogueDb;
        private readonly Func<ProjectWrapper, float, Task> _AddToProjectFunc;

        private ObservableCollection<ProjectWrapper> _Projects = new ObservableCollection<ProjectWrapper>();
        public ObservableCollection<ProjectWrapper> Projects
        {
            get { return _Projects; }
            set
            {
                if (value != _Projects)
                {
                    _Projects = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private ProjectWrapper? _SelectedProject;
        public ProjectWrapper? SelectedProject
        {
            get { return _SelectedProject; }
            set
            {
                if (value != _SelectedProject)
                {
                    _SelectedProject = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged(nameof(IsAddEnabled));
                }
            }
        }

        private float _NeededStock;
        public float NeededStock
        {
            get { return _NeededStock; }
            set
            {
                if (value != _NeededStock)
                {
                    _NeededStock = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool IsAddEnabled => SelectedProject != null;


        public ChooseProjectPopupPage(Func<ProjectWrapper, float, Task> addToProjectFunc)
        {
            _CatalogueDb = DependencyService.Resolve<CatalogueContext>();
            _AddToProjectFunc = addToProjectFunc;

            InitializeComponent();

            BindingContext = this;
        }

        private async void OnAppearing(object sender, EventArgs e)
        {
            await LoadData();
        }


        private async Task LoadData()
        {
            try
            {
                NeededStock = 0;
                List<ProjectWrapper> projects = await _CatalogueDb.Projects.Select(p => p.ToProjectWrapper()).ToListAsync();
                foreach (ProjectWrapper project in projects)
                    Projects.Add(project);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error loading projects: {e.Message}");
            }
        }


        private void AddStock_Clicked(object sender, EventArgs e)
        {
            NeededStock++;
        }

        private void MinusStock_Clicked(object sender, EventArgs e)
        {
            NeededStock--;
        }

        private async void Add_Clicked(object sender, EventArgs e)
        {
            if (SelectedProject != null)
            {
                await _AddToProjectFunc(SelectedProject, NeededStock);
                await Close();
            }
        }

        private async void Cancel_Clicked(object sender, EventArgs e)
        {
            await Close();
        }

        private async void Close_Tapped(object sender, EventArgs e)
        {
            await Close();
        }


        private async Task Close()
        {
            await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopAsync();
        }
    }
}
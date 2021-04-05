using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using BastelKatalog.Data;
using BastelKatalog.Models;
using Microsoft.EntityFrameworkCore;
using Xamarin.Forms;

namespace BastelKatalog.ViewModels
{
    public class BrowseProjectsViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged = default!;
        private void NotifyPropertyChanged([CallerMemberName] string caller = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(caller));
        #endregion


        private readonly CatalogueContext _CatalogueDb;

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


        public BrowseProjectsViewModel()
        {
            _CatalogueDb = DependencyService.Resolve<CatalogueContext>();
        }


        public async Task LoadProjects()
        {
            try
            {
                _Projects.Clear();

                // Get projects
                List<Project> projects = await _CatalogueDb.Projects.Include(p=>p.Items).OrderBy(i => i.Name).ToListAsync();
                foreach (Project project in projects)
                    _Projects.Add(project.ToProjectWrapper());
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error loading projects: {e.Message}");
            }
        }

        public async Task DeleteProject(ProjectWrapper project)
        {
            try
            {
                _CatalogueDb.Projects.Remove(project.Project);
                await _CatalogueDb.SaveChangesAsync();
                Projects.Remove(project);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error deleting project: {e.Message}");
            }
        }
    }
}
